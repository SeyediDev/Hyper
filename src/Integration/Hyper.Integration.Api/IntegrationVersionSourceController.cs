using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Authorize]
[Route("api/integrations/v1/connections/{connectionId:long}/mappings/{mappingId:long}/version-source")]
public sealed class IntegrationVersionSourceController(IIntegrationVersionSourceApi api, IIntegrationScopeAuthorization authorization) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Read(long connectionId, long mappingId,
        [FromHeader(Name = "X-Shop-Id")] int shopId, [FromHeader(Name = "X-Tenant-Id")] string tenantId, CancellationToken ct)
    {
        if (!await authorization.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        var result = await api.ReadAsync(new(shopId, tenantId, connectionId), mappingId, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Change(long connectionId, long mappingId,
        [FromHeader(Name = "X-Shop-Id")] int shopId, [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromBody] IntegrationVersionSourceChange change, CancellationToken ct)
    {
        if (!await authorization.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        var result = await api.ChangeAsync(new(shopId, tenantId, connectionId), mappingId, change, ct);
        return result is null ? NotFound() : result.Status switch
        {
            "Conflict" => Conflict(result),
            "Rejected" => BadRequest(result),
            _ => Ok(result)
        };
    }
}
