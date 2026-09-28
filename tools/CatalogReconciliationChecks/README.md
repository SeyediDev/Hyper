# Mapped product and inventory main-path checks

Run `dotnet run --project tools/CatalogReconciliationChecks` from Backend.
Configure `SYNC_CHECKS_CONNECTION` through a secure process environment if the
default local integrated-security connection is not appropriate.

The fixture creates and removes only its validated `HyperCatalogChecks_<guid>`
database. Production queue, capture, Outbox, version ownership, comparison,
inbound product dispatch and combined-start service are source-linked. Provider
and accounting ports are controlled; no real stock, price, token or provider
mutation is performed. This is not live acceptance or a full solution build.

Checks cover initial/manual/periodic products without Inbox, atomic idempotent
paired start, polling beyond the error-attempt budget, acknowledged read-back,
second-job conflict rollback, ACK without a reflected provider change,
shared product/stock versions, active versus committed holds, zero price/stock,
disabled stock source, inbound product direction, unknown values, ownership
conflict, permanent delivery failure, transient read-back recovery, unsupported
SKU/variant details and missing mappings.

See [the operational contract](../../docs/CATALOG_RECONCILIATION.md).
