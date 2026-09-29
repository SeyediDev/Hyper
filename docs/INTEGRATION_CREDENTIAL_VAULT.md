# Integration credential protection and deployment

`IntegrationCredentialVault` protects the complete `CredentialsJson` document with
ASP.NET Core Data Protection. The stored format starts with
`hyper-credentials:v1:`. Unknown versions, corrupt ciphertext and wrong connection
scope fail closed. Purpose segments bind connection ID, shop ID, exact tenant and
provider. The secret, signature scheme and unknown JSON properties are preserved;
the migration preserves the exact original JSON bytes represented by its string.
JSON must be an object with unique top-level property names and at most 65,536
UTF-8 bytes. A present invalid `webhookSecret` is an error; only an absent field in
a valid document permits initial secret generation during OAuth save.

## Required shared configuration

Every panel, API and worker that reads stored Integration credentials/tokens must
receive the same effective protection identity. `AddHyperIntegrations` registers a
dedicated `IIntegrationProtectionProvider`; it does not replace the host's ordinary
provider used for cookies, simulation tickets or timed OAuth state.

| Setting / environment variable suffix | Required value |
| --- | --- |
| `IntegrationProtection:KeyRingPath` / `IntegrationProtection__KeyRingPath` | Existing absolute protected key directory, readable/writable by the intended service identities. Missing directory fails; no implicit unrelated ring is selected. |
| `IntegrationProtection:ApplicationName` / `IntegrationProtection__ApplicationName` | Exact historical application discriminator used to protect existing Integration OAuth tokens. Match across consumers. |
| `IntegrationProtection:KeyEncryption` / `IntegrationProtection__KeyEncryption` | Explicit `DpapiCurrentUser`, `DpapiLocalMachine` or `Certificate`. There is no unencrypted or ephemeral fallback. |
| `IntegrationProtection:CertificateThumbprint` / `IntegrationProtection__CertificateThumbprint` | Required for `Certificate`; install the matching certificate/private key where each service account can access it. |
| `IntegrationProtection:AllowLegacyPlaintextRead` / `IntegrationProtection__AllowLegacyPlaintextRead` | Defaults to `false`. Set `true` only for an explicitly managed temporary legacy-reader window, then disable after verification. |

The known Development panel identity is its existing
`<AdminPanel ContentRoot>/DataProtection-keys` directory and application name
`Hyper.AdminPanel`. Resolve that directory to an absolute path in each host's
configuration; do not create a new worker identity or rename the panel identity.
Production key identity must come from that deployment's actual existing provider;
the Development pair is not a production default. No settings files, key material,
running hosts or existing rows are changed merely by integrating this code.

`DpapiCurrentUser` requires the same Windows account/profile for decryption;
`DpapiLocalMachine` requires the same machine and appropriate directory ACLs.
For cross-machine consumers, use a shared protected ring and accessible certificate
private key according to the deployment's security policy. Keep keys/certificates
and required historical key material in protected backups. Do not delete expired
Data Protection keys or old decryption certificates while stored ciphertext still
depends on them.

Certificate mode validates that the framework-selected certificate has an
accessible RSA private key using an in-memory roundtrip probe before configuring
new-key encryption. This does not replace verification of every service account,
future certificate renewal or retained historical decryption certificates.

Selecting a file-system ring disables automatic key encryption; this composition
therefore requires an explicit encryption mode. That setting applies to newly
generated keys and does **not** retroactively encrypt historical plaintext XML key
files. Verify historical key-at-rest protection and service-account access through
the deployment process, preserving compatibility. The runtime does not silently
move or rewrite historical keys. See [Microsoft's Data Protection configuration
guidance](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-10.0).

Every writer sharing a ring must use the same key-encryption policy. When the
configured Integration directory matches the Development panel's existing ring,
the panel composition applies that encryption policy to its ordinary provider too,
without changing its ring or `Hyper.AdminPanel` discriminator. With no Integration
configuration, the panel retains its previous behavior; this is not a completed
vault rollout. For other production host/ring writers, verify equivalent policy
explicitly rather than assuming the dedicated provider controls another writer.

## Writers and readers

OAuth connection creation saves an empty `{}` placeholder only inside its existing
uncommitted transaction to obtain the ID. It then protects the secret document
before commit; plaintext secrets are never flushed. Reconnect reads structured
credentials, retains the existing secret and unknown properties, and protects the
document again. Corrupt/unreadable documents roll back; they never trigger an
implicit secret rotation. Generic management creation likewise commits a protected
empty document and exposes no credential field in request/response DTOs.

OAuth access/refresh tokens retain the existing `Basalam.OAuth.Token` purpose and
now use the dedicated compatible provider. OAuth state retains the existing host
provider and `Hyper.Basalam.Authorization.v2` purpose. Existing token refresh locking,
rotation, omitted refresh/scope preservation and `RawTokenResponse = null` behavior
remain in place.

Webhook verification reads through the vault without database writes; invalid
credentials yield an invalid signature result for that candidate. Registration
validates the stored ordinal scope and reads its secret **before** token retrieval,
which might refresh remotely. It sends the same bearer secret used previously.
No new secret/credential output is added to frontend/API contracts. Never include
plaintext, ciphertext, bearer headers, provider response bodies or key files in
logs, diagnostics or evidence.

## Controlled same-secret migration

1. Inventory selected deployment rows by ID/classification/count only, and confirm
   the historical ring/application identity plus service-account/key access. Keep
   database backups according to the existing protected operational policy.
2. Deploy compatible readers and explicit protection settings to every consumer
   before producing protected writes. If old plaintext rows must remain available
   during rollout, explicitly enable the temporary legacy-read setting. Even then,
   protected-prefix failures never fall back to plaintext.
3. Build and explicitly invoke the single-row
   [CredentialVaultMaintenance tool](../tools/CredentialVaultMaintenance/README.md)
   for each selected authorized row. It verifies the requested database and scope,
   protects the whole original document and changes only `CredentialsJson` in a
   transaction. Binary bytes **and** lengths are compared for tenant/account/old
   document because SQL's existing CI collation ignores some text differences.
   Concurrent case/trailing-space edits conflict; no caller's tracked edits are
   flushed. Output is ID/result only, or a sanitized failure code.
4. Verify classifications again and prove cross-host decryption of stored OAuth
   tokens and protected credentials. Disable the legacy-read setting when no
   remaining required plaintext rows exist. Reinvocation of a protected row
   validates it and is idempotent; it does not rotate or re-register anything.

The service is not a startup/schema migration and is never called by verifier or
ingress. Storage rewrap preserves the same secret, requiring no provider request
or renewed consent. Changing the actual webhook secret, revoking grants, changing
protection identity or deleting keys is a separate coordinated action.

Code/fixture acceptance, actual shared-key compatibility and actual row migration
are separate outcomes. An enabled legacy capability does not itself prove rows
remain plaintext; disabled compatibility does not prove all deployments migrated.
Report the measured state rather than declaring deployed encryption from a build.

## Targeted verification

`tools/CredentialVaultChecks` covers the codec, independent compatible key providers,
existing OAuth-purpose compatibility, unchanged timed state, protected bearer/HMAC
verification and controlled SQL writer/migration cases. Existing OAuth, registry,
security and synchronization tools retain their targeted regressions. Test rings,
databases and controlled responses are isolated; they establish no live provider
consent, production key ACL/backup acceptance or automatic deployment/migration.
