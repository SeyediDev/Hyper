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
    IIntegrationStrategyResolver strategies, IOptions<IntegrationInventoryCaptureOptions> options,
    IIntegrationPlatformCatalogPort catalog) : IIntegrationInventoryCapture
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

    public Task<bool> ReconcileOneAsync(long connectionId, long mappingId, CancellationToken ct) => CaptureOneAsync(connectionId, mappingId, ct, true);

    public async Task<bool> CaptureOneAsync(long connectionId, long mappingId, CancellationToken ct, bool forceResend = false)
    {
        if (!options.Value.AccountingStockSourceVerified) return false;
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var mapping = await db.ExternalProductMappings.FromSqlInterpolated($"SELECT * FROM dbo.ExternalProductMappings WITH (UPDLOCK,HOLDLOCK) WHERE Id={mappingId}")
            .AsNoTracking().SingleOrDefaultAsync(ct);
        var connection = await db.ExternalIntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == connectionId, ct);
        if (mapping is null || connection is null || !mapping.IsActive || mapping.ConnectionId != connectionId || mapping.ShopId != connection.ShopId)
            return false;
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        await IntegrationInventoryReadLock.AcquireAsync(db, connection.ShopId, ct);
        // Accounting owns gross stock and sellability. Only Integration's active
        // holds are read here; the two databases never need a cross-domain join.
        var source = (await catalog.GetProductsAsync(connection.ShopId, connection.TenantId, ct))
            .SingleOrDefault(x => x.ProductId == mapping.HyperProductId);
        if (source is null) return false;
        var reserved = await db.InventoryReservationLogs.Where(x => x.ShopId == connection.ShopId
                && x.HyperProductId == mapping.HyperProductId && x.Status == 0 && x.ReleasedAtUtc == null)
            .SumAsync(x => x.Quantity, ct);
        var quantity = IntegrationAvailableInventory.Calculate(source.Stock, reserved, source.CanSell);
        var latest = await db.IntegrationOutbox.AsNoTracking().Where(x => x.MappingId == mappingId)
            .OrderByDescending(x => x.SourceVersion).FirstOrDefaultAsync(ct);
        var update = new ExternalInventoryUpdate(mapping.ExternalProductId, mapping.ExternalVariantId, quantity);
        if (latest is not null && latest.PayloadJson == JsonSerializer.Serialize(update)
            && (!forceResend || latest.Status is 0 or 1)) return false;
        var version = checked((latest?.SourceVersion ?? 0) + 1);
        await outbox.EnqueueInventoryAsync(connectionId, mappingId, version, quantity, ct);
        await transaction.CommitAsync(ct);
        return true;
    }

}

internal static class IntegrationInventoryReadLock
{
    // Same transaction-owned lock as reservation create/release/commit: an ACK
    // cannot remove a hold between reading gross stock and reading active holds.
    public static Task AcquireAsync(HyperIntegrationContext db, int shopId, CancellationToken ct) =>
        db.Database.ExecuteSqlInterpolatedAsync($"""
            DECLARE @result int;
            EXEC @result=sys.sp_getapplock @Resource={"integration-reservations:" + shopId},
                @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=15000;
            IF @result < 0 THROW 51000, 'InventoryReadLockUnavailable', 1;
            """, ct);
}
