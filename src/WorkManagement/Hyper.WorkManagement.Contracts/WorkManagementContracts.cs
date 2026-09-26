namespace Hyper.WorkManagement.Contracts;

public enum WorkItemStatus : byte { Backlog = 1, Ready = 2, InProgress = 3, Blocked = 4, Review = 5, Done = 6, Cancelled = 7 }
public enum WorkItemPriority : byte { Low = 1, Normal = 2, High = 3, Critical = 4 }
public sealed record WorkRoleSummary(string Key, string Name, string Scope, bool IsAvailable, long? ActiveWorkItemId);
public sealed record WorkItemSummary(long Id, string ProjectKey, long? ParentWorkItemId, string Key, string Title, string Domain, WorkItemStatus Status, WorkItemPriority Priority,
    string? OwnerRole, string? OwnerAgent, string? Branch, string? ChatId, string? Description, DateTime UpdatedAtUtc,
    long TimeSpentSeconds, long? EstimatedSeconds, int ProgressPercent, bool IsTracking, int ChildCount, bool IsArchived);
public sealed record WorkBoardMetrics(int TotalItems, int ActiveItems, int BlockedItems, int DoneItems, int ArchivedItems,
    long TimeSpentSeconds, long EstimatedSeconds, int ProgressPercent);
public sealed record WorkBoardResponse(IReadOnlyList<WorkItemSummary> Items, IReadOnlyList<WorkRoleSummary> Roles,
    IReadOnlyList<string> Projects, IReadOnlyList<string> Domains, WorkBoardMetrics Metrics);
public sealed record ClaimWorkItemRequest(string RoleKey, string AgentId, string ChatId, string Branch);
public sealed record WorkLogRequest(string Author, string Message, string? ChatId = null);
public sealed record ChatIntakeRequest(string ChatId, string Author, string Message, string? SuggestedTitle = null, string? Domain = null);
public sealed record ChatIntakeResponse(long IntakeId, long? WorkItemId, string Status);
public sealed record CreateWorkItemRequest(string ProjectKey, string Key, string Title, string Domain, WorkItemPriority Priority = WorkItemPriority.Normal, string? Description = null, long? ParentWorkItemId = null, long? EstimatedSeconds = null);
public sealed record EstimateWorkItemRequest(long? EstimatedSeconds);
public sealed record AgentProfileView(long Id, string Key, string Name, string RoleKey, string Provider, string? Model, string? Instructions, string? SkillPath, bool IsEnabled);
public sealed record WorkflowTransitionView(long Id, string Key, string? ProjectKey, string? Domain, WorkItemStatus FromStatus, WorkItemStatus TriggerStatus, string FromRoleKey, string NextRoleKey, WorkItemStatus NextStatus, bool RequireTests, bool RequireCommits, bool AutoDispatch, bool IsEnabled);
public sealed record AgentRunView(long Id, long WorkItemId, string WorkItemKey, long AgentProfileId, string AgentProfileKey, string TargetRoleKey, string Status, string? ExternalRunId, string? Branch, string? LastError, DateTime CreatedAtUtc, DateTime? StartedAtUtc, DateTime? CompletedAtUtc);
public sealed record AgentRunCallbackRequest(string Status, string? ExternalRunId = null, string? Error = null);
public sealed record CreateAgentProfileRequest(string Key, string Name, string RoleKey, string Provider = "codex", string? Model = null, string? Instructions = null, string? SkillPath = null, bool IsEnabled = true);
public sealed record CreateWorkflowTransitionRequest(string Key, string? ProjectKey, string? Domain, WorkItemStatus FromStatus, WorkItemStatus TriggerStatus, string FromRoleKey, string NextRoleKey, WorkItemStatus NextStatus = WorkItemStatus.Review, bool RequireTests = false, bool RequireCommits = false, bool AutoDispatch = true, bool IsEnabled = true);
public sealed record AgentOrchestrationSnapshot(IReadOnlyList<AgentProfileView> Profiles, IReadOnlyList<WorkflowTransitionView> Transitions, IReadOnlyList<AgentRunView> RecentRuns);
public sealed record ChangeWorkItemStatusRequest(WorkItemStatus Status, string Author, string? Message = null);
public sealed record CommitEvidenceRequest(string Sha, string? Message = null);
public sealed record TestEvidenceRequest(string TestName, string Result, string? Details = null);
public sealed record DependencyRequest(long DependsOnWorkItemId);
public sealed record WorkItemDetails(WorkItemSummary Item, IReadOnlyList<WorkLogView> Logs, IReadOnlyList<CommitEvidenceView> Commits, IReadOnlyList<TestEvidenceView> Tests, IReadOnlyList<long> Dependencies, IReadOnlyList<WorkItemSummary> Children, IReadOnlyList<TimeEntryView> TimeEntries);
public sealed record WorkLogView(string Author, string Message, string? ChatId, DateTime CreatedAtUtc);
public sealed record CommitEvidenceView(string Sha, string? Message, DateTime CreatedAtUtc);
public sealed record TestEvidenceView(string TestName, string Result, string? Details, DateTime CreatedAtUtc);
public sealed record TimeEntryView(DateTime StartedAtUtc, DateTime? EndedAtUtc, long DurationSeconds, string? Note);
public sealed record TimeTrackingRequest(string? Note = null);
public interface IWorkManagementApi
{
    Task<WorkBoardResponse> GetBoardAsync(string? domain, string? project = null, string? role = null, bool includeArchived = false, CancellationToken ct = default);
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
    Task<WorkItemSummary?> StartTrackingAsync(long workItemId, TimeTrackingRequest request, CancellationToken ct = default);
    Task<WorkItemSummary?> StopTrackingAsync(long workItemId, TimeTrackingRequest request, CancellationToken ct = default);
    Task<bool> ArchiveAsync(long workItemId, CancellationToken ct = default);
    Task<bool> UnarchiveAsync(long workItemId, CancellationToken ct = default);
    Task<WorkItemSummary?> SetEstimateAsync(long workItemId, EstimateWorkItemRequest request, CancellationToken ct = default);
    Task<AgentOrchestrationSnapshot> GetOrchestrationAsync(CancellationToken ct = default);
    Task<AgentProfileView?> CreateAgentProfileAsync(CreateAgentProfileRequest request, CancellationToken ct = default);
    Task<WorkflowTransitionView?> CreateWorkflowTransitionAsync(CreateWorkflowTransitionRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<AgentRunView>> GetRunsAsync(long workItemId, CancellationToken ct = default);
    Task<AgentRunView?> DispatchNextAsync(long workItemId, CancellationToken ct = default);
    Task<AgentRunView?> CompleteRunAsync(long runId, AgentRunCallbackRequest request, string? harnessKey, CancellationToken ct = default);
}
public interface IAgentOrchestrationService
{
    Task<AgentOrchestrationSnapshot> GetAsync(CancellationToken ct = default);
    Task<AgentProfileView?> CreateProfileAsync(CreateAgentProfileRequest request, CancellationToken ct = default);
    Task<WorkflowTransitionView?> CreateTransitionAsync(CreateWorkflowTransitionRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<AgentRunView>> GetRunsAsync(long workItemId, CancellationToken ct = default);
    Task<AgentRunView?> DispatchNextAsync(long workItemId, CancellationToken ct = default);
    Task<AgentRunView?> CompleteRunAsync(long runId, AgentRunCallbackRequest request, string? harnessKey, CancellationToken ct = default);
    Task OnStatusChangedAsync(long workItemId, WorkItemStatus fromStatus, WorkItemStatus toStatus, CancellationToken ct = default);
}
