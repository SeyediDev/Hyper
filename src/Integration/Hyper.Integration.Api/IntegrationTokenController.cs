using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Authorize]
[Route("api/integrations/v1/tokens")]
public sealed class IntegrationTokenController(IIntegrationTokenApi tokens,
    IIntegrationScopeAuthorization scope) : ControllerBase
{
    [HttpPost("requests")]
    public async Task<IActionResult> RequestToken([FromBody] IntegrationTokenRequestCommand command,
        CancellationToken ct)
    {
        if (!await scope.CanAccessAsync(User, command.ShopId, command.TenantId, ct)) return Forbid();
        var adminId = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(adminId)) return Unauthorized();
        var result = await tokens.RequestAsync(adminId, command, ct);
        return result is null ? NotFound(new { error = "SimulationNotFound" }) : Accepted(result);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromHeader(Name = "X-Shop-Id")] int shopId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId, CancellationToken ct)
    {
        if (!await scope.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        return Ok(await tokens.ListAsync(shopId, tenantId, ct));
    }

    [HttpDelete("{connectionId:long}")]
    public async Task<IActionResult> Revoke(long connectionId,
        [FromHeader(Name = "X-Shop-Id")] int shopId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId, CancellationToken ct)
    {
        if (!await scope.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        return await tokens.RevokeAsync(shopId, tenantId, connectionId, ct) ? NoContent() : NotFound();
    }
}
