# Work Management Domain

The work-management domain is the shared coordination layer for multi-chat and multi-agent development. It is separate from Integration, Accounting and Neo.Bpms. The AdminPanel may consume its API, but task ownership and logs are not stored in panel entities.

## Session handshake

Each chat starts with: `Role Key`, `Agent Id`, `Chat Id`, active `Task Key`, branch and scope. If no task is active, query `/api/work-management/v1/roles` and claim the highest-priority matching item through `/items/{id}/claim`.

## Statuses

`Backlog → Ready → InProgress → Review → Done`; use `Blocked` when an external dependency prevents progress and `Cancelled` only with an explicit decision. A role has at most one `InProgress` item.

## Concurrency

One task has one branch. Concurrent tasks use `git worktree add` with separate directories. Shared contracts/schema are serialized through `architecture-lead`; all other work must avoid overlapping files. A commit is required for each coherent stage and its SHA belongs in the task record.

## Chat intake

`POST /api/work-management/v1/chat-intake` creates a WorkItem and preserves the original message. Follow-up messages are appended through the log endpoint; the task description is never silently replaced.
