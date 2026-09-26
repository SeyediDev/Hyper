using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Authorize]
[Route("api/integrations/v1/accounting/events")]
public sealed class IntegrationAccountingProductEventController(IIntegrationAccountingProductEventIngress ingress,
    IIntegrationScopeAuthorization scope) : ControllerBase
{
    [HttpPost("product-changed")]
    public async Task<IActionResult> ProductChanged(AccountingProductChangedRequest request, CancellationToken ct)
    {
        if (!await scope.CanAccessAsync(User, request.ShopId, request.TenantId, ct)) return Forbid();
        var result = await ingress.ReceiveAsync(request, ct);
        return result.Status switch
        {
            "Queued" => Accepted(result),
            "Conflict" => Conflict(result),
            _ => BadRequest(result)
        };
    }
}
