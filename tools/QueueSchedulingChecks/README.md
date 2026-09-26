# Shop-ordered scenario scheduling checks

Run from the repository root:

```powershell
dotnet run --project tools/QueueSchedulingChecks/QueueSchedulingChecks.csproj
```

The checks compile the production scheduling helper and cover FIFO selection across
a shop's connections, retry/lease eligibility, terminal states, connection-specific
processing and paging across shops. SQL Server translation is checked without
opening a database connection. No SQL fixture or external provider is used.

These checks do **not** establish live SQL lock exclusion, crash recovery,
inbox acknowledgement atomicity or exactly-once financial effects. The queue's
existing event identity and lease acknowledgement mechanisms are unchanged.
Ordering is by durable queue ID, not a provider timestamp/version.

Deployment must drain/stop every old scenario processor (including any panel
host that processes the queue) before starting the new version. Old builds use
connection-scoped application locks and cannot coordinate with the new shop key;
do not mix versions during a rolling deployment. No host is stopped by this tool.
