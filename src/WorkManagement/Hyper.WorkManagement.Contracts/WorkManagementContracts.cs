namespace Hyper.WorkManagement.Contracts;

public enum WorkItemStatus : byte { Backlog = 1, Ready = 2, InProgress = 3, Blocked = 4, Review = 5, Done = 6, Cancelled = 7 }
public enum WorkItemPriority : byte { Low = 1, Normal = 2, High = 3, Critical = 4 }
public sealed record WorkRoleSummary(string Key, string Name, string Scope, bool IsAvailable, long? ActiveWorkItemId);
public sealed record WorkItemSummary(long Id, string Key, string Title, string Domain, WorkItemStatus Status, WorkItemPriority Priority,
    string? OwnerRole, string? OwnerAgent, string? Branch, string? ChatId, DateTime UpdatedAtUtc);
public sealed record WorkBoardResponse(IReadOnlyList<WorkItemSummary> Items, IReadOnlyList<WorkRoleSummary> Roles);
public sealed record ClaimWorkItemRequest(string RoleKey, string AgentId, string ChatId, string Branch);
public sealed record WorkLogRequest(string Author, string Message, string? ChatId = null);
public sealed record ChatIntakeRequest(string ChatId, string Author, string Message, string? SuggestedTitle = null, string? Domain = null);
public sealed record ChatIntakeResponse(long IntakeId, long? WorkItemId, string Status);
public sealed record CreateWorkItemRequest(string ProjectKey, string Key, string Title, string Domain, WorkItemPriority Priority = WorkItemPriority.Normal, string? Description = null);
public sealed record ChangeWorkItemStatusRequest(WorkItemStatus Status, string Author, string? Message = null);
public sealed record CommitEvidenceRequest(string Sha, string? Message = null);
public sealed record TestEvidenceRequest(string TestName, string Result, string? Details = null);
public sealed record DependencyRequest(long DependsOnWorkItemId);
public sealed record WorkItemDetails(WorkItemSummary Item, IReadOnlyList<WorkLogView> Logs, IReadOnlyList<CommitEvidenceView> Commits, IReadOnlyList<TestEvidenceView> Tests, IReadOnlyList<long> Dependencies);
public sealed record WorkLogView(string Author, string Message, string? ChatId, DateTime CreatedAtUtc);
public sealed record CommitEvidenceView(string Sha, string? Message, DateTime CreatedAtUtc);
public sealed record TestEvidenceView(string TestName, string Result, string? Details, DateTime CreatedAtUtc);
public interface IWorkManagementApi
{
    Task<WorkBoardResponse> GetBoardAsync(string? domain, CancellationToken ct = default);
    Task<IReadOnlyList<WorkRoleSummary>> GetRolesAsync(CancellationToken ct = default);
    Task<WorkItemSummary?> ClaimAsync(long workItemId, ClaimWorkItemRequest request, CancellationToken ct = default);
    Task<WorkItemSummary?> AddLogAsync(long workItemId, WorkLogRequest request, CancellationToken ct = default);
    Task<ChatIntakeResponse> IntakeChatAsync(ChatIntakeRequest request, CancellationToken ct = default);
    Task<WorkItemSummary?> CreateAsync(CreateWorkItemRequest request, CancellationToken ct = default);
    Task<WorkItemSummary?> ChangeStatusAsync(long workItemId, ChangeWorkItemStatusRequest request, CancellationToken ct = default);
    Task<WorkItemDetails?> GetDetailsAsync(long workItemId, CancellationToken ct = default);
    Task<bool> AddCommitAsync(long workItemId, CommitEvidenceRequest request, CancellationToken ct = default);
    Task<bool> AddTestEvidenceAsync(long workItemId, TestEvidenceRequest request, CancellationToken ct = default);
    Task<bool> AddDependencyAsync(long workItemId, DependencyRequest request, CancellationToken ct = default);
}
