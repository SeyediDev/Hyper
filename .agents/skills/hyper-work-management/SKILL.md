---
name: hyper-work-management
description: Coordinate Hyper implementation through the current Neo work board, ownership, dependencies, evidence and time. Use when starting or resuming development, auditing backlog status, or continuously completing unfinished tasks in priority order; advice alone does not authorize execution.
---

# Hyper work management

Read [the operational reference](../../../docs/WORK_MANAGEMENT.md) before task
actions. Paths in that reference are relative to the repository root. The database
behind the configured Neo API is the operational source of truth;
`docs/BACKLOG.md` is architectural/release context, not a parallel board.
Hyper now redirects to the independent Neo board. Do not resume writes to the
legacy `WorkManagement` database or run its seed/provisioner for coordination.

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
   or request a documented handoff. Supply the latest ExpectedVersion for every
   existing-item mutation; on conflict, reload and reassess instead of blindly retrying.

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
- For a new actionable chat request, use scoped task creation; preserve the
  original message. Append follow-up decisions, progress and user clarifications
  to logs instead of replacing the original description. Avoid duplicate intake
  after retries/restarts. Never put tokens, passwords or full connection strings
  in task descriptions, logs, commits or evidence.
- Prioritize the requested working scenario. Do not expand routine work into
  architecture tests, unrelated hardening or infrastructure without need.

## Continuous priority execution

The user has requested continuous execution of unfinished Hyper backlog tasks.
After finishing a task, immediately refresh the live board and start the next
eligible task without asking whether to continue. Apply the same loop when the
user asks to resume or work through the backlog; a read-only question does not
start execution. A later pause or scope change overrides this standing request.

- Order by explicit user priority, then Critical, High, Normal, Low; at equal
  priority prefer dependencies that unblock other work, then the older task.
  Consider Backlog and Review as well as Ready: reevaluate their actual remaining
  acceptance work, move eligible work to Ready, and claim it before editing.
- Inspect all pages, dependencies, children, evidence and current source. Done
  means the acceptance scope is met, not that a title resembles implemented code.
  Preserve a correct Done status; repair an incorrect one with an audit reason.
- Finish required validation, record actual evidence and elapsed time, persist
  the outcome and verify the timer is stopped before claiming the next task.
  Keep only one active task per role. Independent read-only subagent reviews
  may share the parent audit; implementation workers need separate claims/scopes.
- Reassess old blockers. For an explicit backlog audit requesting it, return
  Blocked items to Backlog with their blocker history intact, then test whether
  the dependency is resolved. Backlog placement does not resolve a dependency.
- If a task needs an unavailable external contract, an unanswered material user
  decision, or another chat's owned files, record the precise next action and
  continue with other eligible work. Do not repeatedly retry the same blocker
  or mark it Done. Stop only when no eligible authorized work remains, the user
  pauses, or an actual execution limit prevents continuation; report what remains.
- Merge only demonstrated duplicate acceptance scopes, retaining the canonical
  task's evidence and incoming/outgoing links. Parent/child tasks, implementation
  versus acceptance tests, and similarly named distinct outcomes are not duplicates.

This loop authorizes work within the requested project scope. It does not itself
schedule background executions, publish externally, or grant a missing business
decision. Use the operational reference for API limitations and audited maintenance.

ProjectKey and RoleKey identify the human-facing scope; resolve their ProjectId
and RoleId GUIDs from the current catalog before sending API requests.

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
each closed interval has its real DurationSeconds and IsTracking is false.
Then check role availability before taking another task. Report completion and
remaining limits plainly; never claim that documentation provides a server-side
lock or guarantees another running chat has loaded updated instructions.
