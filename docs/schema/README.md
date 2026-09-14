# Database-owned Hyperyek model

Captured 2026-09-10 from the configured local Hyperyek SQL Server: 154 tables, 3 views, 2049 columns, 230 foreign keys. `hyper-schema.json` contains schema metadata only, no customer rows or connection credentials.

`SqlServerEntity` implements Neo `IEntity` and `IDomainEventEntity` without inventing a primary key. `SqlServerEntity<TKey>` additionally implements `IEntity<TKey>` only for actual single keys named Id in the CLR mapping. Tables with composite or differently named keys retain those properties. Two tables have no primary key and remain keyless; all three views use ToView/HasNoKey. Generic Neo repositories requiring a single Id must not be used for keyless/composite records; use the explicit accounting context.

`HyperSqlServerContext` is the database-owned accounting baseline. `HyperIntegrationContext` explicitly contains eleven integration entities (106 columns); the integration controller and synchronization service use it instead of the copied Club context. These are separate EF models over overlapping SQL tables, never one combined context. Legacy contexts and features elsewhere still require cleanup.

Generation preserves column store types, nullability, length, numeric precision/scale, real PK order/names, identity seed/increment, default SQL/names, computed SQL, indexes and foreign keys. Existing tables are deliberately excluded from migrations. Zero migration operations therefore verifies protection against DDL; it is not proof that the legacy application migration snapshot equals the database. Snapshot parity and physical schema comparison must be treated separately.

SQL unique constraints permitting NULL cannot be represented as EF alternate keys without changing nullability. Five such/keyless constraints are represented as unique indexes with `Hyper:DatabaseUniqueConstraint` annotation. The database constraint remains unchanged. Column collations are mapped. Full DDL fidelity (including all index options and this constraint distinction) is not claimed. The schema snapshot preserves this evidence. Integration table defaults for IsEnabled/IsActive use a true sentinel, so explicitly setting false is not lost to a database true default.

## Reproduce

From Backend with PowerShell and Python:

```powershell
./scripts/export_sqlserver_schema.ps1 -OutputPath ../docs/schema/hyper-schema.json
python scripts/generate_sqlserver_model.py ../docs/schema/hyper-schema.json
$env:NUGET_PACKAGES = Join-Path $env:USERPROFILE '.nuget/packages'
dotnet build tests/Hyper.SqlServerModel.Checks/Hyper.SqlServerModel.Checks.csproj -m:1 -nr:false -p:UseSharedCompilation=false
dotnet tests/Hyper.SqlServerModel.Checks/bin/Debug/net10.0/Hyper.SqlServerModel.Checks.dll
```

The checks use cached EF SQL Server 10.0.8 and compile actual Neo contracts, generated model and integration production sources. They do not build the whole application.

Optional read-only materialization check against the configured database:

```powershell
dotnet tests/Hyper.SqlServerModel.Checks/bin/Debug/net10.0/Hyper.SqlServerModel.Checks.dll --live src/CustomerPortal/Hyper.CustomerPortal.Api/appsettings.json
```

When Windows sandbox TLS is unavailable, `--live-local` is an explicit local-only shared-memory fallback with optional encryption. It accepts only '.', '(local)' or 'localhost', never a remote server. Application settings are not modified. On 2026-09-10 the local mode read all 153 objects successfully; 113 yielded a row. No customer values or credentials were logged and no writes occurred.
