# WRK-002 production queue SQL checks

This executable compiles the actual queue, scheduling helper, processor,
business dispatcher, webhook verifier/ingress and production EF mappings.
Synthetic normalized review events reach a controlled engagement owner through
the real processor. Unrelated provider, catalog, accounting and reservation ports
throw an unhandled test exception if reached. No host, external HTTP call,
real message or financial write is involved.

Build from the checkout root:

```powershell
dotnet build tools/QueueSqlChecks/QueueSqlChecks.csproj -m:1 -p:NuGetAudit=false
```

Running without exactly `--sql`, or without `QUEUE_SQL_TEST_CONNECTION`, exits 2
without opening SQL. Supply that environment variable privately for an authorized
test SQL Server login with create/drop database permission, then run:

```powershell
dotnet tools/QueueSqlChecks/bin/Debug/net10.0/QueueSqlChecks.dll --sql
```

The configured database name is never used as a test target. The executable
creates `HyperQueueChecks_<32 lowercase GUID digits>` and validates that exact
name before accessing or removing it. Connections use the generated database
or `master`; pooling is disabled. After successful CREATE acknowledgement,
cleanup drops only that fixture, including on later initialization/test failure.
If CREATE itself fails ambiguously, the tool reports its generated name for
inspection and does not drop a database whose creation it cannot prove.
Cleanup failure returns nonzero. No connection string, SQL error body or
credential is printed. Schema creation uses production EF mappings with migration
exclusion disabled for the fixture; it is not a deployment-script rehearsal.

The checks exercise:

- Concurrent real enqueue and authenticated ingress, exact replay, conflicting
  content, and atomic creation of one audit/inbox/job identity.
- SQL session lock exclusion while an owner call is paused. The fixture expires
  the live worker's lease: another worker must still stay out. A different shop
  can complete concurrently.
- FIFO across a shop's connections and refusal to skip the head through
  connection-specific processing, delayed retry or unknown lease expiry.
- Production Retry-After/backoff persistence, finite retry exhaustion,
  DeadLetter/NeedsAttention and advancement to the next shop event.
- Host cancellation, retained unacknowledged lease and fresh DbContext recovery
  after expiry. This is not an operating-system process-kill test.
- Lease-conditional ACK fencing: fixture-only replacement of a live lease must
  prevent the stale worker from changing either job or inbox.
- Completion atomicity: an isolated-table trigger throws during the real inbox
  UPDATE, so the preceding job ACK must roll back. Fresh-context recovery accepts
  the owner's Duplicate result and completes both rows.

TaskCompletionSource barriers synchronize concurrency. Barrier waits have a
15-second watchdog; worker operations receive a 30-second cancellation token.
All started workers are fully awaited, including in finally blocks, before
fixture cleanup. Cancellation-ignoring production ACK commands remain bounded by
the fixture's SQL command timeouts. Fixture-only SQL changes due/expiry timestamps
instead of waiting for the real minute-scale retry/lease durations. No production
scheduling duration is changed.

The controlled owner tracks synthetic effects by connection/event in memory. It
demonstrates the queue's **at-least-once delivery** contract and the need for an
idempotent owner; it does not establish production owner persistence, exactly-once
financial effects, real provider envelopes, server restart or deployment safety.
Review events do not exercise AwaitingOutboxDelivery polling. Ordering is durable
queue ID within a shop, not provider timestamps/sequences.

Before deployment, drain all older scenario processors: their connection locks
do not coordinate with the shop lock. This tool does not stop/deploy any host.
Compilation alone is not SQL acceptance; record the executable exit status and
final assertion count only after an authorized fixture run completes.
