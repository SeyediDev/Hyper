using Hyper.WorkManagement.Contracts;
using Hyper.WorkManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hyper.WorkManagement.Infrastructure;

public sealed class WorkManagementService(WorkManagementContext db) : IWorkManagementApi
{
    public async Task<WorkBoardResponse> GetBoardAsync(string? domain, CancellationToken ct = default)
    {
        var items = await db.WorkItems.AsNoTracking().Where(x => domain == null || x.Domain == domain)
            .OrderBy(x => x.Status).ThenByDescending(x => x.Priority).ThenBy(x => x.Id)
            .Select(x => ToSummary(x)).ToListAsync(ct);
        var roles = await RolesQuery(ct);
        return new(items, roles);
    }
    public async Task<IReadOnlyList<WorkRoleSummary>> GetRolesAsync(CancellationToken ct = default) => await RolesQuery(ct);
    public async Task<WorkItemSummary?> ClaimAsync(long id, ClaimWorkItemRequest request, CancellationToken ct = default)
    {
        var role = await db.WorkRoles.SingleOrDefaultAsync(x => x.Key == request.RoleKey && x.IsEnabled, ct);
        var item = await db.WorkItems.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (role is null || item is null || item.Status is WorkItemStatus.Done or WorkItemStatus.Cancelled) return null;
        var busy = await db.WorkItems.AnyAsync(x => x.OwnerRole == role.Key && x.Status == WorkItemStatus.InProgress && x.Id != id, ct);
        if (busy) return null;
        item.OwnerRole = role.Key; item.OwnerAgent = request.AgentId; item.ChatId = request.ChatId;
        item.Branch = request.Branch; item.Status = WorkItemStatus.InProgress; item.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return ToSummary(item);
    }
    public async Task<WorkItemSummary?> AddLogAsync(long id, WorkLogRequest request, CancellationToken ct = default)
    {
        var item = await db.WorkItems.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null || string.IsNullOrWhiteSpace(request.Message)) return null;
        db.WorkItemLogs.Add(new WorkItemLog { WorkItemId = id, Author = request.Author, Message = request.Message, ChatId = request.ChatId });
        item.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct); return ToSummary(item);
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
        var item = new WorkItem { ProjectId = project.Id, Key = request.Key.Trim(), Title = request.Title.Trim(), Domain = request.Domain.Trim(), Priority = request.Priority, Description = request.Description };
        db.WorkItems.Add(item); await db.SaveChangesAsync(ct); return ToSummary(item);
    }
    public async Task<WorkItemSummary?> ChangeStatusAsync(long id, ChangeWorkItemStatusRequest request, CancellationToken ct = default)
    {
        var item = await db.WorkItems.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return null;
        if (request.Status == WorkItemStatus.InProgress && item.OwnerRole is null) return null;
        item.Status = request.Status; item.UpdatedAtUtc = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(request.Message)) db.WorkItemLogs.Add(new WorkItemLog { WorkItemId = id, Author = request.Author, Message = request.Message });
        await db.SaveChangesAsync(ct); return ToSummary(item);
    }
    public async Task<WorkItemDetails?> GetDetailsAsync(long id, CancellationToken ct = default)
    {
        var item = await db.WorkItems.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return null;
        var logs = await db.WorkItemLogs.AsNoTracking().Where(x => x.WorkItemId == id).OrderBy(x => x.CreatedAtUtc).Select(x => new WorkLogView(x.Author, x.Message, x.ChatId, x.CreatedAtUtc)).ToListAsync(ct);
        var commits = await db.WorkItemCommits.AsNoTracking().Where(x => x.WorkItemId == id).OrderBy(x => x.CreatedAtUtc).Select(x => new CommitEvidenceView(x.Sha, x.Message, x.CreatedAtUtc)).ToListAsync(ct);
        var tests = await db.WorkItemTestEvidence.AsNoTracking().Where(x => x.WorkItemId == id).OrderBy(x => x.CreatedAtUtc).Select(x => new TestEvidenceView(x.TestName, x.Result, x.Details, x.CreatedAtUtc)).ToListAsync(ct);
        var dependencies = await db.WorkItemDependencies.AsNoTracking().Where(x => x.WorkItemId == id).Select(x => x.DependsOnWorkItemId).ToListAsync(ct);
        return new(ToSummary(item), logs, commits, tests, dependencies);
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
    private async Task<IReadOnlyList<WorkRoleSummary>> RolesQuery(CancellationToken ct) => await db.WorkRoles.AsNoTracking().Select(r => new WorkRoleSummary(r.Key, r.Name, r.Scope, !db.WorkItems.Any(w => w.OwnerRole == r.Key && w.Status == WorkItemStatus.InProgress), db.WorkItems.Where(w => w.OwnerRole == r.Key && w.Status == WorkItemStatus.InProgress).Select(w => (long?)w.Id).FirstOrDefault())).ToListAsync(ct);
    private static WorkItemSummary ToSummary(WorkItem x) => new(x.Id, x.Key, x.Title, x.Domain, x.Status, x.Priority, x.OwnerRole, x.OwnerAgent, x.Branch, x.ChatId, x.UpdatedAtUtc);
}
