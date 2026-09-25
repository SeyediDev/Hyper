using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Route("api/integrations/v1/accounting/events")]
public sealed class IntegrationAccountingEventController(IIntegrationAccountingEventIngress ingress) : ControllerBase
{
    [HttpPost("inventory-changed")]
    public async Task<IActionResult> InventoryChanged([FromBody] AccountingInventoryChangedRequest request,
        CancellationToken cancellationToken)
    {
        var result = await ingress.ReceiveInventoryChangedAsync(request, cancellationToken);
        return result is null ? BadRequest(new { Error = "MappingOrScopeNotFound" }) : Accepted(result);
    }
}
