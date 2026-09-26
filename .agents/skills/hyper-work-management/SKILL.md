---
name: hyper-work-management
description: Coordinate Hyper repository implementation across chats using WorkManagement tasks, role ownership, dependencies, time entries, commits and handoffs. Use when starting or resuming development, taking backlog work, or recording task progress; read-only advice does not create or claim work.
---

# Hyper work management

Read [the operational reference](../../../docs/WORK_MANAGEMENT.md) before task
actions. Paths in that reference are relative to the repository root. The database
is the operational source of truth; `docs/BACKLOG.md` is architectural/release
context, not a parallel board. Do not introduce another database or MCP server
just to follow this workflow.

## Start or resume

1. Resolve the Git root, branch/worktree and dirty files. Read the board, available
   roles and the candidate item's details, dependencies, children, logs and tests.
2. Announce `ProjectKey`, `Domain`, `Task Key`, `Role Key`, `Agent Id`, `Chat Id`,
   branch and file scope. Use the real session ID when available, otherwise a
   unique recorded chat identifier; do not copy another chat's identity.
3. Resume an active item only when its recorded owner/chat is this session. A
   restart or an old timestamp does not authorize stealing or releasing a claim.
   For new work, use one available role and the highest-priority Ready item in
   its scope whose dependencies are satisfied. An explicit user task takes
   precedence after intake and ownership checks. Never claim a second active
   item for that role; a busy role is not an invitation to reassign its task.
4. Check file overlap with other active items and their branches/worktrees before
   claiming. Re-read ownership after the claim and before edits, commits and
   completion. If another chat owns it, leave it untouched and select other work
   or request a documented handoff. Do not assume the current claim API prevents
   every takeover/race; see the reference's implementation limits.

If the board cannot be reached, diagnose read-only within available permissions.
Do not infer that SQL Server is stopped from a sandbox, authentication or network
failure. Do not start unclaimed edits, silently create an offline board, or claim
that a write succeeded without verifying it. Use the documented local fallback
only within the user's authorized database access.

## Execute without overlap

- One task branch/worktree per concurrent worker. The user-designated main chat
  stays on `develop`; other chats do not inherit that exception. Preserve dirty
  files and the shared index. Stage/commit only the claimed changes.
- Record the intended file scope. Coordinate shared architecture, contracts and
  schema with `architecture-lead`; add a dependency or obtain a handoff before
  crossing another task's scope. A different role name does not remove overlap.
- Keep each item associated with its project and domain. Subtasks use the same
  ownership, dependencies, logs, evidence and time rules; owning a parent does
  not grant ownership of its children.
- For a new actionable chat request, use managed chat intake; preserve the
  original message. Append follow-up decisions, progress and user clarifications
  to logs instead of replacing the original description. Avoid duplicate intake
  after retries/restarts. Never put tokens, passwords or full connection strings
  in task descriptions, logs, commits or evidence.
- Prioritize the requested working scenario. Do not expand routine work into
  architecture tests, unrelated hardening or infrastructure without need.

## Evidence, time and handoff

Claiming/entering InProgress starts tracking. Verify exactly one open time entry;
do not create another on resume. Stop tracking when work is paused, handed off or
leaves InProgress. Record elapsed active intervals, not token counts or estimates;
if a reset left the timer running through downtime, log the uncertainty and use
known timestamps only, never invent time corrections.

Commit each coherent verified stage when authorized and attach its actual SHA.
Record test command, scope, result and limitations. Wait for an exit status;
partial output is not success. Distinguish a targeted build with existing
dependencies from a full build, SQL fixtures from live external tests, and
compiled Razor from browser verification. Never stop other agents' processes or
clean their outputs to unblock a build.

Before completing or handing off, append a compact durable log containing:

- outcome and acceptance criteria covered;
- files changed, decisions and relevant context;
- commit SHA and test evidence, including failures/untested scope;
- blockers/dependencies and the precise next action for the next agent.

Mark Done only when this item's acceptance scope is met and required subtasks are
complete. Use Review for pending review and Blocked for a documented dependency
or missing authority; do not cancel work without an explicit decision. Verify
the persisted status, commit/evidence and stopped timer: no open time entry,
`StartedAtUtc` cleared, elapsed duration included in `AccumulatedSeconds`.
Then check role availability before taking another task. Report completion and
remaining limits plainly; never claim that documentation provides a server-side
lock or guarantees another running chat has loaded updated instructions.
