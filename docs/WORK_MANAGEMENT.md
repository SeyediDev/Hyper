# Work Management Domain

The work-management domain is the shared coordination layer for multi-chat and multi-agent development. It is separate from Integration, Accounting and Neo.Bpms. The AdminPanel may consume its API, but task ownership and logs are not stored in panel entities.

The operational source of truth is the `WorkManagement` database. The active backlog from `docs/BACKLOG.md` is seeded idempotently by `WorkManagementSchemaProvisioner`; the document remains the architectural/release history, not a second task board.

## Repository entry points

[AGENTS.md](../AGENTS.md) requires implementation chats to read the canonical
[hyper-work-management skill](../.agents/skills/hyper-work-management/SKILL.md).
The old `.codex/skills/hyper-work-management/SKILL.md` is a compatibility pointer.
Open the repository at `Backend` (the Git root), or a directory below it, so the
repository instructions and skill are in scope. Chats already running should
explicitly reload `AGENTS.md` and the skill; their earlier context is not evidence
that updated instructions were loaded. No personal/global Codex settings are
changed. This is agent guidance, not database-enforced mutual exclusion.

## Session handshake

Each chat starts with: `Role Key`, `Agent Id`, `Chat Id`, active `Task Key`, branch and scope. If no task is active, query `/api/work-management/v1/roles` and claim the highest-priority matching item through `/items/{id}/claim`.

Every new chat follows this order: select exactly one available role, claim one Ready item, record the chat and branch, append progress logs, attach the commit SHA and test evidence, then move the item to Review/Done or Blocked with a reason. A new request becomes a task only through `chat-intake`.

## Concurrency gate

Before editing, query the board for the exact `Task Key` and its `OwnerRole`, `OwnerAgent`, `ChatId`, `Branch` and status. If the item is `InProgress` for another chat/agent, do not start it, do not edit its files and do not claim it; choose another Ready item or request a handoff. Also inspect the other task's branch/worktree for file overlap. A task may be reassigned only after an explicit handoff log or `Blocked`/`Review` transition.

Each item carries both `ProjectKey` and `Domain`. A work item may have a `ParentWorkItemId`; subtasks use the same claim, status, dependency, evidence and time-tracking rules as their parent. Time is recorded as start/stop entries in `WorkItemTimeEntries`, while the API exposes accumulated and currently running seconds.

Time endpoints: `POST /api/work-management/v1/items/{id}/time/start` and `POST /api/work-management/v1/items/{id}/time/stop`. Starting time requires an owned `InProgress` item; stopping time closes the open entry and adds its duration to the item total.

## Statuses

`Backlog → Ready → InProgress → Review → Done`; use `Blocked` when an external dependency prevents progress and `Cancelled` only with an explicit decision. A role has at most one `InProgress` item.

## Concurrency

One task has one branch. Concurrent tasks use `git worktree add` with separate directories. Shared contracts/schema are serialized through `architecture-lead`; all other work must avoid overlapping files. A commit is required for each coherent stage and its SHA belongs in the task record.

The user-designated main chat is explicitly allowed to stay on `develop`.
Other workers still use separate worktrees/branches; the exception never permits
overlapping file edits or blanket staging. Build output directories may still be
shared through Neo project references: a file lock is not permission to kill
another worker or delete its outputs. Report targeted validation accurately.

## Chat intake

`POST /api/work-management/v1/chat-intake` creates a WorkItem and preserves the original message. Follow-up messages are appended through the log endpoint; the task description is never silently replaced.

## Current API and implementation limits

Routes below are relative to `/api/work-management/v1` and require the AdminPanel
host's authentication. Never disable authentication to automate coordination.
Source of truth for route/DTO changes:
`src/AdminPanel/Hyper.AdminPanel.Web/Controllers/WorkManagementController.cs`,
`src/WorkManagement/Hyper.WorkManagement.Contracts/WorkManagementContracts.cs`,
and `src/WorkManagement/Hyper.WorkManagement.Infrastructure/WorkManagementService.cs`.

| Operation | Route / request fields |
| --- | --- |
| Read board / roles | `GET /board?project=HYPER&domain=...`, `GET /roles` |
| Read item, logs, tests, children and dependencies | `GET /items/{id}` |
| Managed chat intake | `POST /chat-intake`: `ChatId`, `Author`, `Message`, optional `SuggestedTitle`, `Domain` |
| Create explicit project/subtask | `POST /items`: `ProjectKey`, `Key`, `Title`, `Domain`, `Priority`, `Description`, `ParentWorkItemId` |
| Claim | `POST /items/{id}/claim`: `RoleKey`, `AgentId`, `ChatId`, `Branch` |
| Append context | `POST /items/{id}/logs`: `Author`, `Message`, `ChatId` |
| Attach evidence | `POST /items/{id}/commits`: `Sha`, `Message`; `POST /items/{id}/tests`: `TestName`, `Result`, `Details` |
| Add dependency | `POST /items/{id}/dependencies`: `DependsOnWorkItemId` |
| Change status | `POST /items/{id}/status`: `Status`, `Author`, `Message` |
| Start/stop elapsed time | `POST /items/{id}/time/start` or `/time/stop`: optional `Note` |

Persisted status values: Backlog=1, Ready=2, InProgress=3, Blocked=4, Review=5,
Done=6, Cancelled=7. Priority: Low=1, Normal=2, High=3, Critical=4.
Current chat intake defaults to project `HYPER`; it has no `ProjectKey` field.
Do not claim that this endpoint handles arbitrary project selection. Explicit
item creation supports projects and same-project parents.

The current service does not provide an ownership lease/CAS token, enforce all
dependency/status-transition policies, or safely reject every attempt to reclaim
an active item. Therefore a successful claim response alone is not proof of
exclusive ownership. Follow the skill's pre/post ownership checks and serialize
competing claims; if exclusive ownership cannot be established, do not edit.
The schema provisioner's optional claim mode is not a concurrency-safe task
client and must not replace this gate.

## Local database fallback

Prefer the API when an authenticated host is available. Within authorized local
database access, a guarded SQL transaction can maintain the same records without
starting the panel. This is an audited fallback, not a separate backlog or a new
public API. Inspect the current schema before writing; do not expose credentials.
`WorkItems.ProjectId` joins `Projects.Id`; `ProjectKey` is an API field, not a
`WorkItems` column. Do not run the schema provisioner just to query the board.

- Use `SET XACT_ABORT ON`, a transaction, ownership/status predicates and
  appropriate `UPDLOCK,HOLDLOCK` reads for the task and role's active items.
  Check the role is enabled and idle and the item is eligible. Abort on conflict;
  never overwrite another chat's claim. Other clients must still cooperate with
  the workflow; a fallback transaction is not a global lease mechanism.
- For intake preserve the user's original message in `ChatWorkIntakes`, link it
  to the created `WorkItems` row, and record scope/decisions in `WorkItemLogs`.
  Check for a prior intake/task before retrying an uncertain operation.
- Claim with the actual project/domain, role, agent, chat and branch; create only
  one open `WorkItemTimeEntries` row and set `StartedAtUtc` consistently.
- On completion/handoff, lock and verify the exact owner first; close the open
  interval, add its duration once to `AccumulatedSeconds`, clear `StartedAtUtc`,
  update status/timestamps, and attach commit/test/log records in the transaction.
  Use the timestamps actually recorded and investigate multiple open intervals.
- Re-read final status, evidence and timer rows after commit. Wait for SQL exit
  status; absence of output or a truncated result is not evidence of success.
  If neither API nor authorized SQL is available, report the coordination blocker
  and limit work to read-only inspection until ownership can be verified.

## Workflow validation

When maintaining the skill, validate its frontmatter using skill-creator's
`scripts/quick_validate.py` if available, resolve its relative links and check
the API examples against the controller/contracts above. Review these cases:
an item owned by another chat, a resumed own item with an open timer, a database
access failure, a parent with unfinished subtasks, a partial build, and completion
with evidence. They must not imply takeover, duplicate timing, unclaimed edits
or untested success. Documentation changes do not require live Basalam calls.
