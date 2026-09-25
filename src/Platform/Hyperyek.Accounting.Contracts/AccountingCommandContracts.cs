namespace Hyperyek.Accounting.Contracts;

public sealed record AccountingScope(int ShopId, string TenantId);

public sealed record CounterpartyCommand(
    string EventId, AccountingScope Scope, string ExternalCustomerId,
    string? DisplayName, string? Mobile, string? NationalCode);

public sealed record AccountingCustomerIdentity(string Name, string? Mobile,
    string? IdentifierNumber, string ExternalCustomerId);
public sealed record ValidateCustomerCommand(AccountingScope Scope, int PersonId,
    AccountingCustomerIdentity Customer);
public sealed record ResolveCustomerCommand(AccountingScope Scope, AccountingCustomerIdentity Customer);

public sealed record AccountingShopRead(int ShopId, string ShopName, string MerchantIdentifier, string TenantId);
public sealed record AccountingProductRead(int ProductId, string Name, string? Sku, decimal Price,
    decimal Stock, bool IsEnabled, bool IsStockable, decimal? MinimumStock);
public sealed record AccountingOverviewDay(DateTime Date, int Invoices);
public sealed record AccountingPlatformOverview(int Shops, int Products, int ActiveProducts, int People,
    int Invoices, int LowStockProducts, IReadOnlyList<AccountingOverviewDay> InvoiceTrend);

public interface IAccountingPlatformReadHandler
{
    Task<IReadOnlyList<AccountingShopRead>> SearchShopsAsync(string? search, CancellationToken ct);
    Task<IReadOnlyList<AccountingShopRead>> GetShopsAsync(IReadOnlyCollection<int> shopIds, CancellationToken ct);
    Task<AccountingShopRead?> GetShopAsync(int shopId, CancellationToken ct);
    Task<IReadOnlyList<AccountingProductRead>> GetProductsAsync(AccountingScope scope, CancellationToken ct);
    Task<AccountingPlatformOverview> GetOverviewAsync(int days, AccountingScope? scope, CancellationToken ct);
}

public sealed record OrderLineCommand(
    int HyperProductId, decimal Quantity, decimal UnitPrice,
    string? ExternalProductId = null, string? ExternalVariantId = null);

public sealed record VendorOrderCommand(
    string EventId, AccountingScope Scope, long ConnectionId,
    string ExternalOrderId, string? ExternalParcelId, string ExternalCustomerId,
    IReadOnlyCollection<OrderLineCommand> Lines, decimal TotalAmount, byte PaymentStatus,
    int? AccountingCustomerId = null);

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

public sealed record ExternalProductChangedCommand(
    string EventId, AccountingScope Scope, long ConnectionId,
    int HyperProductId, string ExternalProductId, string? ExternalVariantId,
    string? Sku, string Title, decimal? Price, decimal? Inventory, long SourceVersion);

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

public interface IAccountingCommandHandler
{
    Task<AccountingCommandResult> ValidateCustomerAsync(ValidateCustomerCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ResolveCustomerAsync(ResolveCustomerCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ApplyCounterpartyAsync(CounterpartyCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ApplyVendorOrderAsync(VendorOrderCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ApplyCustomerOrderAsync(CustomerOrderCommand command, CancellationToken ct);
    Task<AccountingCommandResult> CancelOrderAsync(CancelOrderCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ApplyParcelStatusAsync(ParcelStatusCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ApplyExternalProductChangedAsync(ExternalProductChangedCommand command, CancellationToken ct);
}
