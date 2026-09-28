using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Authorize]
[Route("api/integrations/v1/connections/{connectionId:long}/product-drafts")]
public sealed class IntegrationProductDraftController(IIntegrationProductDraftApi drafts, IIntegrationScopeAuthorization scope) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateDraft(long connectionId, [FromBody] IntegrationProductDraftRequest request, CancellationToken ct)
    {
        if (request.ConnectionId != connectionId) return BadRequest(new { error = "ConnectionIdentityConflict" });
        if (!await scope.CanAccessAsync(User, request.ShopId, request.TenantId, ct)) return Forbid();
        try
        {
            var result = await drafts.StartAsync(request, ct);
            return result is null ? NotFound(new { error = "ConnectionNotFound" }) : Accepted(result);
        }
        catch (ArgumentException) { return BadRequest(new { error = "InvalidProductDraft" }); }
        catch (InvalidOperationException) { return Conflict(new { error = "ProductDraftConflictOrUnavailable" }); }
    }

    [HttpGet("{requestId:guid}")]
    public async Task<IActionResult> ReadDraft(long connectionId, Guid requestId,
        [FromHeader(Name = "X-Shop-Id")] int shopId, [FromHeader(Name = "X-Tenant-Id")] string tenantId, CancellationToken ct)
    {
        if (!await scope.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        var result = await drafts.ReadAsync(new(shopId, tenantId, connectionId), requestId, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
