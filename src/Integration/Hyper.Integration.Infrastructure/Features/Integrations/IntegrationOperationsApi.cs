using Hyper.Integration.Contracts;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationMappingApi(HyperIntegrationContext db) : IIntegrationMappingApi
{
    public async Task<IReadOnlyList<IntegrationProductMappingSummary>> ListProductMappingsAsync(
        IntegrationConnectionCommandRequest request, CancellationToken ct)
    {
        var tenantId = NormalizeTenant(request.TenantId);
        if (request.ShopId <= 0 || request.ConnectionId <= 0 || tenantId is null) return [];
        return await db.ExternalProductMappings.AsNoTracking()
            .Where(x => x.ConnectionId == request.ConnectionId && x.ShopId == request.ShopId
                && db.ExternalIntegrationConnections.Any(c => c.Id == x.ConnectionId
                    && c.ShopId == request.ShopId && c.TenantId == tenantId))
            .OrderBy(x => x.Id).Select(x => new IntegrationProductMappingSummary(x.Id, x.ConnectionId,
                x.ShopId, x.HyperProductId, x.ExternalProductId, x.ExternalSku, x.ExternalVariantId,
                x.LastExternalPrice, x.LastExternalInventory, x.LastSyncAtUtc, x.IsActive)).ToListAsync(ct);
    }

    public async Task<IntegrationProductMappingSummary?> CreateProductMappingAsync(
        IntegrationProductMappingRequest request, CancellationToken ct)
    {
        var tenantId = NormalizeTenant(request.TenantId);
        var externalProductId = NormalizeText(request.ExternalProductId, 200);
        var externalSku = NormalizeOptional(request.ExternalSku, 200);
        var externalVariantId = NormalizeOptional(request.ExternalVariantId, 200);
        if (request.ShopId <= 0 || request.ConnectionId <= 0 || tenantId is null || request.HyperProductId <= 0
            || externalProductId is null || !externalSku.Valid || !externalVariantId.Valid)
            return null;
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var connection = await db.ExternalIntegrationConnections.FromSqlInterpolated(
            $"SELECT * FROM dbo.ExternalIntegrationConnections WITH (UPDLOCK,HOLDLOCK) WHERE Id={request.ConnectionId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        if (connection is null || connection.ShopId != request.ShopId || connection.TenantId != tenantId) return null;
        if (await db.Set<IntegrationProductCreation>().AnyAsync(x => x.ConnectionId == connection.Id && x.State != 3
            && (x.HyperProductId == request.HyperProductId || x.ExternalProductId == externalProductId), ct)) return null;
        var mapping = new ExternalProductMapping
        {
            ConnectionId = connection.Id, ShopId = request.ShopId, HyperProductId = request.HyperProductId,
            ExternalProductId = externalProductId, ExternalSku = externalSku.Value,
            ExternalVariantId = externalVariantId.Value
        };
        db.ExternalProductMappings.Add(mapping);
        try { await db.SaveChangesAsync(ct); } catch (DbUpdateException) { return null; }
        await tx.CommitAsync(ct);
        return ToSummary(mapping);
    }

    public async Task<bool> DeactivateProductMappingAsync(IntegrationConnectionCommandRequest request,
        long mappingId, CancellationToken ct)
    {
        var tenantId = NormalizeTenant(request.TenantId);
        if (mappingId <= 0 || request.ConnectionId <= 0 || request.ShopId <= 0 || tenantId is null) return false;
        return await db.ExternalProductMappings.Where(x => x.Id == mappingId && x.ConnectionId == request.ConnectionId
            && x.ShopId == request.ShopId && x.IsActive
            && db.ExternalIntegrationConnections.Any(c => c.Id == x.ConnectionId
                && c.ShopId == request.ShopId && c.TenantId == tenantId))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false), ct) == 1;
    }

    private static string? NormalizeTenant(string? value) => NormalizeText(value, 30);

    private static string? NormalizeText(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        return normalized.Length == 0 || normalized.Length > maxLength || normalized.Any(char.IsControl)
            ? null : normalized;
    }

    private static OptionalText NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return new(true, null);
        var normalized = value.Trim();
        return normalized.Length > maxLength || normalized.Any(char.IsControl) ? new(false, null) : new(true, normalized);
    }

    private readonly record struct OptionalText(bool Valid, string? Value);

    private static IntegrationProductMappingSummary ToSummary(ExternalProductMapping x) =>
        new(x.Id, x.ConnectionId, x.ShopId, x.HyperProductId, x.ExternalProductId, x.ExternalSku,
            x.ExternalVariantId, x.LastExternalPrice, x.LastExternalInventory, x.LastSyncAtUtc, x.IsActive);
}

public sealed class IntegrationSyncApi(HyperIntegrationContext db, IIntegrationSynchronizationService synchronization,
    IIntegrationScenarioQueue? scenarios = null)
    : IIntegrationSyncApi
{
    public async Task<IntegrationCatalogReconciliationResponse?> ReconcileCatalogAsync(
        IntegrationCatalogReconciliationRequest request, CancellationToken ct)
    {
        if (request.RequestId == Guid.Empty) throw new ArgumentException("InvalidReconciliationRequestId");
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id == request.ConnectionId && x.ShopId == request.ShopId && x.TenantId == request.TenantId, ct);
        if (connection is null) return null;
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        if (scenarios is null) throw new InvalidOperationException("ScenarioQueueUnavailable");
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var shop = new OwnedIntegrationShop(request.ShopId, request.TenantId);
        // Stable identities plus one transaction make a repeated start/poll safe;
        // no provider call or incomplete single-job pair is exposed by ingress.
        var prefix = $"catalog:{request.RequestId:N}:";
        var productId = await scenarios.EnqueueAsync(shop, connection.Id,
            new(prefix + "product", IntegrationSyncItem.Product, IntegrationSyncTrigger.Manual), ct);
        var inventoryId = await scenarios.EnqueueAsync(shop, connection.Id,
            new(prefix + "inventory", IntegrationSyncItem.Inventory, IntegrationSyncTrigger.Manual), ct);
        var jobs = await db.IntegrationScenarioJobs.AsNoTracking()
            .Where(x => x.Id == productId || x.Id == inventoryId).ToListAsync(ct);
        IntegrationCatalogJobStatus Status(long id)
        {
            var job = jobs.Single(x => x.Id == id);
            return new(job.Id, job.Status.ToString(), job.ErrorCode, job.CompletedAtUtc);
        }
        await transaction.CommitAsync(ct);
        return new(request.RequestId, Status(productId), Status(inventoryId));
    }

    public async Task<IntegrationSyncTriggerResponse?> TriggerAsync(IntegrationSyncTriggerRequest request, CancellationToken ct)
    {
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id == request.ConnectionId && x.ShopId == request.ShopId && x.TenantId == request.TenantId, ct);
        if (connection is null) return null;
        var runId = await synchronization.SynchronizeAsync(connection.Id, ct);
        return new(runId, "Accepted");
    }
}
