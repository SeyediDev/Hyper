# Work Management Domain

The work-management domain is the shared coordination layer for multi-chat and multi-agent development. It is separate from Integration, Accounting and Neo.Bpms. The AdminPanel may consume its API, but task ownership and logs are not stored in panel entities.

The operational source of truth is the `WorkManagement` database. The active backlog from `docs/BACKLOG.md` is seeded idempotently by `WorkManagementSchemaProvisioner`; the document remains the architectural/release history, not a second task board.

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

## Chat intake

`POST /api/work-management/v1/chat-intake` creates a WorkItem and preserves the original message. Follow-up messages are appended through the log endpoint; the task description is never silently replaced.
