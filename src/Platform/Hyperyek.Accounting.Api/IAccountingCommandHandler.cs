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

public interface IAccountingCommandHandler
{
    Task<AccountingCommandResult> ApplyCounterpartyAsync(CounterpartyCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ApplyVendorOrderAsync(VendorOrderCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ApplyCustomerOrderAsync(CustomerOrderCommand command, CancellationToken ct);
    Task<AccountingCommandResult> CancelOrderAsync(CancelOrderCommand command, CancellationToken ct);
    Task<AccountingCommandResult> ApplyParcelStatusAsync(ParcelStatusCommand command, CancellationToken ct);
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

    private static Task<AccountingCommandResult> Result<T>(T command, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        AccountingCommandValidation.Validate(command);
        return Task.FromResult(NotRegistered);
    }
}

internal static class AccountingCommandValidation
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
            _ => ""
        };
        if (string.IsNullOrWhiteSpace(eventId) || eventId.Length > 128 || eventId.Any(char.IsControl))
            throw new ArgumentException("InvalidAccountingEventId");
    }
}
