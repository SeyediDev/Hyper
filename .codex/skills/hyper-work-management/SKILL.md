---
name: hyper-work-management
description: Manage Hyper development roles, backlog tasks, chat-to-task intake, ownership, branches, commits and handoffs across Codex sessions.
---

# Hyper work management

At the start of a development chat, select one role and report its `Role Key`, scope, active task, branch and chat id. Never claim a second active task for the same role until the current task is Done, Blocked or explicitly handed off.

Use the Work Management API and database as the operational source of truth; `docs/BACKLOG.md` remains the architectural and release-level backlog. A chat explanation becomes a task only through the managed `chat-intake` flow, which preserves the original message as the task description and creates an auditable log.

Every task must have: key, domain, owner role, owner agent, chat id, branch, status, priority, dependencies, latest commit and test evidence. Before editing, claim the task. Before handoff, append a log containing files changed, decisions, tests, blockers and next action. Commit each coherent stage and record its SHA in the task.

Use one branch per task. Use separate git worktrees for concurrent tasks; do not use parallel edits in one working tree. Do not modify files outside the claimed scope without adding a dependency or requesting a handoff. Architecture, contracts, schema and shared files are coordinated by `architecture-lead`.

Role keys currently seeded by schema: `architecture-lead`, `basalam-integration`, `accounting-platform`, `worker-operations`, `quality`, `panel-operations`. If a role has no active item, select the highest-priority Ready item matching its scope; otherwise report availability.

For the detailed API and status transition rules, read `docs/WORK_MANAGEMENT.md`.
