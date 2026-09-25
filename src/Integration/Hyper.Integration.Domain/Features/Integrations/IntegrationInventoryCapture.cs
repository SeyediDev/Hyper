namespace Hyper.Integration.Domain.Features.Integrations;

public interface IIntegrationInventoryCapture
{
    Task<int> CaptureAsync(CancellationToken ct);
    Task<bool> ReconcileOneAsync(long connectionId, long mappingId, CancellationToken ct);
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

