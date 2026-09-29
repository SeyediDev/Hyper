# Controlled credential rewrap

This is an operator-invoked, single-row storage migration using the production
credential vault and binary compare-and-swap service. It does not start a host,
call a provider, create a database or change the webhook secret. See
[the deployment guide](../../docs/INTEGRATION_CREDENTIAL_VAULT.md) before use.

Build with `dotnet build tools/CredentialVaultMaintenance/CredentialVaultMaintenance.csproj`.
Privately supply `INTEGRATION_CREDENTIAL_MIGRATION_SQL` for the authorized existing
Integration database and all required `IntegrationProtection__*` environment
settings. Never supply secrets through command arguments or checked-in files.

The following is an invocation template; replace the nonsensitive identifiers
with the selected row and exact database name after verifying scope:

```powershell
dotnet run --no-build --project tools/CredentialVaultMaintenance/CredentialVaultMaintenance.csproj -- --apply --database DATABASE --connection-id 123 --shop-id 45 --tenant TENANT
```

The supplied connection's catalog must match `--database` exactly and may not be a
system database. The transaction changes only `CredentialsJson`, only when the
connection's ID/shop/tenant/provider/credential type/account and original document
still match. String CAS uses binary bytes plus byte length, so case and trailing
space edits conflict. Existing protected rows are verified without rewrapping;
repeating a completed plan is idempotent. Conflict exits with code 3; invalid
arguments/missing connection exit with 2; other failures exit with 1. Output is
restricted to row ID/result or sanitized failure codes. Keep key material and
database backups under the deployment's existing protected backup policy.
