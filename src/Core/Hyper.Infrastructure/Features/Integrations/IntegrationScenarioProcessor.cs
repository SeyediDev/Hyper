using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationScenarioProcessor(HyperIntegrationContext db, IIntegrationStrategyResolver strategies,
    IIntegrationInventoryCapture inventory, IOptions<IntegrationInventoryCaptureOptions> options,
    IOptions<BasalamOAuthSettings> basalamOptions, BasalamDemoProvisioner demoProvisioner)
{
    public async Task<IntegrationScenarioResult> ProcessAsync(IntegrationScenarioJob job, CancellationToken ct)
    {
        if (job.Item is not (IntegrationSyncItem.Product or IntegrationSyncItem.Inventory))
            throw new IntegrationProviderException("BusinessCommandNotImplemented", false);
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id == job.ConnectionId && x.ShopId == job.ShopId && x.TenantId == job.TenantId, ct)
            ?? throw new IntegrationProviderException("ConnectionScopeChanged", false);
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
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
        var rows = await db.Database.SqlQuery<LocalProductRow>($"""
            SELECT p.ID_ AS Id, p.NAME_ AS Title, p.ACCOUNTINGSTOCK_ AS Stock,
              CAST(CASE WHEN p.ISENABLED_=1 AND p.ISSELLABLE_=1 AND p.ISONLINESELLABLE_=1
                AND p.ISSTOCKABLE_=1 AND p.ISSERVICE_=0 THEN 1 ELSE 0 END AS bit) AS CanSell,
              COALESCE((SELECT SUM(r.Quantity) FROM dbo.InventoryReservationLogs r
                WHERE r.ShopId=p.SHOPID_ AND r.HyperProductId=p.ID_ AND r.Status=0 AND r.ReleasedAtUtc IS NULL),0) AS Reserved
            FROM dbo.TBL_Product p JOIN dbo.TBL_Shop s ON s.SHOPID_=p.SHOPID_
            WHERE p.SHOPID_={job.ShopId}
              AND COALESCE(NULLIF(LTRIM(RTRIM(p.TENANT_ID_)),''),CONCAT('shop:',p.SHOPID_))={job.TenantId}
              AND COALESCE(NULLIF(LTRIM(RTRIM(s.TENANT_ID_)),''),CONCAT('shop:',s.SHOPID_))={job.TenantId}
            """).ToListAsync(ct);
        var local = rows.Select(x => new IntegrationLocalProduct(x.Id, x.Title,
            job.Item == IntegrationSyncItem.Inventory ? IntegrationAvailableInventory.Calculate(x.Stock, x.Reserved, x.CanSell) : 0)).ToArray();
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

    private sealed class LocalProductRow
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Stock { get; set; }
        public decimal Reserved { get; set; }
        public bool CanSell { get; set; }
    }
}
