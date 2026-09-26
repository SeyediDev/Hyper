# SEC-204 endpoint isolation checks

This executable runs the actual Integration API controllers through ASP.NET Core
authentication, authorization, routing and JSON binding on an ephemeral loopback
port. The test-only authentication handler supplies controlled principals; it is
not registered in any production host. A service probe asserts that denied
requests never reach the application service and successful requests carry the
requested shop/tenant. Both supported claim types are exercised.

```powershell
dotnet run --project tools/EndpointIsolationChecks --artifacts-path .artifacts/isolation
```

All 14 protected actions are covered: connection list/create/enable/disable,
webhook replay, mapping list/create/deactivate, sync, token request/list/revoke,
dashboard and accounting inventory ingress. Anonymous, wrong-shop, wrong-tenant,
Admin-without-scope and mixed authenticated/unauthenticated identities must be
rejected. Positive owner cases prevent a blanket-deny implementation passing.
The sole anonymous webhook action must delegate to webhook ingress; provider
signature/replay verification is covered separately by SEC-101.

For real SQL service-boundary checks, set `ISOLATION_TEST_SQL` in the process
environment to a local SQL Server connection allowed to create/drop disposable
databases, without printing or committing the value:

```powershell
dotnet run --project tools/EndpointIsolationChecks --artifacts-path .artifacts/isolation -- --sql
```

SQL tests compile the production EF mappings and management, mapping, token,
sync, dashboard and accounting-ingress services directly. They create a unique
`HyperIsolationChecks_<guid>` database and remove exactly that database in
`finally`, with a target-name guard. Existing application data is never a test
target. Fixtures include the same shop in different tenants, different shops in
the same tenant, foreign record IDs and inconsistent imported child ownership.
External catalog, synchronization and outbox ports are probes; no provider or
accounting calls are made. Unauthorized operations must leave data unchanged.

Limits: this is a targeted build and controller/service regression suite, not a
full deployed-host or live issuer/Neo authentication test. It does not establish
who may issue `integration_scope`/`scope` claims (ACCESS-REGISTRY-001), certify
OAuth callbacks or AdminPanel UI endpoints, validate migration rollout, or change
database tenant collation/canonicalization. Deployment identity and broader
acceptance remain review items; do not mark SEC-204 Done solely from this suite.

Baseline at `f87524f`, before production fixes: the initial non-SQL suite exited
1 with **88 passed, 18 failed**, reproducing mixed-identity claim acceptance,
control characters in tenant scope and token request simulation tenant mismatch.

SQL execution in the sandbox failed at the SQL TLS connection, before test
assertions. Execution outside the sandbox was not approved. SQL coverage is
therefore **not verified**; this includes the connection-owner predicates added
to token revocation and accounting inventory ingress. These changes require
SQL review/verification before acceptance. No SQL pass count is claimed.

Verified after the fixes on 2026-09-26:

- `dotnet build --no-restore tools/EndpointIsolationChecks/EndpointIsolationChecks.csproj --artifacts-path .artifacts/isolation -m:1 -v:minimal`
  exited 0, with 0 warnings and 0 errors.
- `dotnet .artifacts/isolation/bin/EndpointIsolationChecks/debug/EndpointIsolationChecks.dll`
  exited 0: **106 passed, 0 failed**; SQL checks explicitly skipped.
- A separate read-only `sys.databases` check found no remaining database with
  the `HyperIsolationChecks_` prefix after the failed SQL attempt.
