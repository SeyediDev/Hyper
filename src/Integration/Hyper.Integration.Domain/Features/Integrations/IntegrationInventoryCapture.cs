namespace Hyper.Integration.Domain.Features.Integrations;

public interface IIntegrationInventoryCapture
{
    Task<int> CaptureAsync(CancellationToken ct);
    Task<bool> ReconcileOneAsync(long connectionId, long mappingId, CancellationToken ct);
}

public interface IIntegrationInventoryReservation
{
    Task ReserveAsync(OwnedIntegrationShop shop, string reservationKey,
        IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct);
    Task ReleaseAsync(OwnedIntegrationShop shop, string reservationKey, CancellationToken ct);
    Task CommitAsync(OwnedIntegrationShop shop, string reservationKey, CancellationToken ct);
}

public static class IntegrationReservationIdentity
{
    public static string ForOrder(OwnedIntegrationShop shop, long connectionId, string externalOrderId)
    {
        if (shop.ShopId <= 0 || string.IsNullOrWhiteSpace(shop.TenantId) || connectionId <= 0
            || string.IsNullOrWhiteSpace(externalOrderId) || externalOrderId.Length > 128)
            throw new ArgumentException("InvalidReservationScope");
        var identity = System.Text.Json.JsonSerializer.Serialize(new { shop.ShopId, shop.TenantId, connectionId, externalOrderId });
        return "order:" + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(identity)));
    }
}

public static class IntegrationAvailableInventory
{
    // DEC-005: whole-shop stock less outstanding reservations, in the product's base unit.
    public static decimal Calculate(decimal accountingStock, decimal reserved, bool canSell)
    {
        if (reserved < 0) throw new InvalidOperationException("InvalidReservationBalance");
        return canSell ? Math.Max(0, accountingStock - reserved) : 0;
    }
}

