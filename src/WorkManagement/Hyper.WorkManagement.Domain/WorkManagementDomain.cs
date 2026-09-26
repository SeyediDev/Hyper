using Hyper.WorkManagement.Contracts;

namespace Hyper.WorkManagement.Domain;

public sealed class WorkItem
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public WorkProject? Project { get; set; }
    public long? ParentWorkItemId { get; set; }
    public string Key { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Domain { get; set; } = null!;
    public WorkItemStatus Status { get; set; } = WorkItemStatus.Backlog;
    public WorkItemPriority Priority { get; set; } = WorkItemPriority.Normal;
    public string? Description { get; set; }
    public string? OwnerRole { get; set; }
    public string? OwnerAgent { get; set; }
    public string? ChatId { get; set; }
    public string? Branch { get; set; }
    public string? CommitSha { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public long AccumulatedSeconds { get; set; }
    public long? EstimatedSeconds { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAtUtc { get; set; }
}
public sealed class WorkProject
{
    public long Id { get; set; }
    public string Key { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsEnabled { get; set; } = true;
}
public sealed class WorkRole
{
    public long Id { get; set; }
    public string Key { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Scope { get; set; } = null!;
    public bool IsEnabled { get; set; } = true;
}
public sealed class WorkItemDependency
{
    public long Id { get; set; }
    public long WorkItemId { get; set; }
    public long DependsOnWorkItemId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
public sealed class WorkItemCommit
{
    public long Id { get; set; }
    public long WorkItemId { get; set; }
    public string Sha { get; set; } = null!;
    public string? Message { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
public sealed class WorkItemTestEvidence
{
    public long Id { get; set; }
    public long WorkItemId { get; set; }
    public string TestName { get; set; } = null!;
    public string Result { get; set; } = null!;
    public string? Details { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
public sealed class WorkItemTimeEntry
{
    public long Id { get; set; }
    public long WorkItemId { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public long DurationSeconds { get; set; }
    public string? Note { get; set; }
}
public sealed class WorkItemLog
{
    public long Id { get; set; }
    public long WorkItemId { get; set; }
    public string Author { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? ChatId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
public sealed class ChatWorkIntake
{
    public long Id { get; set; }
    public string ChatId { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Message { get; set; } = null!;
    public long? WorkItemId { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
public sealed class AgentProfile
{
    public long Id { get; set; }
    public string Key { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string RoleKey { get; set; } = null!;
    public string Provider { get; set; } = "codex";
    public string? Model { get; set; }
    public string? Instructions { get; set; }
    public string? SkillPath { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
public sealed class WorkflowTransition
{
    public long Id { get; set; }
    public string Key { get; set; } = null!;
    public string? ProjectKey { get; set; }
    public string? Domain { get; set; }
    public WorkItemStatus FromStatus { get; set; }
    public WorkItemStatus TriggerStatus { get; set; }
    public string FromRoleKey { get; set; } = null!;
    public string NextRoleKey { get; set; } = null!;
    public WorkItemStatus NextStatus { get; set; } = WorkItemStatus.Review;
    public bool RequireTests { get; set; }
    public bool RequireCommits { get; set; }
    public bool AutoDispatch { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
public sealed class AgentRun
{
    public long Id { get; set; }
    public long WorkItemId { get; set; }
    public long AgentProfileId { get; set; }
    public string TargetRoleKey { get; set; } = null!;
    public string Status { get; set; } = "Queued";
    public string? ExternalRunId { get; set; }
    public string? Prompt { get; set; }
    public string? Branch { get; set; }
    public string? LastError { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
