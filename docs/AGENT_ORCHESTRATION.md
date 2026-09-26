# Agent orchestration

The Work Management domain now contains a provider-neutral orchestration layer.
It does not impersonate the Codex app or silently call a cloud account.

## Components

- `AgentProfiles`: role, provider, model, instructions and repository skill path.
- `WorkflowTransitions`: gated role handoff rules.
- `AgentRuns`: queued, dispatched and configuration/error outcomes.
- `HttpAgentHarnessDispatcher`: an adapter for a Codex SDK gateway, Agents API
  worker or MCP bridge. It accepts a JSON dispatch envelope and returns an
  external run identifier.

The default configuration keeps the adapter disabled. Enable it only with an
approved endpoint and secret held by deployment configuration, never in task logs
or source control.

## Default flow

`InProgress → Review` can create a run for `quality` when tests and commits are
present. A later `quality → Review` transition can create a run for
`panel-operations`. The panel exposes profiles, transitions and recent runs and
allows an administrator to add the initial records.

The adapter is intentionally asynchronous at the boundary: a successful HTTP
acceptance means the external harness accepted a run, not that the code change is
complete. The external callback/worker must later attach commits, tests and logs,
then move the Work Management item through its normal status rules.

The callback is protected by the configured Harness key in the
`X-Agent-Harness-Key` header. Keep this key in deployment secrets. An absent key
or disabled endpoint cannot mark a run successful.
