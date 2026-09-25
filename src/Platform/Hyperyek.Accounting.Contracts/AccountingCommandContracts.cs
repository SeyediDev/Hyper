namespace Hyperyek.Accounting.Contracts;

public sealed record AccountingScope(int ShopId, string TenantId);

public sealed record CounterpartyCommand(
    string EventId, AccountingScope Scope, string ExternalCustomerId,
    string? DisplayName, string? Mobile, string? NationalCode);

public sealed record OrderLineCommand(
    int HyperProductId, decimal Quantity, decimal UnitPrice,
    string? ExternalProductId = null, string? ExternalVariantId = null);

public sealed record VendorOrderCommand(
    string EventId, AccountingScope Scope, long ConnectionId,
    string ExternalOrderId, string? ExternalParcelId, string ExternalCustomerId,
    IReadOnlyCollection<OrderLineCommand> Lines, decimal TotalAmount, byte PaymentStatus);

public sealed record CustomerOrderCommand(
    string EventId, AccountingScope Scope, long ConnectionId,
    string ExternalOrderId, IReadOnlyCollection<OrderLineCommand> Lines,
    decimal TotalAmount, byte PaymentStatus);

public sealed record CancelOrderCommand(
    string EventId, AccountingScope Scope, long ConnectionId,
    string ExternalOrderId, string Reason);

public sealed record ParcelStatusCommand(
    string EventId, AccountingScope Scope, long ConnectionId,
    string ExternalOrderId, string ExternalParcelId, string Status, string? TrackingCode);

public enum AccountingCommandStatus : byte
{
    Applied = 1,
    Duplicate = 2,
    PendingDependency = 3,
    Rejected = 4
}

public sealed record AccountingCommandResult(
    AccountingCommandStatus Status, string? InternalReference = null,
    string? ErrorCode = null);
