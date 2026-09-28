using System.Security.Claims;
using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController, Authorize]
[Route("api/integrations/v1/connections/{connectionId:long}/parcels/commands")]
public sealed class IntegrationParcelController(IIntegrationParcelApi parcels, IIntegrationScopeAuthorization authorization) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Start(long connectionId, [FromHeader(Name="X-Shop-Id")] int shopId,
        [FromHeader(Name="X-Tenant-Id")] string tenantId, [FromBody] ParcelCommandRequest request, CancellationToken ct)
    {
        if (!await authorization.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        try
        {
            var result = await parcels.StartAsync(new(shopId, tenantId, connectionId), request,
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "authenticated-api", ct);
            return result is null ? NotFound() : Accepted(result);
        }
        catch (ArgumentException) { return BadRequest(new { error = "InvalidParcelCommand" }); }
        catch (InvalidOperationException) { return Conflict(new { error = "ParcelCommandConflict" }); }
    }
    [HttpGet("{requestId:guid}")]
    public async Task<IActionResult> Read(long connectionId, Guid requestId, [FromHeader(Name="X-Shop-Id")] int shopId,
        [FromHeader(Name="X-Tenant-Id")] string tenantId, CancellationToken ct)
    {
        if (!await authorization.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        var result = await parcels.ReadAsync(new(shopId, tenantId, connectionId), requestId, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
