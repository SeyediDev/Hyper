using Hyper.Integration.Contracts;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationMappingApi(HyperIntegrationContext db) : IIntegrationMappingApi
{
    public async Task<IReadOnlyList<IntegrationProductMappingSummary>> ListProductMappingsAsync(
        IntegrationConnectionCommandRequest request, CancellationToken ct) =>
        await db.ExternalProductMappings.AsNoTracking()
            .Where(x => x.ConnectionId == request.ConnectionId && x.ShopId == request.ShopId
                && db.ExternalIntegrationConnections.Any(c => c.Id == x.ConnectionId && c.TenantId == request.TenantId))
            .OrderBy(x => x.Id).Select(x => new IntegrationProductMappingSummary(x.Id, x.ConnectionId,
                x.ShopId, x.HyperProductId, x.ExternalProductId, x.ExternalSku, x.ExternalVariantId,
                x.LastExternalPrice, x.LastExternalInventory, x.LastSyncAtUtc, x.IsActive)).ToListAsync(ct);

    public async Task<IntegrationProductMappingSummary?> CreateProductMappingAsync(
        IntegrationProductMappingRequest request, CancellationToken ct)
    {
        if (request.ShopId <= 0 || string.IsNullOrWhiteSpace(request.TenantId) || request.HyperProductId <= 0
            || string.IsNullOrWhiteSpace(request.ExternalProductId) || request.ExternalProductId.Length > 200)
            return null;
        var connection = await db.ExternalIntegrationConnections.SingleOrDefaultAsync(x => x.Id == request.ConnectionId
            && x.ShopId == request.ShopId && x.TenantId == request.TenantId, ct);
        if (connection is null) return null;
        var mapping = new ExternalProductMapping
        {
            ConnectionId = connection.Id, ShopId = request.ShopId, HyperProductId = request.HyperProductId,
            ExternalProductId = request.ExternalProductId.Trim(), ExternalSku = request.ExternalSku?.Trim(),
            ExternalVariantId = request.ExternalVariantId?.Trim()
        };
        db.ExternalProductMappings.Add(mapping);
        try { await db.SaveChangesAsync(ct); } catch (DbUpdateException) { return null; }
        return ToSummary(mapping);
    }

    public async Task<bool> DeactivateProductMappingAsync(IntegrationConnectionCommandRequest request,
        long mappingId, CancellationToken ct) =>
        await db.ExternalProductMappings.Where(x => x.Id == mappingId && x.ConnectionId == request.ConnectionId
            && x.ShopId == request.ShopId && x.IsActive
            && db.ExternalIntegrationConnections.Any(c => c.Id == x.ConnectionId && c.TenantId == request.TenantId))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false), ct) == 1;

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
