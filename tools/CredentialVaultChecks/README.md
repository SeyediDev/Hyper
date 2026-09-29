# Credential vault checks

```powershell
dotnet build tools/CredentialVaultChecks --artifacts-path .artifacts/int002-tests --nologo -m:1
dotnet .artifacts/int002-tests/bin/CredentialVaultChecks/debug/CredentialVaultChecks.dll --pure
# Set CREDENTIAL_VAULT_SQL_TEST_CONNECTION securely before the SQL mode.
dotnet .artifacts/int002-tests/bin/CredentialVaultChecks/debug/CredentialVaultChecks.dll --sql
```

`--pure` uses only synthetic credentials, intercepted/no-network HTTP handlers,
and fresh GUID-named temporary key directories. It verifies exact whole-document
round trips, UTF-8 bounds, strict/explicit legacy behavior, invalid scope/key/
version/tamper, protected bearer/HMAC authentication, independent shared-key
providers, existing OAuth token purpose compatibility, separate timed state,
and the actual shared host key-encryption hook. On Windows it additionally
exercises DPAPI current-user encryption and old-format fixture key compatibility.
Certificate private-key configuration is not exercised by this fixture.

`--sql` also runs the pure checks, then creates and removes only this process's
fresh `HyperCredentialChecks_<guid>` database. Cleanup requires an acknowledged
CREATE and the exact generated name. The supplied connection's original database
is never used. The production Integration model keeps its constraints and
collations; the fixture only enables its DDL creation. SQL cases verify guarded
same-secret migration, binary CAS conflicts, scope changes, idempotence, tracked
edit isolation, encrypted registry/OAuth writes, reconnect preservation,
malformed-storage refusal, and registration using the original secret before
any potentially refreshing token lookup.

This is fixture evidence, not a live key-ring/configuration inventory, migration
of an existing database, OAuth consent, provider registration or native accounting
acceptance. Test output never contains credential documents, ciphertext, provider
bodies or key contents. `FixtureProtection.cs` is explicitly test-only; old
source-linked regression fixtures deliberately use its named legacy reader while
this suite verifies the strict production default and protected paths.
