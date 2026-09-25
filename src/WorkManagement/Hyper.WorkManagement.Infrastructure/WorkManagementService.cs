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
        var key = $"CHAT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..30];
        var item = new WorkItem { Key = key, Title = request.SuggestedTitle ?? request.Message[..Math.Min(120, request.Message.Length)], Domain = request.Domain ?? "Untriaged", Description = request.Message, Status = WorkItemStatus.Backlog, Priority = WorkItemPriority.Normal, ChatId = request.ChatId };
        db.WorkItems.Add(item); await db.SaveChangesAsync(ct); intake.WorkItemId = item.Id; await db.SaveChangesAsync(ct);
        return new(intake.Id, item.Id, "Created");
    }
    private async Task<IReadOnlyList<WorkRoleSummary>> RolesQuery(CancellationToken ct) => await db.WorkRoles.AsNoTracking().Select(r => new WorkRoleSummary(r.Key, r.Name, r.Scope, !db.WorkItems.Any(w => w.OwnerRole == r.Key && w.Status == WorkItemStatus.InProgress), db.WorkItems.Where(w => w.OwnerRole == r.Key && w.Status == WorkItemStatus.InProgress).Select(w => (long?)w.Id).FirstOrDefault())).ToListAsync(ct);
    private static WorkItemSummary ToSummary(WorkItem x) => new(x.Id, x.Key, x.Title, x.Domain, x.Status, x.Priority, x.OwnerRole, x.OwnerAgent, x.Branch, x.ChatId, x.UpdatedAtUtc);
}
