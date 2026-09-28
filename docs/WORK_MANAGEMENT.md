# Hyper work coordination

The configured independent Neo board is the operational source of truth. Hyper's
WorkManagementPageController redirects to AgentOrchestration:WebUrl and no longer
hosts the old work-management API. Current local Web: http://localhost:5181/;
API: http://localhost:5180/; database: NeoAgentOrchestration; schema: nao.
Resolve configuration and verify the live catalog before acting. These local
addresses are not deployment defaults.

docs/BACKLOG.md, the root BACKLOG.md and the old WorkManagement database are
historical context. Do not seed, recreate or maintain a second operational board.

## Entry point and continuous execution

[AGENTS.md](../AGENTS.md) requires the canonical
[hyper-work-management skill](../.agents/skills/hyper-work-management/SKILL.md).
The .codex/skills/hyper-work-management/SKILL.md file is a compatibility pointer.
Open the repository at Backend or below and reread instructions after a restart
or handoff. Query current ownership; do not infer it from history or timestamps.

The user requested continuous execution: after each completed task, refresh the
board and immediately take the next eligible unfinished Hyper task by explicit
user priority, then Critical > High > Normal > Low. At equal priority prefer work
that unlocks dependencies, then older work. Review and Backlog participate after
their remaining acceptance work and dependencies are reassessed. Do not stop
after one task while eligible authorized work remains. Record a real blocker and
continue another eligible task when necessary; do not fabricate completion or
repeatedly retry an unchanged external dependency. A user pause or scope change
overrides this request. This is an execution loop, not a scheduler or permission
to publish/deploy outside the authorized scope.

## Scope, ownership and intake

Announce project/domain, task key, role, agent/chat, branch and file scope. The
user-designated main chat stays on develop; concurrent implementation workers use
separate branches/worktrees. Preserve other changes and the shared index.

Read catalog, all board pages, details, dependencies, children, history/evidence
and runs. Role availability is workspace-wide, across projects. Claim one Ready
task for an enabled idle role; verify subject/chat, branch, version and the single
open timer. Never take another chat's InProgress task or overwrite its files.
An old timer is not authorization to release ownership.

New actionable requests use scoped POST items. Preserve the original request in
Description; append subsequent decisions through logs. Reconcile the project/key
and existing records before retrying uncertain creation. Neo has no separate
idempotent chat-intake endpoint. The authenticated subject determines AgentId;
send the real chat ID through X-Orchestration-Chat.

## Current API

Base route:
`/api/orchestration/v1/organizations/{organizationId}/workspaces/{workspaceId}`.

Use configured workspace authentication. An already enabled loopback Development
host supports X-Orchestration-Local: true, yielding subject local-web. Do not
change authentication or enable this mode to gain access. Every mutation also
needs the actual chat ID in X-Orchestration-Chat.

| Operation | Route and request |
| --- | --- |
| Catalog: projects, roles, profiles, workflows | GET catalog |
| Board | GET items?projectId=...&includeArchived=true&skip=0&take=200; page until total is covered |
| Details | GET items/{id}: children, dependencies, logs, evidence, time, former owners |
| Create | POST items: ProjectId, Key, Title, Domain, Priority, Description, optional ParentWorkItemId/EstimatedSeconds |
| Claim | POST items/{id}/claim: RoleId, ExpectedVersion, Branch |
| Status | POST items/{id}/status: ExpectedVersion, Status, Note |
| Log | POST items/{id}/logs: ExpectedVersion, Message |
| Evidence | POST items/{id}/evidence: ExpectedVersion, Kind, Reference, Outcome, Details, optional CommitSha |
| Dependency | POST items/{id}/dependencies: ExpectedVersion, DependsOnWorkItemId |
| Time | POST items/{id}/time/start or /time/stop: ExpectedVersion |
| Archive / restore | POST items/{id}/archive or /restore: ExpectedVersion |
| Forecast | PUT items/{id}/estimate: ExpectedVersion, Seconds |
| Runs | GET items/{id}/runs; managed execution requires its own authorized workflow |

Use the newest returned GUID version. On 409, reload and reassess; do not blindly
replay against a newer version. Neo enforces owner/version, dependency and role
availability rules in serialized workspace transactions. Still inspect file
overlap in other worktrees and projects before editing.

Contract sources in sibling Neo/products/Neo.AgentOrchestration/src:
Neo.AgentOrchestration.Api/WorkItemsController.cs,
Neo.AgentOrchestration.Application/Work/WorkItemHandlers.cs,
Neo.AgentOrchestration.Domain/Work/WorkItem.Lifecycle.cs.
The product skill and docs/API.md describe managed runs and additional routes.

## Status and evidence limits

Persisted status: Backlog=1, Ready=2, InProgress=3, Blocked=4, Review=5, Done=6,
Cancelled=7. Priority: Low=1, Normal=2, High=3, Critical=4.
These numeric values are for SQL storage; HTTP requests use enum names such as
Ready, Test and Passed, not numbers or numeric strings.

Claim is Ready -> InProgress. Normal status changes allow Backlog -> Ready/Cancelled;
Ready -> Blocked/Cancelled; InProgress -> Review/Blocked/Cancelled;
Blocked -> Ready/Cancelled; Review -> Ready/Done/Blocked/Cancelled.
The current API has no return-to-Backlog, reopen-Done or delete operation.
Archive accepts only Blocked/Done. Cancellation/archival does not delete a task.
Ownership also applies to notes/evidence and stopped tasks retaining their owner.

EvidenceKind: Commit=1, Test=2, Artifact=3.
EvidenceOutcome: NotApplicable=0, Passed=1, Failed=2, Skipped=3.
Commit references require a full Git SHA and NotApplicable; Test requires a
nonzero outcome. Record the commit first when binding test/artifact CommitSha.
Imported records may have enum mapping defects: verify actual source/logs before
repairing classification; never invent a passing result or a commit.

Done requires actual acceptance, completed dependencies and required children.
Record exact test command, result and scope. A build is not a browser check;
a fixture is not a live provider test; code existence alone is not acceptance.
Claim starts tracking; resume never creates a second interval. Completion or
handoff stops it. Verify persisted status, evidence and closed time entries.
Keep uncertainty about time spent during outages explicit.

## Explicit administrative backlog reconciliation

For an authorized audit inspect every status, including archived/completed items.
Return Blocked to Backlog when requested, preserving reasons and dependencies for
reevaluation. Preserve unrelated active ownership. Merge only confirmed duplicate
acceptance scopes; retain distinct parents, children and acceptance tests.

Prefer the API for supported operations. A user-authorized local SQL maintenance
transaction may handle unsupported reconciliation without changing the public API
or impersonating an owner. Inspect actual schema/FKs, save a recoverable before
snapshot and explicit changes/reasons. Use parameterized commands, SET XACT_ABORT
ON, a transaction and the same application lock as Neo:
neo-orchestration:workspace:{workspaceId}, Exclusive, Transaction owner.
Recheck expected versions under row locks; abort on concurrent change. Preserve
active assignments/timers. Append audit logs and refresh Version/UpdatedAtUtc for
changed aggregates. Never expose credentials or connection strings in artifacts.

Before duplicate deletion retain unique descriptions, logs, evidence and time;
rewire children and incoming/outgoing dependencies without cycles, self-edges or
duplicate pairs. Inspect AgentRuns, deliveries, receipts and approvals; never
erase execution history. Check evidence sequence uniqueness and referential
integrity. Retain a merge record naming removed and canonical keys.
After commit, re-read through the API and verify mutations, owners/timers and no
orphans. An audit snapshot is evidence of one operation, not a parallel board.
Only completed commands and persisted reads prove success.
