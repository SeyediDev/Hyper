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

    public Task<AccountingCommandResult> ApplyCounterpartyAsync(CounterpartyCommand command, CancellationToken ct) => Result(ct);
    public Task<AccountingCommandResult> ApplyVendorOrderAsync(VendorOrderCommand command, CancellationToken ct) => Result(ct);
    public Task<AccountingCommandResult> ApplyCustomerOrderAsync(CustomerOrderCommand command, CancellationToken ct) => Result(ct);
    public Task<AccountingCommandResult> CancelOrderAsync(CancelOrderCommand command, CancellationToken ct) => Result(ct);
    public Task<AccountingCommandResult> ApplyParcelStatusAsync(ParcelStatusCommand command, CancellationToken ct) => Result(ct);

    private static Task<AccountingCommandResult> Result(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(NotRegistered);
    }
}
