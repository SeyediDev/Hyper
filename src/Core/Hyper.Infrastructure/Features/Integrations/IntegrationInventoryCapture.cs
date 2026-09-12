using System.Data;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationInventoryCaptureOptions
{
    // ASM-001: enable only after validating that the legacy writer maintains ACCOUNTINGSTOCK_
    // as gross stock and that native reservations have not already been deducted from it.
    public bool AccountingStockSourceVerified { get; set; }
}

public sealed class IntegrationInventoryCapture(HyperIntegrationContext db, IIntegrationOutbox outbox,
    IIntegrationStrategyResolver strategies, IOptions<IntegrationInventoryCaptureOptions> options) : IIntegrationInventoryCapture
{
    public async Task<int> CaptureAsync(CancellationToken ct)
    {
        if (!options.Value.AccountingStockSourceVerified) return 0;
        var captured = 0;
        long cursor = 0;
        while (true)
        {
            var now = DateTime.UtcNow;
            var mappings = await (from mapping in db.ExternalProductMappings.AsNoTracking()
                                  join connection in db.ExternalIntegrationConnections.AsNoTracking() on mapping.ConnectionId equals connection.Id
                                  where mapping.Id > cursor && mapping.IsActive && mapping.ShopId == connection.ShopId
                                    && connection.IsEnabled && (connection.ExpiresAtUtc == null || connection.ExpiresAtUtc > now)
                                  orderby mapping.Id
                                  select new { mapping.Id, mapping.ConnectionId, connection.Provider, connection.CredentialType })
                .Take(100).ToListAsync(ct);
            if (mappings.Count == 0) return captured;
            foreach (var candidate in mappings)
            {
                cursor = candidate.Id;
                try { strategies.Resolve(candidate.Provider, candidate.CredentialType); }
                catch (NotSupportedException) { continue; }
                if (await CaptureOneAsync(candidate.ConnectionId, candidate.Id, ct)) captured++;
            }
        }
    }

    public async Task<bool> CaptureOneAsync(long connectionId, long mappingId, CancellationToken ct)
    {
        if (!options.Value.AccountingStockSourceVerified) return false;
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var mapping = await db.ExternalProductMappings.FromSqlInterpolated($"SELECT * FROM dbo.ExternalProductMappings WITH (UPDLOCK,HOLDLOCK) WHERE Id={mappingId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == connectionId, ct);
        if (mapping is null || connection is null || !mapping.IsActive || mapping.ConnectionId != connectionId || mapping.ShopId != connection.ShopId)
            return false;
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        // Read only. No triggers, CDC enabling, update timestamp or schema changes on legacy tables.
        var source = await db.Database.SqlQuery<InventorySourceRow>($"""
            SELECT p.ACCOUNTINGSTOCK_ AS AccountingStock,
              CAST(CASE WHEN p.ISENABLED_=1 AND p.ISSELLABLE_=1 AND p.ISONLINESELLABLE_=1
                     AND p.ISSTOCKABLE_=1 AND p.ISSERVICE_=0 THEN 1 ELSE 0 END AS bit) AS CanSell,
              COALESCE((SELECT SUM(r.Quantity) FROM dbo.InventoryReservationLogs r
                WHERE r.ShopId=p.SHOPID_ AND r.HyperProductId=p.ID_ AND r.Status=0 AND r.ReleasedAtUtc IS NULL),0) AS Reserved
            FROM dbo.TBL_Product p JOIN dbo.TBL_Shop s ON p.SHOPID_=s.SHOPID_
            WHERE p.ID_={mapping.HyperProductId} AND p.SHOPID_={connection.ShopId}
              AND COALESCE(NULLIF(LTRIM(RTRIM(p.TENANT_ID_)),''), CONCAT('shop:',p.SHOPID_))={connection.TenantId}
              AND COALESCE(NULLIF(LTRIM(RTRIM(s.TENANT_ID_)),''), CONCAT('shop:',s.SHOPID_))={connection.TenantId}
            """).SingleOrDefaultAsync(ct);
        if (source is null) return false;
        var quantity = IntegrationAvailableInventory.Calculate(source.AccountingStock, source.Reserved, source.CanSell);
        var latest = await db.IntegrationOutbox.AsNoTracking().Where(x => x.MappingId == mappingId)
            .OrderByDescending(x => x.SourceVersion).FirstOrDefaultAsync(ct);
        var update = new ExternalInventoryUpdate(mapping.ExternalProductId, mapping.ExternalVariantId, quantity);
        if (latest is not null && latest.PayloadJson == JsonSerializer.Serialize(update)) return false;
        var version = checked((latest?.SourceVersion ?? 0) + 1);
        await outbox.EnqueueInventoryAsync(connectionId, mappingId, version, quantity, ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    private sealed class InventorySourceRow
    {
        public decimal AccountingStock { get; set; }
        public decimal Reserved { get; set; }
        public bool CanSell { get; set; }
    }
}
