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
        var connection = await db.ExternalIntegrationConnections.SingleOrDefaultAsync(x => x.Id == request.ConnectionId
            && x.ShopId == request.ShopId && x.TenantId == tenantId, ct);
        if (connection is null) return null;
        var mapping = new ExternalProductMapping
        {
            ConnectionId = connection.Id, ShopId = request.ShopId, HyperProductId = request.HyperProductId,
            ExternalProductId = externalProductId, ExternalSku = externalSku.Value,
            ExternalVariantId = externalVariantId.Value
        };
        db.ExternalProductMappings.Add(mapping);
        try { await db.SaveChangesAsync(ct); } catch (DbUpdateException) { return null; }
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

public sealed class IntegrationSyncApi(HyperIntegrationContext db, IIntegrationSynchronizationService synchronization)
    : IIntegrationSyncApi
{
    public async Task<IntegrationSyncTriggerResponse?> TriggerAsync(IntegrationSyncTriggerRequest request, CancellationToken ct)
    {
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id == request.ConnectionId && x.ShopId == request.ShopId && x.TenantId == request.TenantId, ct);
        if (connection is null) return null;
        var runId = await synchronization.SynchronizeAsync(connection.Id, ct);
        return new(runId, "Accepted");
    }
}
