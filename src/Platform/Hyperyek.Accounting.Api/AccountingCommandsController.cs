using Hyperyek.Accounting.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyperyek.Accounting.Api;

/// <summary>
/// Public boundary for platform-owned accounting commands.
/// Persistence and application handlers are intentionally supplied by the
/// Hyperyek platform host; this assembly contains no database reference.
/// </summary>
[ApiController]
[Authorize]
[Route("api/hyperyek/v1/accounting")]
public sealed class AccountingCommandsController(IAccountingCommandHandler handler) : ControllerBase
{
    [HttpPost("counterparties")]
    public Task<IActionResult> Counterparty([FromBody] CounterpartyCommand command, CancellationToken ct) =>
        Execute(handler.ApplyCounterpartyAsync(command, ct));

    [HttpPost("vendor-orders")]
    public Task<IActionResult> VendorOrder([FromBody] VendorOrderCommand command, CancellationToken ct) =>
        Execute(handler.ApplyVendorOrderAsync(command, ct));

    [HttpPost("customer-orders")]
    public Task<IActionResult> CustomerOrder([FromBody] CustomerOrderCommand command, CancellationToken ct) =>
        Execute(handler.ApplyCustomerOrderAsync(command, ct));

    [HttpPost("orders/cancel")]
    public Task<IActionResult> CancelOrder([FromBody] CancelOrderCommand command, CancellationToken ct) =>
        Execute(handler.CancelOrderAsync(command, ct));

    [HttpPost("parcels/status")]
    public Task<IActionResult> ParcelStatus([FromBody] ParcelStatusCommand command, CancellationToken ct) =>
        Execute(handler.ApplyParcelStatusAsync(command, ct));

    [HttpPost("products/external-changed")]
    public Task<IActionResult> ExternalProductChanged([FromBody] ExternalProductChangedCommand command, CancellationToken ct) =>
        Execute(handler.ApplyExternalProductChangedAsync(command, ct));

    private async Task<IActionResult> Execute(Task<AccountingCommandResult> operation)
    {
        var result = await operation;
        return result.Status switch
        {
            AccountingCommandStatus.Applied => Ok(result),
            AccountingCommandStatus.Duplicate => Conflict(result),
            AccountingCommandStatus.PendingDependency => Accepted(result),
            _ when result.ErrorCode == "AccountingCommandHandlerNotRegistered" =>
                StatusCode(StatusCodes.Status501NotImplemented, result),
            _ => UnprocessableEntity(result)
        };
    }
}
