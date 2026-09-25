using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationInventoryReservation(HyperIntegrationContext db,
    IIntegrationPlatformCatalogPort catalog) : IIntegrationInventoryReservation
{
    public async Task ReserveAsync(OwnedIntegrationShop shop, string reservationKey,
        IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(reservationKey) || reservationKey.Length > 200)
            throw new ArgumentException("InvalidReservationKey", nameof(reservationKey));
        if (lines.Count == 0) throw new ArgumentException("MissingOrderLines", nameof(lines));
        var requested = lines.GroupBy(x => x.HyperProductId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        if (requested.Keys.Any(x => x <= 0) || requested.Values.Any(x => x <= 0))
            throw new ArgumentException("InvalidReservationQuantity", nameof(lines));

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var existing = await db.InventoryReservationLogs.Where(x => x.ShopId == shop.ShopId
            && x.ReservationKey == reservationKey).ToListAsync(ct);
        if (existing.Count > 0)
        {
            if (existing.Any(x => x.Status != 0 || x.ReleasedAtUtc is not null))
                throw new InvalidOperationException("ReservationAlreadyReleased");
            await transaction.CommitAsync(ct);
            return;
        }

        var products = await catalog.GetProductsAsync(shop.ShopId, shop.TenantId, ct);
        foreach (var item in requested)
        {
            var product = products.SingleOrDefault(x => x.ProductId == item.Key);
            if (product is null || !product.IsEnabled || !product.IsStockable)
                throw new IntegrationProviderException("ProductNotSellable", false);
            var reserved = await db.InventoryReservationLogs.AsNoTracking().Where(x => x.ShopId == shop.ShopId
                && x.HyperProductId == item.Key && x.Status == 0 && x.ReleasedAtUtc == null)
                .SumAsync(x => (decimal?)x.Quantity, ct) ?? 0;
            if (IntegrationAvailableInventory.Calculate(product.Stock, reserved, true) < item.Value)
                throw new IntegrationProviderException("InsufficientAvailableInventory", false);
            db.InventoryReservationLogs.Add(new InventoryReservationLog
            {
                ShopId = shop.ShopId, HyperProductId = item.Key, ReservationKey = reservationKey,
                Quantity = item.Value, Source = "basalam-order"
            });
        }
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public async Task ReleaseAsync(OwnedIntegrationShop shop, string reservationKey, CancellationToken ct)
    {
        var rows = await db.InventoryReservationLogs.Where(x => x.ShopId == shop.ShopId
            && x.ReservationKey == reservationKey && x.Status == 0 && x.ReleasedAtUtc == null).ToListAsync(ct);
        foreach (var row in rows)
        {
            row.Status = 1;
            row.ReleasedAtUtc = DateTime.UtcNow;
        }
        if (rows.Count > 0) await db.SaveChangesAsync(ct);
    }
}
