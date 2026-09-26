using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hyperyek.Accounting.Contracts;

namespace Hyperyek.Accounting.Infrastructure;

public sealed class AccountingInventoryOptions
{
    // Enable only after checking ASM-001 and that invoice/stock-card writers
    // do not independently debit the same stock for this API's invoices.
    public bool AccountingStockSourceVerified { get; set; }
}

internal static class AccountingOrderIdentity
{
    public static string Marker(AccountingScope scope, long connectionId, string orderId)
    {
        if (scope is null || scope.ShopId <= 0 || string.IsNullOrWhiteSpace(scope.TenantId)
            || scope.TenantId.Length > 30 || connectionId <= 0 || string.IsNullOrWhiteSpace(orderId)
            || orderId.Length > 128 || orderId.Any(char.IsControl))
            throw new ArgumentException("InvalidAccountingOrderScope");
        return "integration-order:v2:" + Hash(new { scope.ShopId, scope.TenantId, connectionId, orderId }) + ";";
    }

    public static string Fingerprint(VendorOrderCommand command) => "payload:" + Hash(new
    {
        command.AccountingCustomerId, command.ExternalCustomerId, TotalAmount = Number(command.TotalAmount), command.PaymentStatus,
        Lines = command.Lines.OrderBy(x => x.HyperProductId).ThenBy(x => x.UnitPrice)
            .ThenBy(x => x.ExternalProductId, StringComparer.Ordinal).ThenBy(x => x.ExternalVariantId, StringComparer.Ordinal)
            .ThenBy(x => x.Quantity).Select(x => new { x.HyperProductId, x.ExternalProductId, x.ExternalVariantId,
                Quantity = Number(x.Quantity), UnitPrice = Number(x.UnitPrice) }).ToArray()
    }) + ";";

    public static string? Validate(VendorOrderCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.EventId) || command.EventId.Length > 128
            || string.IsNullOrWhiteSpace(command.ExternalCustomerId) || command.ExternalCustomerId.Length > 128
            || command.AccountingCustomerId is null or <= 0 || command.Lines is null || command.Lines.Count is 0 or > 1000)
            return "InvalidAccountingOrder";
        if (command.Lines.Any(x => x.HyperProductId <= 0 || x.Quantity <= 0 || x.Quantity > 1_000_000
            || decimal.Round(x.Quantity, 3) != x.Quantity || x.UnitPrice < 0 || x.UnitPrice > 1_000_000_000
            || decimal.Round(x.UnitPrice, 2) != x.UnitPrice
            || decimal.Round(x.UnitPrice * x.Quantity, 3) != x.UnitPrice * x.Quantity)) return "InvalidAccountingOrderLine";
        if (command.TotalAmount < 0 || command.TotalAmount >= 1_000_000_000_000_000_000m
            || decimal.Round(command.TotalAmount, 3) != command.TotalAmount
            || command.TotalAmount != command.Lines.Sum(x => x.Quantity * x.UnitPrice))
            return "OrderTotalRequiresAccountingPolicy";
        return null;
    }

    private static string Number(decimal value) => value.ToString("G29", System.Globalization.CultureInfo.InvariantCulture);
    private static string Hash<T>(T value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value))));
}
