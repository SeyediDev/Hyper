# SEC-101 security regression checks

The executable compiles the production webhook verifier, ingress, scope
authorizer and Integration persistence mappings directly. Project references
build the current Domain and Contracts sources; no prebuilt DLLs, panel host,
workers or live marketplace calls are used.

From the repository root:

```powershell
dotnet run --project tools/SecurityChecks --artifacts-path .artifacts/security
```

For SQL coverage, supply `SECURITY_TEST_SQL` through the process environment,
using a local test SQL Server login that may create/drop disposable databases:

```powershell
dotnet run --project tools/SecurityChecks --artifacts-path .artifacts/security -- --sql
```

The tool generates `HyperSecurityChecks_<guid>` and validates that exact name
before removing its database in `finally`. Existing application databases are
not test targets. Never commit or print a connection string. SQL errors or failed
assertions return a nonzero exit status; a run without `--sql` explicitly reports
that SQL checks were skipped.

Coverage:

- Independently generated custom HMAC; body, event ID, type, timestamp and
  connection binding; signed timestamps at and outside the +/-300 second limit;
  malformed signatures, invalid headers and disabled connections.
- Basalam's configured bearer credential: valid/wrong values, empty/whitespace
  secrets and malformed/non-object credential JSON. The custom HMAC protocol is
  not asserted to be Basalam's delivery protocol.
- Safe source extraction and rejection of non-object webhook bodies before
  database access (a connection interceptor fails any unexpected SQL attempt).
- Authenticated shop/tenant claim matching, denial of mismatched/wildcard or
  unauthenticated claims, and no implicit arbitrary scope for an Admin role.
- Credential fields excluded from connection/token DTOs.
- SQL inbox/audit/job creation, exact and changed replay, competing duplicate
  deliveries, per-connection event identity, routing a shared account through
  its verified credential, rejection of ambiguous Basalam credentials and
  loopback auditing without a new business job.
- Audit payload SHA-256 is correct and the fixture's private body and credential
  are absent from the serialized audit entity.

Limits: the SQL schema is generated from current production EF mappings, not a
deployment/migration rehearsal. Claims use controlled identities; this does not
certify production issuers, gateway configuration, every HTTP endpoint or all
telemetry/log sinks. Raw payload storage in the inbox is intentional and is not
claimed to be redacted. Basalam delivery and full accounting E2E require separate
acceptance. SEC-101 should remain in Review until those broader requirements
have been assessed.

Regression baseline on `d12fee6`: 44 non-SQL checks passed and 11 failed.
The failures cover non-object source/credential JSON and empty Basalam secrets.

Verified after the fixes on 2026-09-26:

- `dotnet build --no-restore tools/SecurityChecks/SecurityChecks.csproj --artifacts-path .artifacts/security -m:1 -v:minimal`
  exited 0 with no warnings or errors (targeted harness build only).
- `dotnet .artifacts/security/bin/SecurityChecks/debug/SecurityChecks.dll --sql`
  exited 0: **66 passed, 0 failed** (55 non-SQL and 11 SQL checks).
- The disposable SQL database was removed by the fixture cleanup.
