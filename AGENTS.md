# Hyper repository work coordination

Before starting or resuming implementation in this repository, read and follow
[hyper-work-management](.agents/skills/hyper-work-management/SKILL.md), then its
linked [operational reference](docs/WORK_MANAGEMENT.md).
This also applies after a restart, handoff or context compaction; chat history is
not proof of current task ownership.

Use the `WorkManagement` database as the task/role source of truth. Confirm the
project, domain, task, owner and active role before editing. Do not take over an
item being worked on by another chat. Read-only questions and reviews do not
authorize task mutations or implementation.

The main chat remains on `develop` at the user's explicit request. Other chats
use separate task branches/worktrees. This exception does not permit overlapping
file edits or committing another agent's changes.

Keep domain boundaries intact: `Neo.Bpms` is for AdminPanel only; other APIs may
use Neo framework/tools. A new dependency on `Neo.Bpms` outside the panel needs
the user's approval. Work-management records belong to their own domain/database.

When changing the workflow, update the canonical skill and its reference together.
Do not maintain an independent second backlog in Markdown.
