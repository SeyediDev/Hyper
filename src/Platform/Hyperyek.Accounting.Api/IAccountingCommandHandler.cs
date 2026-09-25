using Hyperyek.Accounting.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Hyperyek.Accounting.Api;

public static class AccountingApiServiceCollectionExtensions
{
    public static IServiceCollection AddHyperyekAccountingApi(this IServiceCollection services)
    {
        services.AddScoped<IAccountingCommandHandler, UnregisteredAccountingCommandHandler>();
        return services;
    }

    /// <summary>Registers the platform-owned implementation without exposing its persistence to Integration.</summary>
    public static IServiceCollection AddHyperyekAccountingCommandHandler<THandler>(this IServiceCollection services)
        where THandler : class, IAccountingCommandHandler
    {
        services.AddScoped<IAccountingCommandHandler, THandler>();
        return services;
    }
}

internal sealed class UnregisteredAccountingCommandHandler : IAccountingCommandHandler
{
    private static readonly AccountingCommandResult NotRegistered = new(
        AccountingCommandStatus.Rejected, ErrorCode: "AccountingCommandHandlerNotRegistered");

    public Task<AccountingCommandResult> ApplyCounterpartyAsync(CounterpartyCommand command, CancellationToken ct) => Result(command, ct);
    public Task<AccountingCommandResult> ApplyVendorOrderAsync(VendorOrderCommand command, CancellationToken ct) => Result(command, ct);
    public Task<AccountingCommandResult> ApplyCustomerOrderAsync(CustomerOrderCommand command, CancellationToken ct) => Result(command, ct);
    public Task<AccountingCommandResult> CancelOrderAsync(CancelOrderCommand command, CancellationToken ct) => Result(command, ct);
    public Task<AccountingCommandResult> ApplyParcelStatusAsync(ParcelStatusCommand command, CancellationToken ct) => Result(command, ct);
    public Task<AccountingCommandResult> ApplyExternalProductChangedAsync(ExternalProductChangedCommand command, CancellationToken ct) => Result(command, ct);
    public Task<AccountingCommandResult> ValidateCustomerAsync(ValidateCustomerCommand command, CancellationToken ct) => Result(command, ct);
    public Task<AccountingCommandResult> ResolveCustomerAsync(ResolveCustomerCommand command, CancellationToken ct) => Result(command, ct);

    private static Task<AccountingCommandResult> Result<T>(T command, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        AccountingCommandValidation.Validate(command);
        return Task.FromResult(NotRegistered);
    }
}

public static class AccountingCommandValidation
{
    public static void Validate<T>(T command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var scope = command switch
        {
            CounterpartyCommand x => x.Scope,
            VendorOrderCommand x => x.Scope,
            CustomerOrderCommand x => x.Scope,
            CancelOrderCommand x => x.Scope,
            ParcelStatusCommand x => x.Scope,
            ExternalProductChangedCommand x => x.Scope,
            _ => throw new ArgumentException("UnsupportedAccountingCommand")
        };
        if (scope.ShopId <= 0 || string.IsNullOrWhiteSpace(scope.TenantId) || scope.TenantId.Length > 128)
            throw new ArgumentException("InvalidAccountingScope");
        var eventId = command switch
        {
            CounterpartyCommand x => x.EventId,
            VendorOrderCommand x => x.EventId,
            CustomerOrderCommand x => x.EventId,
            CancelOrderCommand x => x.EventId,
            ParcelStatusCommand x => x.EventId,
            ExternalProductChangedCommand x => x.EventId,
            _ => ""
        };
        if (string.IsNullOrWhiteSpace(eventId) || eventId.Length > 128 || eventId.Any(char.IsControl))
            throw new ArgumentException("InvalidAccountingEventId");

        switch (command)
        {
            case VendorOrderCommand vendor:
                ValidateOrder(vendor.ConnectionId, vendor.ExternalOrderId, vendor.ExternalCustomerId,
                    vendor.Lines, vendor.TotalAmount);
                if (vendor.AccountingCustomerId is <= 0) throw new ArgumentException("InvalidAccountingCustomerId");
                if (vendor.PaymentStatus > 4) throw new ArgumentException("InvalidPaymentStatus");
                break;
            case CustomerOrderCommand customer:
                ValidateOrder(customer.ConnectionId, customer.ExternalOrderId, null,
                    customer.Lines, customer.TotalAmount);
                if (customer.PaymentStatus > 4) throw new ArgumentException("InvalidPaymentStatus");
                break;
            case CancelOrderCommand cancel:
                ValidateIdentity(cancel.ConnectionId, cancel.ExternalOrderId, "InvalidExternalOrderId");
                if (string.IsNullOrWhiteSpace(cancel.Reason) || cancel.Reason.Length > 500)
                    throw new ArgumentException("InvalidCancellationReason");
                break;
            case ParcelStatusCommand parcel:
                ValidateIdentity(parcel.ConnectionId, parcel.ExternalOrderId, "InvalidExternalOrderId");
                if (string.IsNullOrWhiteSpace(parcel.ExternalParcelId) || parcel.ExternalParcelId.Length > 128
                    || string.IsNullOrWhiteSpace(parcel.Status) || parcel.Status.Length > 80)
                    throw new ArgumentException("InvalidParcelStatus");
                break;
            case ExternalProductChangedCommand product:
                ValidateIdentity(product.ConnectionId, product.ExternalProductId, "InvalidExternalProductId");
                if (product.HyperProductId <= 0 || product.SourceVersion <= 0
                    || string.IsNullOrWhiteSpace(product.Title) || product.Title.Length > 500
                    || product.Price is < 0 || product.Inventory is < 0)
                    throw new ArgumentException("InvalidExternalProduct");
                break;
        }
    }

    private static void ValidateOrder(long connectionId, string externalOrderId, string? customerId,
        IReadOnlyCollection<OrderLineCommand> lines, decimal totalAmount)
    {
        ValidateIdentity(connectionId, externalOrderId, "InvalidExternalOrderId");
        if (customerId is not null && (string.IsNullOrWhiteSpace(customerId) || customerId.Length > 128))
            throw new ArgumentException("InvalidExternalCustomerId");
        if (lines is null || lines.Count == 0 || lines.Count > 1000 || totalAmount < 0)
            throw new ArgumentException("InvalidOrderLines");
        foreach (var line in lines)
        {
            if (line.HyperProductId <= 0 || line.Quantity <= 0 || line.UnitPrice < 0
                || line.Quantity > 1_000_000 || line.UnitPrice > 1_000_000_000)
                throw new ArgumentException("InvalidOrderLine");
        }
    }

    private static void ValidateIdentity(long connectionId, string value, string error)
    {
        if (connectionId <= 0 || string.IsNullOrWhiteSpace(value) || value.Length > 128 || value.Any(char.IsControl))
            throw new ArgumentException(error);
    }
}
