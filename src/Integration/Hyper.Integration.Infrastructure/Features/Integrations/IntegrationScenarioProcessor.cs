using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationScenarioProcessor(HyperIntegrationContext db, IIntegrationStrategyResolver strategies,
    IIntegrationInventoryCapture inventory, IOptions<IntegrationInventoryCaptureOptions> options,
    IOptions<BasalamOAuthSettings> basalamOptions, BasalamDemoProvisioner demoProvisioner,
    IntegrationBusinessEventDispatcher businessEvents, IIntegrationPlatformCatalogPort catalog)
{
    public async Task<IntegrationScenarioResult> ProcessAsync(IntegrationScenarioJob job, CancellationToken ct)
    {
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id == job.ConnectionId && x.ShopId == job.ShopId && x.TenantId == job.TenantId, ct)
            ?? throw new IntegrationProviderException("ConnectionScopeChanged", false);
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        if (job.Item is IntegrationSyncItem.Counterparty or IntegrationSyncItem.Product
            or IntegrationSyncItem.Sale or IntegrationSyncItem.Purchase)
            return await businessEvents.DispatchAsync(job, connection, ct);
        var demoBasalam = connection.Provider == IntegrationProvider.Basalam
            && string.Equals(basalamOptions.Value.Mode, "Demo", StringComparison.OrdinalIgnoreCase);
        if (job.Item == IntegrationSyncItem.Inventory && !options.Value.AccountingStockSourceVerified && !demoBasalam)
            throw new IntegrationProviderException("AccountingStockSourceUnverified", false);
        if (demoBasalam)
            await demoProvisioner.EnsureProductMappingsAsync(connection.Id, connection.ShopId, connection.TenantId, ct);
        // All trigger types re-read both authoritative sources. Simulator/webhook payloads cannot overwrite accounting.
        var adapter = strategies.Resolve(connection.Provider, connection.CredentialType);
        var remote = await adapter.ReadCatalogAsync(connection, ct);
        var mappings = await db.ExternalProductMappings.AsNoTracking().Where(x => x.ConnectionId == connection.Id
            && x.ShopId == connection.ShopId && x.IsActive).ToListAsync(ct);
        IntegrationLocalProduct[] local;
        await using (var snapshot = await db.Database.BeginTransactionAsync(ct))
        {
            await IntegrationInventoryReadLock.AcquireAsync(db, job.ShopId, ct);
            var rows = await catalog.GetProductsAsync(job.ShopId, job.TenantId, ct);
            var reserved = await db.InventoryReservationLogs.AsNoTracking()
                .Where(x => x.ShopId == job.ShopId && x.Status == 0 && x.ReleasedAtUtc == null)
                .GroupBy(x => x.HyperProductId).Select(x => new { ProductId = x.Key, Quantity = x.Sum(r => r.Quantity) })
                .ToDictionaryAsync(x => x.ProductId, x => x.Quantity, ct);
            local = rows.Select(x => new IntegrationLocalProduct(x.ProductId, x.Name,
                job.Item == IntegrationSyncItem.Inventory
                    ? IntegrationAvailableInventory.Calculate(x.Stock, reserved.GetValueOrDefault(x.ProductId), x.CanSell) : 0,
                x.Sku, x.Price)).ToArray();
            await snapshot.CommitAsync(ct);
        }
        var result = IntegrationCatalogComparison.Compare(local, remote, mappings, job.Item);
        var enqueued = 0;
        if (job.Item == IntegrationSyncItem.Inventory)
        {
            foreach (var diff in result.Differences.Where(x => x.Code == "InventoryMismatch"))
            {
                var candidates = mappings.Where(x => x.HyperProductId == diff.HyperProductId
                    && x.ExternalProductId == diff.ExternalProductId && x.ExternalVariantId == diff.VariantId).ToArray();
                if (candidates.Length == 1 && await inventory.ReconcileOneAsync(job.ConnectionId, candidates[0].Id, ct)) enqueued++;
            }
        }
        return result with { Enqueued = enqueued };
    }

}
