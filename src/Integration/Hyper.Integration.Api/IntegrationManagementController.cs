using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController]
[Authorize]
[Route("api/integrations/v1/connections")]
public sealed class IntegrationManagementController(IIntegrationManagementApi management,
    IIntegrationScopeAuthorization scope) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<IntegrationConnectionSummary>>> List(
        [FromQuery] int shopId, [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        CancellationToken cancellationToken)
    {
        if (shopId <= 0 || string.IsNullOrWhiteSpace(tenantId)) return BadRequest("InvalidScope");
        if (!await scope.CanAccessAsync(User, shopId, tenantId, cancellationToken)) return Forbid();
        return Ok(await management.ListConnectionsAsync(new(shopId, tenantId), cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IntegrationConnectionCreateRequest request,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessAsync(User, request.ShopId, request.TenantId, cancellationToken)) return Forbid();
        var created = await management.CreateConnectionAsync(request, cancellationToken);
        return created is null ? BadRequest(new { error = "InvalidConnection" })
            : CreatedAtAction(nameof(List), new { shopId = request.ShopId }, created);
    }

    [HttpPost("{connectionId:long}/enable")]
    public Task<IActionResult> Enable(long connectionId, [FromHeader(Name = "X-Shop-Id")] int shopId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId, CancellationToken cancellationToken) =>
        SetEnabled(connectionId, shopId, tenantId, true, cancellationToken);

    [HttpPost("{connectionId:long}/disable")]
    public Task<IActionResult> Disable(long connectionId, [FromHeader(Name = "X-Shop-Id")] int shopId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId, CancellationToken cancellationToken) =>
        SetEnabled(connectionId, shopId, tenantId, false, cancellationToken);

    [HttpPost("{connectionId:long}/webhooks/{inboxId:long}/replay")]
    public async Task<IActionResult> Replay(long connectionId, long inboxId,
        [FromHeader(Name = "X-Shop-Id")] int shopId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessAsync(User, shopId, tenantId, cancellationToken)) return Forbid();
        var result = await management.ReplayWebhookAsync(new(shopId, tenantId, connectionId), inboxId, cancellationToken);
        return result is null ? NotFound(new { error = "WebhookNotFound" }) : Accepted(result);
    }

    private async Task<IActionResult> SetEnabled(long connectionId, int shopId, string tenantId, bool enabled,
        CancellationToken cancellationToken)
    {
        if (shopId <= 0 || string.IsNullOrWhiteSpace(tenantId)) return BadRequest("InvalidScope");
        if (!await scope.CanAccessAsync(User, shopId, tenantId, cancellationToken)) return Forbid();
        var changed = await management.SetConnectionEnabledAsync(new(shopId, tenantId, connectionId), enabled, cancellationToken);
        return changed ? Ok(new { connectionId, enabled }) : NotFound(new { error = "ConnectionNotFound" });
    }
}
