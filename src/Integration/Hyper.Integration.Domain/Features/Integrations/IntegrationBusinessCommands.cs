namespace Hyper.Integration.Domain.Features.Integrations;

/// <summary>
/// Boundary for applying marketplace business events to Hyperyek.
/// Integration owns the external identity and orchestration; Hyperyek owns
/// accounting/order persistence and must implement these commands through its
/// supported application port.
/// </summary>
public interface IIntegrationBusinessCommandPort
{
    Task<BusinessCommandResult> ApplyCounterpartyAsync(
        IntegrationCounterpartyCommand command, CancellationToken cancellationToken);

    Task<BusinessCommandResult> ApplyVendorOrderAsync(
        IntegrationVendorOrderCommand command, CancellationToken cancellationToken);

    Task<BusinessCommandResult> ApplyCustomerOrderAsync(
        IntegrationCustomerOrderCommand command, CancellationToken cancellationToken);

    Task<BusinessCommandResult> CancelOrderAsync(
        IntegrationOrderCancellationCommand command, CancellationToken cancellationToken);
    Task<BusinessCommandResult> ApplyParcelStatusAsync(
        IntegrationParcelStatusCommand command, CancellationToken cancellationToken);
    Task<BusinessCommandResult> ApplyExternalProductChangedAsync(
        IntegrationExternalProductChangedCommand command, CancellationToken cancellationToken);
}

public sealed record IntegrationCounterpartyCommand(
    string EventId, int ShopId, string TenantId, string ExternalCustomerId,
    string? DisplayName, string? Mobile, string? NationalCode);

public sealed record IntegrationOrderLineCommand(
    int HyperProductId, string? ExternalProductId, string? ExternalVariantId,
    decimal Quantity, decimal UnitPrice);

public sealed record IntegrationVendorOrderCommand(
    string EventId, int ShopId, string TenantId, long ConnectionId,
    string ExternalOrderId, string? ExternalParcelId, string ExternalCustomerId,
    IReadOnlyCollection<IntegrationOrderLineCommand> Lines, decimal TotalAmount,
    SalePaymentStatus PaymentStatus, int? AccountingCustomerId = null);

public sealed record IntegrationCustomerOrderCommand(
    string EventId, int ShopId, string TenantId, long ConnectionId,
    string ExternalOrderId, IReadOnlyCollection<IntegrationOrderLineCommand> Lines,
    decimal TotalAmount, SalePaymentStatus PaymentStatus);

public sealed record IntegrationOrderCancellationCommand(
    string EventId, int ShopId, string TenantId, long ConnectionId,
    string ExternalOrderId, string Reason);

public sealed record IntegrationParcelStatusCommand(
    string EventId, int ShopId, string TenantId, long ConnectionId,
    string ExternalOrderId, string ExternalParcelId, string Status, string? TrackingCode);

public sealed record IntegrationExternalProductChangedCommand(
    string EventId, int ShopId, string TenantId, long ConnectionId,
    int HyperProductId, string ExternalProductId, string? ExternalVariantId,
    string? Sku, string Title, decimal? Price, decimal? Inventory, long SourceVersion);

public enum BusinessCommandStatus : byte
{
    Applied = 1,
    Duplicate = 2,
    PendingDependency = 3,
    Rejected = 4
}

public sealed record BusinessCommandResult(
    BusinessCommandStatus Status, string? InternalReference = null,
    string? ErrorCode = null);
