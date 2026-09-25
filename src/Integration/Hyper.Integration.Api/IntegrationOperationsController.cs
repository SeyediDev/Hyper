using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Authorize]
[Route("api/integrations/v1/connections/{connectionId:long}")]
public sealed class IntegrationOperationsController(
    IIntegrationMappingApi mappings, IIntegrationSyncApi sync, IIntegrationScopeAuthorization scope) : ControllerBase
{
    [HttpGet("mappings")]
    public async Task<IActionResult> ListMappings(long connectionId,
        [FromHeader(Name = "X-Shop-Id")] int shopId, [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessAsync(User, shopId, tenantId, cancellationToken)) return Forbid();
        return Ok(await mappings.ListProductMappingsAsync(new(shopId, tenantId, connectionId), cancellationToken));
    }

    [HttpPost("mappings")]
    public async Task<IActionResult> CreateMapping(long connectionId,
        [FromBody] IntegrationProductMappingRequest request, CancellationToken cancellationToken)
    {
        if (request.ConnectionId != connectionId) return BadRequest(new { error = "ConnectionIdentityConflict" });
        if (!await scope.CanAccessAsync(User, request.ShopId, request.TenantId, cancellationToken)) return Forbid();
        var result = await mappings.CreateProductMappingAsync(request, cancellationToken);
        return result is null ? Conflict(new { error = "MappingUnavailableOrDuplicate" }) : Ok(result);
    }

    [HttpDelete("mappings/{mappingId:long}")]
    public async Task<IActionResult> DeactivateMapping(long connectionId, long mappingId,
        [FromHeader(Name = "X-Shop-Id")] int shopId, [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessAsync(User, shopId, tenantId, cancellationToken)) return Forbid();
        return await mappings.DeactivateProductMappingAsync(new(shopId, tenantId, connectionId), mappingId, cancellationToken)
            ? NoContent() : NotFound(new { error = "MappingNotFound" });
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync(long connectionId,
        [FromHeader(Name = "X-Shop-Id")] int shopId, [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessAsync(User, shopId, tenantId, cancellationToken)) return Forbid();
        var result = await sync.TriggerAsync(new(shopId, tenantId, connectionId), cancellationToken);
        return result is null ? NotFound(new { error = "ConnectionNotFound" }) : Accepted(result);
    }
}
