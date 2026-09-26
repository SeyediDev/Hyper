# Accounting service authentication

`Hyperyek.Accounting.Host` composes JWT authentication through Neo (not Neo.Bpms).
It requires explicit Keycloak configuration, HTTPS discovery, audience/lifetime/
signature validation and an allow-listed service client carrying the exact
accounting scope. Memory/development signing credentials are not accepted.
This is a trusted internal service boundary with cross-shop authority, not an
end-user or merchant-login endpoint.

Example **host** settings (replace placeholders through deployment configuration):

```json
{
  "IdpSetting": {
    "TokenProvider": "Keycloak",
    "Authority": "https://idp.example/realms/hyper",
    "ClientId": "hyperyek-accounting"
  },
  "AccountingApiSecurity": {
    "AllowedClientIds": ["hyper-integration-service"],
    "RequiredScope": "hyperyek.accounting"
  }
}
```

Here Neo's `IdpSetting:ClientId` means the **API token audience**, not the calling
service's client ID. The identity provider must issue an access token containing
that audience, `azp` or `client_id` equal to an allowed caller, and the required
space-delimited `scope`. Conflicting client claims are rejected. The service
client must be confidential, limited to client-credentials grants, and must not
grant this scope to merchant or interactive-user clients. This implementation
does not provision or modify the identity provider.

Example **Integration Worker/AdminPanel** settings:

```json
{
  "HyperyekAccountingApi": {
    "BaseAddress": "https://accounting.example/",
    "TokenEndpoint": "https://idp.example/realms/hyper/protocol/openid-connect/token",
    "ClientId": "hyper-integration-service",
    "Scope": "hyperyek.accounting"
  }
}
```

Supply `HyperyekAccountingApi__ClientSecret` through secret storage/environment;
never commit it. Configure each actual caller separately. URLs must be absolute
HTTPS without credentials, query or fragment; the API base URL ends with `/`.
Certificate validation stays enabled and redirects are disabled for both token
and accounting requests. Neither upstream error bodies nor token responses are
included in application exceptions. Missing setup does not break unrelated panel
controller construction: validation fails closed when the service is actually
called. The standalone accounting host rejects invalid security setup at startup.

All five accounting ports share the same scoped HTTP adapter and dedicated token
provider. Concurrent acquisition is serialized; cached tokens refresh before
expiry. The provider does not require HttpContext and does not share Neo's admin
credential cache or merchant Basalam tokens. Configuration is process-scoped:
restart callers after credential rotation. A 401 invalidates the cached token but
does not automatically resend a command. The durable queue owns retries using the
existing event/order identity. A 403 remains an authorization/configuration error.

## Verification and activation

`tools/AccountingSecurityChecks` runs the real Neo-based JWT middleware on an
ephemeral loopback listener with locally generated signing keys and static test
metadata. It also exercises the real authenticated HTTP client registration with
controlled token/API transports. It does not contact the real IdP or write
accounting/Basalam data. HTTP loopback is only the test listener; production URLs
and discovery still require HTTPS.

Verified on 2026-09-26: all 32 local JWT/client assertions passed. They include
401/403/200 policy outcomes, tampered/expired/wrong-issuer/wrong-audience tokens,
exact scopes and client IDs, concurrent token caching, invalid/expired token
responses, deferred panel activation and production transport redirect/TLS
settings. The check project and its accounting host reference built with zero
warnings/errors. Package vulnerability auditing was not part of this verification
(`NuGetAudit=false` was supplied for the local build).

Before activation: provision the service client/audience/scope in the trusted IdP,
configure secrets and HTTPS endpoints, verify anonymous=401, unauthorized=403 and
authorized read=200 using the actual host, then verify a controlled idempotent
command with explicit business approval. No deployment or production settings
are changed here. POS/stock-source gates remain disabled until their separate
acceptance. Transitive legacy Core/Neo.Bpms removal remains separate work.
