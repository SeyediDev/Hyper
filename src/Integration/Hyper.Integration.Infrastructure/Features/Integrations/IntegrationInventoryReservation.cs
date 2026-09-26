using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationInventoryReservation(HyperIntegrationContext db,
    IIntegrationPlatformCatalogPort catalog, IOptions<IntegrationInventoryCaptureOptions> options) : IIntegrationInventoryReservation
{
    public async Task ReserveAsync(OwnedIntegrationShop shop, string reservationKey,
        IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct)
    {
        if (!options.Value.AccountingStockSourceVerified)
            throw new IntegrationProviderException("AccountingStockSourceUnverified", false);
        if (string.IsNullOrWhiteSpace(reservationKey) || reservationKey.Length > 170)
            throw new ArgumentException("InvalidReservationKey", nameof(reservationKey));
        if (lines.Count == 0) throw new ArgumentException("MissingOrderLines", nameof(lines));
        if (shop.ShopId <= 0 || string.IsNullOrWhiteSpace(shop.TenantId)
            || lines.Any(x => x.HyperProductId <= 0 || x.Quantity <= 0 || decimal.Round(x.Quantity, 3) != x.Quantity))
            throw new ArgumentException("InvalidReservationQuantity", nameof(lines));
        var requested = lines.GroupBy(x => x.HyperProductId).OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        if (requested.Keys.Any(x => x <= 0) || requested.Values.Any(x => x <= 0))
            throw new ArgumentException("InvalidReservationQuantity", nameof(lines));

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        await LockShopAsync(shop.ShopId, ct);
        var prefix = reservationKey + ":product:";
        var existing = await db.InventoryReservationLogs.Where(x => x.ShopId == shop.ShopId
            && (x.ReservationKey == reservationKey || x.ReservationKey.StartsWith(prefix))).ToListAsync(ct);
        if (existing.Count > 0)
        {
            if (existing.Any(x => x.Status is not (0 or 2) || x.ReleasedAtUtc is not null))
                throw new InvalidOperationException("ReservationAlreadyReleased");
            if (existing.Count != requested.Count || existing.Any(x =>
                !requested.TryGetValue(x.HyperProductId, out var quantity) || quantity != x.Quantity))
                throw new IntegrationProviderException("ReservationContentChanged", false);
            await transaction.CommitAsync(ct);
            return;
        }

        var products = await catalog.GetProductsAsync(shop.ShopId, shop.TenantId, ct);
        foreach (var item in requested)
        {
            var product = products.SingleOrDefault(x => x.ProductId == item.Key);
            if (product is null || !product.IsEnabled || !product.IsStockable || !product.CanSell)
                throw new IntegrationProviderException("ProductNotSellable", false);
            var reserved = await db.InventoryReservationLogs.AsNoTracking().Where(x => x.ShopId == shop.ShopId
                && x.HyperProductId == item.Key && x.Status == 0 && x.ReleasedAtUtc == null)
                .SumAsync(x => (decimal?)x.Quantity, ct) ?? 0;
            if (IntegrationAvailableInventory.Calculate(product.Stock, reserved, true) < item.Value)
                throw new IntegrationProviderException("InsufficientAvailableInventory", false);
        }
        foreach (var item in requested)
        {
            db.InventoryReservationLogs.Add(new InventoryReservationLog
            {
                ShopId = shop.ShopId, HyperProductId = item.Key, ReservationKey = prefix + item.Key,
                Quantity = item.Value, Source = "marketplace-order"
            });
        }
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public async Task ReleaseAsync(OwnedIntegrationShop shop, string reservationKey, CancellationToken ct)
    {
        if (shop.ShopId <= 0 || string.IsNullOrWhiteSpace(shop.TenantId)
            || string.IsNullOrWhiteSpace(reservationKey) || reservationKey.Length > 170)
            throw new ArgumentException("InvalidReservationScope");
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        await LockShopAsync(shop.ShopId, ct);
        var prefix = reservationKey + ":product:";
        var rows = await db.InventoryReservationLogs.Where(x => x.ShopId == shop.ShopId
            && (x.ReservationKey == reservationKey || x.ReservationKey.StartsWith(prefix))
            && x.Status == 0 && x.ReleasedAtUtc == null).ToListAsync(ct);
        foreach (var row in rows)
        {
            row.Status = 1;
            row.ReleasedAtUtc = DateTime.UtcNow;
        }
        if (rows.Count > 0) await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    private Task LockShopAsync(int shopId, CancellationToken ct) => db.Database.ExecuteSqlInterpolatedAsync($"""
        DECLARE @result int;
        EXEC @result = sys.sp_getapplock @Resource={"integration-reservations:" + shopId},
            @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=15000;
        IF @result < 0 THROW 51000, 'ReservationLockUnavailable', 1;
        """, ct);

    public async Task CommitAsync(OwnedIntegrationShop shop, string reservationKey, CancellationToken ct)
    {
        if (shop.ShopId <= 0 || string.IsNullOrWhiteSpace(shop.TenantId)
            || string.IsNullOrWhiteSpace(reservationKey) || reservationKey.Length > 170)
            throw new ArgumentException("InvalidReservationScope");
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        await LockShopAsync(shop.ShopId, ct);
        var prefix = reservationKey + ":product:";
        var rows = await db.InventoryReservationLogs.Where(x => x.ShopId == shop.ShopId
            && (x.ReservationKey == reservationKey || x.ReservationKey.StartsWith(prefix))).ToListAsync(ct);
        if (rows.Count == 0) throw new InvalidOperationException("ReservationNotFound");
        // A concurrent acknowledged cancellation is terminal. A late sale ACK
        // must not turn a released reservation back into a live one.
        foreach (var row in rows.Where(x => x.Status == 0 && x.ReleasedAtUtc == null)) row.Status = 2;
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}
