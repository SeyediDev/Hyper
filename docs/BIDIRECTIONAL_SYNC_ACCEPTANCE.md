# Bidirectional Hyperyek / Basalam acceptance

## SDK transport regression checks

Run `dotnet run --project tools/BasalamTransportChecks` from the Backend Git root.
The checks exercise the production SDK with an in-memory HTTP handler: loading,
refreshing and clearing a connection token must affect child services, distinct
clients must not share booth credentials, and root/section/explicit configuration
must all reach the SDK. Production relative routes resolve to HTTPS and stock zero
is sent as an explicit absolute PATCH. No real provider or business database is
called by this tool.

`BasalamClient.SetToken` now forwards to the shared transport used by its catalog,
product and other services. `AddBasalamSdk` accepts either root configuration or
the already-selected `Basalam` section and preserves an explicitly supplied
`BasalamConfig` instance. Public method signatures and MCP contracts are unchanged.

## SQL-backed two-way flow checks

Run `dotnet run --project tools/SynchronizationFlowChecks`. The default is local
SQL Server with Windows authentication; optionally set `SYNC_CHECKS_CONNECTION`.
The tool creates a uniquely named `HyperSyncChecks_<guid>` database, runs the
production Integration persistence, verifier, queue, dispatcher, accounting HTTP
client, token store, Outbox and Basalam SDK/adapter, then drops only that database.
It never opens the business or configured Integration database. The SQL login
must be allowed to create/drop this isolated fixture. Both HTTP destinations are
intercepted in memory; accounting responses are fixtures, not financial documents.

The 27 checks cover normalized product webhook -> accounting command, accounting
inventory event -> authenticated stock PATCH, connection/tenant selection,
replay/conflicting content, missing mapping, retry, explicit-source loopback,
unknown events, version ordering, zero stock and persisted outcomes. No accounting
tables exist in the fixture Integration database. This does not verify the separate
legacy inventory capture/reconciliation path, actual Basalam payload shape, real
HTTP authentication middleware, accounting SQL writes or public webhook delivery.

Ingress accepts only object JSON and event IDs up to the worker's 128-character
limit. A reused event ID with different type/body is invalid (`EventIdentityConflict`).
Where a vendor ID belongs to multiple scoped connections, exactly one connection
must authenticate the delivery; ambiguous credentials fail closed. Unsupported
events are retained with `UnsupportedEventType`; explicit Hyperyek echoes are
acknowledged without a job. Worker completion atomically updates the job and its
Inbox outcome: pending/retry=0, processed=1, terminal failure/needs attention=2.

`POST /api/integrations/v1/accounting/events/inventory-changed` requires an
authenticated principal and the existing shop/tenant scope authorization. A body
containing `ShopId`/`TenantId` is not authorization. Producers must obtain a valid
scope from the host's trusted issuer; do not forge tokens to exercise this route.

## Real-provider prerequisites (not replaced by fixture tests)

The selected integration database must contain the real shop/tenant connection,
an OAuth grant obtained through the panel's existing Basalam login, and explicit
product/variant mappings. Application ClientId/ClientSecret alone are not a booth
grant. Never paste those credentials into chat or task logs.

The configured OAuth scope list must allow both reading products and modifying
them (`vendor.product.read`, `vendor.product.write`); request only the additional
permissions needed by the selected scenario. These names are defined by the
[official Basalam SDK](https://github.com/basalam/python-sdk/blob/main/src/basalam_sdk/auth.py).
Changing requested scopes does not upgrade an existing grant: the booth owner
must authorize the updated request. Confirm the granted scopes before publishing.

Select an explicitly authorized test product and expected absolute inventory
before any live write. Do not enable broad capture/workers, alter arbitrary
prices/stock, or create customer orders simply to prove connectivity. Record
separately the SQL/HTTP fixture results and actual Basalam/accounting outcomes.
