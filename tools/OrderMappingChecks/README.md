# Order mapping SQL acceptance

This executable source-links the production Integration dispatcher, receipt helper,
context and entity configuration. It uses actual SQL Server persistence and
controlled accounting ACK/reservation probes. It does not start a host, use an
application database, contact a provider or accounting API, or write native invoices.

Build from the repository root:

```powershell
dotnet build tools/OrderMappingChecks/OrderMappingChecks.csproj -m:1 -p:NuGetAudit=false -v:minimal
```

For an authorized test SQL Server, privately supply the process environment variable
`ORDER_MAPPING_SQL_TEST_CONNECTION`, then explicitly opt in:

```powershell
dotnet run --project tools/OrderMappingChecks/OrderMappingChecks.csproj --no-build -- --sql
```

No SQL runs without both that variable and exactly `--sql`. Do not put connection
strings or credentials in checked-in files or shell history. The login needs
permission to create and remove a temporary database.

The supplied catalog is replaced by `master` and a generated exact
`HyperOrderMappingChecks_<32 lowercase hex digits>` name. Only an acknowledged
creation marks the fixture as owned. The real EF model, including its production
collations, keys and relationships, creates the schema; only migration exclusions
are lifted. A receipt trigger injects a controlled failure inside that isolated
database. Every concurrent operation is awaited, SQL commands have timeouts, and
cleanup removes only the exact owned fixture. A create failure with ambiguous
ownership leaves any possible fixture for manual inspection; cleanup failure is a
nonzero exit. Output never includes credentials, SQL exception text or payloads.

Assertions cover numeric Applied/Duplicate ACKs; pending/rejected/timeout/malformed
ACKs; hold consumption before receipt failure and fresh-context Duplicate repair;
scope/identity/parcel preflight before accounting/reservation calls; scope checking
again at receipt time; distinct connections; concurrent identical and conflicting
ACKs; invalid or conflicting existing invoices; null invoice fill; trusted parcel
and status preservation; raw webhook parcel remaining null; unrelated tracked
changes not flushed; and absence of an Integration transaction across the controlled
accounting call.

These checks establish Integration receipt persistence and orchestration against
controlled owner responses. They do not establish native accounting exactly-once
effects, durable reservation behavior, trusted parcel membership, deployed schema
migration success or live provider acceptance. No historical completed-job backfill
or automatic replay of previously completed webhooks is exercised.
