using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Authorize]
[Route("api/integrations/v1/dashboard")]
public sealed class IntegrationDashboardController(IIntegrationDashboardApi dashboard,
    IIntegrationScopeAuthorization scope) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IntegrationDashboardResponse>> Get(
        [FromQuery] int shopId, [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        CancellationToken cancellationToken)
    {
        if (shopId <= 0 || string.IsNullOrWhiteSpace(tenantId)) return BadRequest("InvalidScope");
        if (!await scope.CanAccessAsync(User, shopId, tenantId, cancellationToken)) return Forbid();
        return Ok(await dashboard.GetDashboardAsync(shopId, tenantId, cancellationToken));
    }
}
