# CMD-101: durable application of external product changes

`POST /api/hyperyek/v1/accounting/products/external-changed` remains the
platform-owned accounting boundary. Integration resolves an active product
mapping and reads the provider catalog; the accounting handler receives the
trusted internal product ID. It does not query Integration tables or create a
product/mapping when one is missing. Existing host authentication is unchanged.

## Applied fields and scope

- A positive shop, connection, internal product and source version are required,
  with a bounded tenant, event ID and external product ID.
- The shop and product must have the exact canonical tenant. Legacy tenantless
  shops use `shop:{shopId}`, consistently with the accounting read API.
- Title is required, nonblank, at most 200 characters. Null SKU/price mean
  **preserve**, not clear. Explicit zero price is supported.
- The existing SKU destination is `TAXCODE_ varchar(13)`. Values that would be
  truncated or require a code-page conversion are rejected for review; this does
  not establish a new SKU/barcode/tax-code business mapping.
- Price must fit `decimal(26,8)` without rounding and be nonnegative. No currency
  conversion or fiscal policy is introduced.
- Variant-level changes are explicitly unsupported by this product-level writer;
  they cannot silently mutate the parent. Creation policy remains separate.
- Marketplace inventory is only an observation. It is never assigned to
  `ACCOUNTINGSTOCK_`, and the result never claims a stock movement was committed.

## Atomic receipt and ordering

The accounting database owns `dbo.AccountingProductChangeReceipts`. This is not
Integration inbox/outbox storage: the receipt must commit in the **same local
transaction** as the legacy product update. No distributed transaction, legacy
table alteration, new Neo.Bpms dependency or new database is introduced.

The existing accounting shop transaction lock serializes changes. Hashes use
length-safe JSON encoding and preserve the case of event/tenant identities:

- Event identity is `(shop, tenant, connection, eventId)`.
- Version stream is `(shop, tenant, connection, internalProductId)`.
- Fingerprint includes target identity, fields and source version; numerically
  equal decimal representations compare equally.

An exact replay, including after a process restart or lost HTTP acknowledgment,
returns `Duplicate` with the original product reference without reapplying it.
An older exact replay cannot revert a newer product state. Reusing an event for
another payload/product returns `AccountingProductEventConflict`. Reusing a
version with a new event returns `AccountingProductVersionConflict`; an unseen
older version returns `StaleAccountingProductVersion`. Connections have separate
version namespaces, not a fabricated global order across providers.

The existing HTTP status convention remains: Applied=200, Duplicate=409,
PendingDependency=202 and Rejected=422. Duplicate is a recognized accounting
acknowledgment, not a request to apply again. If a retry re-reads a different
provider snapshot under an already-applied event identity, it is a visible
conflict, not silently accepted as the same command.

## Deployment

Run `docs/schema/ensure-accounting-product-receipts.sql` on the accounting host's
`Domain` / `DomainCommandConnection` database, **not** IntegrationConnection.
The script requires TBL_Product and TBL_Shop and is repeatable. It does not seed
guessed receipts or change business rows. A missing receipt table produces
`PendingDependency / AccountingProductReceiptSchemaMissing` before any mutation.

Previously applied commands have no reconstructable version history. Pause old
writers, review/drain queued old deliveries, then deploy the new handler and
schema together. Do not claim retroactive deduplication or delete receipts to
reset versions. Native accounting edits remain a separate producer concern.

## Verification

`tools/AccountingOrderChecks` now exercises the real product handler and receipt
schema alongside the existing order checks. Configure `ACCOUNTING_TEST_SQL` for
an authorized local test SQL Server and run the project. It creates only a GUID
named `HyperAccountingChecks_...` fixture and removes that exact fixture.

Product checks cover schema-missing behavior, repeated schema application,
absent optional fields, explicit zero, scope/column validation, stale/conflicting
versions, lost-ACK replay from fresh contexts, competing writers and forced
receipt-insert failure proving rollback of the preceding product update.
The injected constraint exists only in the validated disposable database.

This is accounting SQL/handler acceptance, not proof of live Basalam delivery,
host authentication middleware, the native accounting producer, price ownership,
or a complete production accounting schema. INT-006 and live acceptance remain
separate work.

Verified 2026-09-27: the complete AccountingOrderChecks executable passed all 95
SQL assertions, including the new product receipt checks, and removed its exact
disposable fixture. The accounting infrastructure and test executable targeted
builds succeeded with existing dependency outputs; this is not a full build.

The user confirmed that the native writer source is unavailable until later
coordination with the platform manager. The canonical approval TODO is database
WorkManagement item **128 / INT-006.PLATFORM-APPROVAL**, a child of INT-006.
Continue independent implementation/tests while that external sign-off is pending.
It covers the native transaction hook, authoritative stock/reservation semantics,
one version allocator, origin markers, field/unit ownership, receipt deployment
and controlled live acceptance. No legacy writer or live switch was enabled here.
