using Hyper.WorkManagement.Contracts;
using Hyper.WorkManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hyper.WorkManagement.Infrastructure;

public sealed class WorkManagementService(WorkManagementContext db, IAgentOrchestrationService? orchestrator = null) : IWorkManagementApi
{
    public async Task<WorkBoardResponse> GetBoardAsync(string? domain, string? project = null, string? role = null, bool includeArchived = false, CancellationToken ct = default)
    {
        var all = db.WorkItems.AsNoTracking().Include(x => x.Project).Where(x => (domain == null || x.Domain == domain) && (project == null || x.Project!.Key == project) && (role == null || x.OwnerRole == role) && (includeArchived || !x.IsArchived));
        var items = await all
            .OrderBy(x => x.Status).ThenByDescending(x => x.Priority).ThenBy(x => x.Id)
            .ToListAsync(ct);
        var childCounts = await db.WorkItems.Where(x => !x.IsArchived || includeArchived).GroupBy(x => x.ParentWorkItemId??0).Select(g => new { ParentId = g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.ParentId, x => x.Count, ct);
        var roles = await RolesQuery(ct);
        var projects = await db.WorkProjects.AsNoTracking().Where(x => x.IsEnabled).OrderBy(x => x.Key).Select(x => x.Key).ToListAsync(ct);
        var domains = await db.WorkItems.AsNoTracking().Where(x => !x.IsArchived || includeArchived).Select(x => x.Domain).Distinct().OrderBy(x => x).ToListAsync(ct);
        var summaries = items.Select(x => ToSummary(x, childCounts.GetValueOrDefault(x.Id))).ToList();
        return new(summaries, roles, projects, domains, Metrics(summaries));
    }
    public async Task<IReadOnlyList<WorkRoleSummary>> GetRolesAsync(CancellationToken ct = default) => await RolesQuery(ct);
    public async Task<WorkItemSummary?> ClaimAsync(long id, ClaimWorkItemRequest request, CancellationToken ct = default)
    {
        var role = await db.WorkRoles.SingleOrDefaultAsync(x => x.Key == request.RoleKey && x.IsEnabled, ct);
        var item = await db.WorkItems.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (role is null || item is null || item.Status is WorkItemStatus.Done or WorkItemStatus.Cancelled) return null;
        var busy = await db.WorkItems.AnyAsync(x => x.OwnerRole == role.Key && x.Status == WorkItemStatus.InProgress && x.Id != id, ct);
        if (busy) return null;
        item.OwnerRole = role.Key; item.OwnerAgent = request.AgentId; item.ChatId = request.ChatId;
        item.Branch = request.Branch; item.Status = WorkItemStatus.InProgress; item.UpdatedAtUtc = DateTime.UtcNow;
        await StartTrackingInternal(item, null, ct); await db.SaveChangesAsync(ct); return await SummaryAsync(item, ct);
    }
    public async Task<WorkItemSummary?> AddLogAsync(long id, WorkLogRequest request, CancellationToken ct = default)
    {
        var item = await db.WorkItems.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null || string.IsNullOrWhiteSpace(request.Message)) return null;
        db.WorkItemLogs.Add(new WorkItemLog { WorkItemId = id, Author = request.Author, Message = request.Message, ChatId = request.ChatId });
        item.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); return await SummaryAsync(item, ct);
    }
    public async Task<ChatIntakeResponse> IntakeChatAsync(ChatIntakeRequest request, CancellationToken ct = default)
    {
        var intake = new ChatWorkIntake { ChatId = request.ChatId, Author = request.Author, Message = request.Message, Status = 1 };
        db.ChatWorkIntakes.Add(intake);
        var project = await db.WorkProjects.FirstOrDefaultAsync(x => x.Key == "HYPER", ct)
            ?? new WorkProject { Key = "HYPER", Name = "Hyper", IsEnabled = true };
        if (project.Id == 0) { db.WorkProjects.Add(project); await db.SaveChangesAsync(ct); }
        var key = $"CHAT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..30];
        var item = new WorkItem { ProjectId = project.Id, Key = key, Title = request.SuggestedTitle ?? request.Message[..Math.Min(120, request.Message.Length)], Domain = request.Domain ?? "Untriaged", Description = request.Message, Status = WorkItemStatus.Backlog, Priority = WorkItemPriority.Normal, ChatId = request.ChatId };
        db.WorkItems.Add(item); await db.SaveChangesAsync(ct); intake.WorkItemId = item.Id; await db.SaveChangesAsync(ct);
        return new(intake.Id, item.Id, "Created");
    }
    public async Task<WorkItemSummary?> CreateAsync(CreateWorkItemRequest request, CancellationToken ct = default)
    {
        var project = await db.WorkProjects.SingleOrDefaultAsync(x => x.Key == request.ProjectKey && x.IsEnabled, ct);
        if (project is null || string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.Title)) return null;
        if (await db.WorkItems.AnyAsync(x => x.ProjectId == project.Id && x.Key == request.Key, ct)) return null;
        if (request.ParentWorkItemId is { } parentId && !await db.WorkItems.AnyAsync(x => x.Id == parentId && x.ProjectId == project.Id, ct)) return null;
        var item = new WorkItem { ProjectId = project.Id, ParentWorkItemId = request.ParentWorkItemId, Key = request.Key.Trim(), Title = request.Title.Trim(), Domain = request.Domain.Trim(), Priority = request.Priority, Description = request.Description, EstimatedSeconds = request.EstimatedSeconds is > 0 ? request.EstimatedSeconds : null };
        db.WorkItems.Add(item); await db.SaveChangesAsync(ct); item.Project = project; return await SummaryAsync(item, ct);
    }
    public async Task<WorkItemSummary?> ChangeStatusAsync(long id, ChangeWorkItemStatusRequest request, CancellationToken ct = default)
    {
        var item = await db.WorkItems.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return null;
        var previousStatus = item.Status;
        if (request.Status == WorkItemStatus.InProgress && item.OwnerRole is null) return null;
        if (item.Status != WorkItemStatus.InProgress && request.Status == WorkItemStatus.InProgress) await StartTrackingInternal(item, request.Message, ct);
        if (item.Status == WorkItemStatus.InProgress && request.Status != WorkItemStatus.InProgress) await StopTrackingInternal(item, request.Message, ct);
        item.Status = request.Status; item.CompletedAtUtc = request.Status == WorkItemStatus.Done ? DateTime.UtcNow : null; item.UpdatedAtUtc = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(request.Message)) db.WorkItemLogs.Add(new WorkItemLog { WorkItemId = id, Author = request.Author, Message = request.Message });
        await db.SaveChangesAsync(ct);
        if (orchestrator is not null && previousStatus != request.Status) await orchestrator.OnStatusChangedAsync(id, previousStatus, request.Status, ct);
        return await SummaryAsync(item, ct);
    }
    public async Task<WorkItemDetails?> GetDetailsAsync(long id, CancellationToken ct = default)
    {
        var item = await db.WorkItems.AsNoTracking().Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return null;
        var logs = await db.WorkItemLogs.AsNoTracking().Where(x => x.WorkItemId == id).OrderBy(x => x.CreatedAtUtc).Select(x => new WorkLogView(x.Author, x.Message, x.ChatId, x.CreatedAtUtc)).ToListAsync(ct);
        var commits = await db.WorkItemCommits.AsNoTracking().Where(x => x.WorkItemId == id).OrderBy(x => x.CreatedAtUtc).Select(x => new CommitEvidenceView(x.Sha, x.Message, x.CreatedAtUtc)).ToListAsync(ct);
        var tests = await db.WorkItemTestEvidence.AsNoTracking().Where(x => x.WorkItemId == id).OrderBy(x => x.CreatedAtUtc).Select(x => new TestEvidenceView(x.TestName, x.Result, x.Details, x.CreatedAtUtc)).ToListAsync(ct);
        var dependencies = await db.WorkItemDependencies.AsNoTracking().Where(x => x.WorkItemId == id).Select(x => x.DependsOnWorkItemId).ToListAsync(ct);
        var children = await db.WorkItems.AsNoTracking().Include(x => x.Project).Where(x => x.ParentWorkItemId == id).ToListAsync(ct);
        var timeEntries = await db.WorkItemTimeEntries.AsNoTracking().Where(x => x.WorkItemId == id).OrderBy(x => x.StartedAtUtc).Select(x => new TimeEntryView(x.StartedAtUtc, x.EndedAtUtc, x.DurationSeconds, x.Note)).ToListAsync(ct);
        return new(ToSummary(item, children.Count), logs, commits, tests, dependencies, children.Select(x => ToSummary(x, 0)).ToList(), timeEntries);
    }
    public async Task<bool> AddCommitAsync(long id, CommitEvidenceRequest request, CancellationToken ct = default)
    {
        if (!await db.WorkItems.AnyAsync(x => x.Id == id, ct) || string.IsNullOrWhiteSpace(request.Sha)) return false;
        db.WorkItemCommits.Add(new WorkItemCommit { WorkItemId = id, Sha = request.Sha.Trim(), Message = request.Message });
        await db.SaveChangesAsync(ct); return true;
    }
    public async Task<bool> AddTestEvidenceAsync(long id, TestEvidenceRequest request, CancellationToken ct = default)
    {
        if (!await db.WorkItems.AnyAsync(x => x.Id == id, ct) || string.IsNullOrWhiteSpace(request.TestName)) return false;
        db.WorkItemTestEvidence.Add(new WorkItemTestEvidence { WorkItemId = id, TestName = request.TestName, Result = request.Result, Details = request.Details });
        await db.SaveChangesAsync(ct); return true;
    }
    public async Task<bool> AddDependencyAsync(long id, DependencyRequest request, CancellationToken ct = default)
    {
        if (id == request.DependsOnWorkItemId || !await db.WorkItems.AnyAsync(x => x.Id == id, ct) || !await db.WorkItems.AnyAsync(x => x.Id == request.DependsOnWorkItemId, ct)) return false;
        if (await db.WorkItemDependencies.AnyAsync(x => x.WorkItemId == id && x.DependsOnWorkItemId == request.DependsOnWorkItemId, ct)) return true;
        db.WorkItemDependencies.Add(new WorkItemDependency { WorkItemId = id, DependsOnWorkItemId = request.DependsOnWorkItemId }); await db.SaveChangesAsync(ct); return true;
    }
    public async Task<WorkItemSummary?> StartTrackingAsync(long id, TimeTrackingRequest request, CancellationToken ct = default) { var item = await db.WorkItems.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == id, ct); if (item is null || item.Status != WorkItemStatus.InProgress || item.OwnerRole is null) return null; await StartTrackingInternal(item, request.Note, ct); await db.SaveChangesAsync(ct); return await SummaryAsync(item, ct); }
    public async Task<WorkItemSummary?> StopTrackingAsync(long id, TimeTrackingRequest request, CancellationToken ct = default) { var item = await db.WorkItems.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == id, ct); if (item is null) return null; await StopTrackingInternal(item, request.Note, ct); await db.SaveChangesAsync(ct); return await SummaryAsync(item, ct); }
    public async Task<bool> ArchiveAsync(long id, CancellationToken ct = default) { var item = await db.WorkItems.SingleOrDefaultAsync(x => x.Id == id, ct); if (item is null || item.Status is not (WorkItemStatus.Blocked or WorkItemStatus.Done)) return false; await StopTrackingInternal(item, null, ct); item.IsArchived = true; item.ArchivedAtUtc = DateTime.UtcNow; item.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); return true; }
    public async Task<bool> UnarchiveAsync(long id, CancellationToken ct = default) { var item = await db.WorkItems.SingleOrDefaultAsync(x => x.Id == id, ct); if (item is null || !item.IsArchived) return false; item.IsArchived = false; item.ArchivedAtUtc = null; item.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); return true; }
    public async Task<WorkItemSummary?> SetEstimateAsync(long id, EstimateWorkItemRequest request, CancellationToken ct = default) { var item = await db.WorkItems.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == id, ct); if (item is null || request.EstimatedSeconds is < 0) return null; item.EstimatedSeconds = request.EstimatedSeconds is > 0 ? request.EstimatedSeconds : null; item.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); return await SummaryAsync(item, ct); }
    public Task<AgentOrchestrationSnapshot> GetOrchestrationAsync(CancellationToken ct = default) => orchestrator?.GetAsync(ct) ?? throw new InvalidOperationException("AgentOrchestrationUnavailable");
    public Task<AgentProfileView?> CreateAgentProfileAsync(CreateAgentProfileRequest request, CancellationToken ct = default) => orchestrator?.CreateProfileAsync(request, ct) ?? throw new InvalidOperationException("AgentOrchestrationUnavailable");
    public Task<WorkflowTransitionView?> CreateWorkflowTransitionAsync(CreateWorkflowTransitionRequest request, CancellationToken ct = default) => orchestrator?.CreateTransitionAsync(request, ct) ?? throw new InvalidOperationException("AgentOrchestrationUnavailable");
    public Task<IReadOnlyList<AgentRunView>> GetRunsAsync(long id, CancellationToken ct = default) => orchestrator?.GetRunsAsync(id, ct) ?? throw new InvalidOperationException("AgentOrchestrationUnavailable");
    public Task<AgentRunView?> DispatchNextAsync(long id, CancellationToken ct = default) => orchestrator?.DispatchNextAsync(id, ct) ?? throw new InvalidOperationException("AgentOrchestrationUnavailable");
    private async Task StartTrackingInternal(WorkItem item, string? note, CancellationToken ct) { if (await db.WorkItemTimeEntries.AnyAsync(x => x.WorkItemId == item.Id && x.EndedAtUtc == null, ct)) return; var now = DateTime.UtcNow; item.StartedAtUtc = now; db.WorkItemTimeEntries.Add(new WorkItemTimeEntry { WorkItemId = item.Id, StartedAtUtc = now, Note = note }); }
    private async Task StopTrackingInternal(WorkItem item, string? note, CancellationToken ct) { var entry = await db.WorkItemTimeEntries.Where(x => x.WorkItemId == item.Id && x.EndedAtUtc == null).OrderByDescending(x => x.StartedAtUtc).FirstOrDefaultAsync(ct); if (entry is null) return; entry.EndedAtUtc = DateTime.UtcNow; entry.DurationSeconds = Math.Max(0, (long)(entry.EndedAtUtc.Value - entry.StartedAtUtc).TotalSeconds); if (!string.IsNullOrWhiteSpace(note)) entry.Note = note; item.AccumulatedSeconds += entry.DurationSeconds; item.StartedAtUtc = null; }
    private async Task<WorkItemSummary> SummaryAsync(WorkItem item, CancellationToken ct) => ToSummary(item, await db.WorkItems.CountAsync(x => x.ParentWorkItemId == item.Id, ct));
    private async Task<IReadOnlyList<WorkRoleSummary>> RolesQuery(CancellationToken ct) => await db.WorkRoles.AsNoTracking().Select(r => new WorkRoleSummary(r.Key, r.Name, r.Scope, !db.WorkItems.Any(w => w.OwnerRole == r.Key && w.Status == WorkItemStatus.InProgress), db.WorkItems.Where(w => w.OwnerRole == r.Key && w.Status == WorkItemStatus.InProgress).Select(w => (long?)w.Id).FirstOrDefault())).ToListAsync(ct);
    private static WorkItemSummary ToSummary(WorkItem x, int childCount) { var live = x.StartedAtUtc is not null && x.Status == WorkItemStatus.InProgress ? (long)(DateTime.UtcNow - x.StartedAtUtc.Value).TotalSeconds : 0; var spent = x.AccumulatedSeconds + live; var progress = x.EstimatedSeconds is > 0 ? (int)Math.Clamp(Math.Round(spent * 100d / x.EstimatedSeconds.Value), 0, 100) : 0; return new(x.Id, x.Project?.Key ?? "", x.ParentWorkItemId, x.Key, x.Title, x.Domain, x.Status, x.Priority, x.OwnerRole, x.OwnerAgent, x.Branch, x.ChatId, x.Description, x.UpdatedAtUtc, spent, x.EstimatedSeconds, progress, live > 0, childCount, x.IsArchived); }
    private static WorkBoardMetrics Metrics(IReadOnlyCollection<WorkItemSummary> items) => new(items.Count, items.Count(x => x.Status == WorkItemStatus.InProgress), items.Count(x => x.Status == WorkItemStatus.Blocked), items.Count(x => x.Status == WorkItemStatus.Done), items.Count(x => x.IsArchived), items.Sum(x => x.TimeSpentSeconds), items.Sum(x => x.EstimatedSeconds ?? 0), items.Where(x => x.EstimatedSeconds is > 0).Select(x => x.ProgressPercent).DefaultIfEmpty(0).Average() is var p ? (int)Math.Round(p) : 0);
}
