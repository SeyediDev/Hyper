using System.Security.Claims;
using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.Integration.Api;

[ApiController, Authorize]
[Route("api/integrations/v1/connections/{connectionId:long}/product-readiness")]
public sealed class IntegrationProductReadinessController(IIntegrationProductReadinessApi readiness,
    IIntegrationScopeAuthorization authorization) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Board(long connectionId, [FromHeader(Name="X-Shop-Id")] int shopId,
        [FromHeader(Name="X-Tenant-Id")] string tenantId, CancellationToken ct, int skip = 0, int take = 25)
    {
        if (!await authorization.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        if (skip < 0 || take is < 1 or > 100) return BadRequest();
        var result = await readiness.BoardAsync(new(shopId, tenantId, connectionId), skip, take, ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("policy/{direction}")]
    public async Task<IActionResult> Policy(long connectionId, ProductTransferDirection direction,
        [FromHeader(Name="X-Shop-Id")] int shopId, [FromHeader(Name="X-Tenant-Id")] string tenantId, CancellationToken ct)
    {
        if (!await authorization.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        if (!Enum.IsDefined(direction)) return BadRequest();
        var result = await readiness.PolicyAsync(new(shopId, tenantId, connectionId), direction, ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpPut("policy/{direction}")]
    public async Task<IActionResult> SetPolicy(long connectionId, ProductTransferDirection direction,
        [FromHeader(Name="X-Shop-Id")] int shopId, [FromHeader(Name="X-Tenant-Id")] string tenantId,
        [FromBody] ProductPreparationPolicy policy, CancellationToken ct)
    {
        if (!await authorization.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        if (!Enum.IsDefined(direction) || !Enum.IsDefined(policy.Mode)) return BadRequest();
        return await readiness.SetPolicyAsync(new(shopId, tenantId, connectionId), direction, policy, Actor(), ct) ? NoContent() : NotFound();
    }
    [HttpPost]
    public async Task<IActionResult> Prepare(long connectionId, [FromHeader(Name="X-Shop-Id")] int shopId,
        [FromHeader(Name="X-Tenant-Id")] string tenantId, [FromBody] ProductPreparationCommand command, CancellationToken ct)
    {
        if (!await authorization.CanAccessAsync(User, shopId, tenantId, ct)) return Forbid();
        try
        {
            var result = await readiness.PrepareAsync(new(shopId, tenantId, connectionId), command.Input, command.ExpectedRevision, Actor(), ct);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException) { return BadRequest(new { error = "InvalidPreparationInput" }); }
        catch (InvalidOperationException) { return Conflict(new { error = "PreparationConflict" }); }
    }
    private string Actor() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "authenticated-api";
}
public sealed record ProductPreparationCommand(ProductPreparationInput Input, int ExpectedRevision = 0);
