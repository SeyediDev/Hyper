using Hyper.WorkManagement.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.AdminPanel.Web.Controllers;

[Authorize]
[Route("api/work-management/v1")]
public sealed class WorkManagementController(IWorkManagementApi api) : ControllerBase
{
    [HttpGet("board")]
    public Task<WorkBoardResponse> Board([FromQuery] string? domain, [FromQuery] string? project, CancellationToken ct) => api.GetBoardAsync(domain, project, ct);
    [HttpGet("roles")]
    public Task<IReadOnlyList<WorkRoleSummary>> Roles(CancellationToken ct) => api.GetRolesAsync(ct);
    [HttpPost("items/{id:long}/claim")]
    public async Task<IActionResult> Claim(long id, [FromBody] ClaimWorkItemRequest request, CancellationToken ct) => (await api.ClaimAsync(id, request, ct)) is { } item ? Ok(item) : Conflict(new { error = "RoleBusyOrItemUnavailable" });
    [HttpPost("items/{id:long}/logs")]
    public async Task<IActionResult> Log(long id, [FromBody] WorkLogRequest request, CancellationToken ct) => (await api.AddLogAsync(id, request, ct)) is { } item ? Ok(item) : NotFound();
    [HttpPost("chat-intake")]
    public Task<ChatIntakeResponse> Intake([FromBody] ChatIntakeRequest request, CancellationToken ct) => api.IntakeChatAsync(request, ct);
    [HttpPost("items")]
    public async Task<IActionResult> Create([FromBody] CreateWorkItemRequest request, CancellationToken ct) => (await api.CreateAsync(request, ct)) is { } item ? Ok(item) : Conflict(new { error = "ProjectOrKeyUnavailable" });
    [HttpGet("items/{id:long}")]
    public async Task<IActionResult> Details(long id, CancellationToken ct) => (await api.GetDetailsAsync(id, ct)) is { } details ? Ok(details) : NotFound();
    [HttpPost("items/{id:long}/status")]
    public async Task<IActionResult> Status(long id, [FromBody] ChangeWorkItemStatusRequest request, CancellationToken ct) => (await api.ChangeStatusAsync(id, request, ct)) is { } item ? Ok(item) : Conflict(new { error = "InvalidStatusTransition" });
    [HttpPost("items/{id:long}/commits")]
    public async Task<IActionResult> Commit(long id, [FromBody] CommitEvidenceRequest request, CancellationToken ct) => await api.AddCommitAsync(id, request, ct) ? Ok() : NotFound();
    [HttpPost("items/{id:long}/tests")]
    public async Task<IActionResult> Test(long id, [FromBody] TestEvidenceRequest request, CancellationToken ct) => await api.AddTestEvidenceAsync(id, request, ct) ? Ok() : NotFound();
    [HttpPost("items/{id:long}/dependencies")]
    public async Task<IActionResult> Dependency(long id, [FromBody] DependencyRequest request, CancellationToken ct) => await api.AddDependencyAsync(id, request, ct) ? Ok() : Conflict(new { error = "InvalidDependency" });
    [HttpPost("items/{id:long}/time/start")]
    public async Task<IActionResult> StartTime(long id, [FromBody] TimeTrackingRequest request, CancellationToken ct) => (await api.StartTrackingAsync(id, request, ct)) is { } item ? Ok(item) : NotFound();
    [HttpPost("items/{id:long}/time/stop")]
    public async Task<IActionResult> StopTime(long id, [FromBody] TimeTrackingRequest request, CancellationToken ct) => (await api.StopTrackingAsync(id, request, ct)) is { } item ? Ok(item) : NotFound();
}
