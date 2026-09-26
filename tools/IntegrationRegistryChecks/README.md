# Integration registry tenant regression checks

Builds the production registry, OAuth store/service, EF context and configurations
from their current source files. It uses SQL Server rather than an in-memory EF
provider so unique constraints, schema upgrades, transactions and rollback are
actually exercised. Unrelated legacy hosts are excluded from the build graph.

From `Backend`:

```powershell
dotnet build tools/IntegrationRegistryChecks/IntegrationRegistryChecks.csproj --disable-build-servers -m:1
dotnet run --project tools/IntegrationRegistryChecks/IntegrationRegistryChecks.csproj --no-build -- --settings src/AdminPanel/Hyper.AdminPanel.Web/appsettings.json
```

Alternatively set `REGISTRY_CHECKS_CONNECTION` via your local secret mechanism.
Do not commit connection strings. The login needs permission to create and drop
a database on the configured test SQL Server. The configured database is never
opened: this tool creates `HyperRegistryChecks_<random GUID>`, operates only on
that database, and drops that exact database in `finally`. Run on a development
SQL Server. No external HTTP request is permitted.

Coverage: fresh schema, legacy connection/token keys upgraded using both scripts,
repeatable migrations preserving existing tokens, same-vendor connections across
tenants, same-tenant duplicate rejection at the API and database, tenant-isolated
OAuth callbacks/renewals, rejection and rollback of a second vendor in the same
scope, list/disable ownership and credential-free registry responses.

The standalone domain checks do not exercise this persistence path. A successful
run of this verifier is also not a full solution build or a live Basalam OAuth test.

## Verification evidence — 2026-09-26

- Fresh verifier build: 0 warnings, 0 errors.
- SQL Server run: 19 checks passed; the disposable database was removed.
- The first SQL run timed out during full-schema upgrade at 120 seconds. The
  verifier now uses the provisioner's 180-second limit and emits scoped wait
  diagnostics after 20 seconds. The subsequent complete run passed.
- Full Infrastructure build was attempted separately. It failed in the existing
  dispatcher/dashboard paths (`IntegrationReservationIdentity`,
  `IntegrationRecentWebhook`, and missing dashboard snapshot properties).
- After the concurrent repository updates, the full build was attempted again:
  0 warnings and 2 errors in `IntegrationBusinessEventDispatcher.cs` (162-163):
  `BusinessCommandResult.StockCommitted` and
  `IIntegrationInventoryReservation.CommitAsync` were missing from the consumed
  contracts. Registry/OAuth source verification remains green independently.
- No application database was migrated and no live Basalam request was made.
