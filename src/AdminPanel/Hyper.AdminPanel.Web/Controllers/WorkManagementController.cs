using Hyper.WorkManagement.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.AdminPanel.Web.Controllers;

[Authorize]
[Route("api/work-management/v1")]
public sealed class WorkManagementController(IWorkManagementApi api) : ControllerBase
{
    [HttpGet("board")]
    public Task<WorkBoardResponse> Board([FromQuery] string? domain, CancellationToken ct) => api.GetBoardAsync(domain, ct);
    [HttpGet("roles")]
    public Task<IReadOnlyList<WorkRoleSummary>> Roles(CancellationToken ct) => api.GetRolesAsync(ct);
    [HttpPost("items/{id:long}/claim")]
    public async Task<IActionResult> Claim(long id, [FromBody] ClaimWorkItemRequest request, CancellationToken ct) => (await api.ClaimAsync(id, request, ct)) is { } item ? Ok(item) : Conflict(new { error = "RoleBusyOrItemUnavailable" });
    [HttpPost("items/{id:long}/logs")]
    public async Task<IActionResult> Log(long id, [FromBody] WorkLogRequest request, CancellationToken ct) => (await api.AddLogAsync(id, request, ct)) is { } item ? Ok(item) : NotFound();
    [HttpPost("chat-intake")]
    public Task<ChatIntakeResponse> Intake([FromBody] ChatIntakeRequest request, CancellationToken ct) => api.IntakeChatAsync(request, ct);
}
