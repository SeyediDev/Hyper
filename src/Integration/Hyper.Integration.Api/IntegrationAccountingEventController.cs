using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Authorize]
[Route("api/integrations/v1/accounting/events")]
public sealed class IntegrationAccountingEventController(IIntegrationAccountingEventIngress ingress,
    IIntegrationScopeAuthorization scope) : ControllerBase
{
    [HttpPost("inventory-changed")]
    public async Task<IActionResult> InventoryChanged([FromBody] AccountingInventoryChangedRequest request,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessAsync(User, request.ShopId, request.TenantId, cancellationToken)) return Forbid();
        var result = await ingress.ReceiveInventoryChangedAsync(request, cancellationToken);
        return result is null ? BadRequest(new { Error = "MappingOrScopeNotFound" }) : Accepted(result);
    }
}
