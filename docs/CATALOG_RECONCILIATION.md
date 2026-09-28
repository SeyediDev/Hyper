# Product and inventory reconciliation

This is the main **existing mapped product** path. It does not create products,
guess mappings, change fiscal units, write SKU to Basalam, or enable stock capture.
For explicit creation of a separate unpublished Basalam product, use the
[product-draft workflow](PRODUCT_DRAFT_CREATION.md), not reconciliation.
Integration reads accounting through `IIntegrationPlatformCatalogPort`, queues
typed Outbox messages, and publishes through the provider adapter. No accounting
tables or Neo.Bpms services are used by this orchestration.

## Start and inspect a pair

`POST /api/integrations/v1/connections/{connectionId}/catalog/reconcile`

The normal authenticated Integration shop/tenant claim is required. Headers or
the body alone are not authorization. Use a new UUID for a new requested run:

```json
{
  "shopId": 7,
  "tenantId": "tenant-a",
  "connectionId": 41,
  "requestId": "ae475351-42f0-4a2a-967f-cf78be311c64"
}
```

The request atomically creates a Product and an Inventory job with stable event
identities. HTTP 202 means the request was accepted, **not** synchronized. Repeat
the same request to read the same two job IDs, status, error and completion time.
It does not restart a completed/failed job. Use a new request ID only when you
intend a new reconciliation after reviewing failures or intervening changes.
Invalid request/route identity returns 400, foreign connection 404, and unavailable
connection/queue 409. Authentication/authorization retains 401/403 behavior.

The existing `/sync` endpoint remains catalog snapshot discovery; its RunId is
not a reconciliation job ID. The panel's Product/Inventory scenario form also
uses the repaired worker path. Manual, Initial, Periodic and StoreChanged product
jobs without an Inbox read accounting; an actual product webhook still follows
the inbound accounting-command path. BoothChanged without Inbox fails visibly.
This change does not add a periodic scheduler: a Periodic job must still be
explicitly enqueued by an existing caller.

## Source and fields

- The capture-owned mode (`InventoryCapture=2`, unchanged numeric contract) now
  permits accounting-catalog title/base-price reconciliation and stock capture
  on the same mapping. They allocate one increasing version stream under the
  mapping lock. There is no new independent version generator or schema migration.
- Accounting event mode (`AccountingEvents=1`) still accepts product-changed and
  inventory-changed with externally coordinated versions. Manual capture never
  steals its stream. Choose/switch source explicitly using the existing guarded
  version-source API; drain pending work first.
- Title and base price come from a fresh scoped accounting API read, not the
  caller or webhook body. Zero price is explicit; no currency conversion occurs.
- Inventory is `max(0, gross accounting stock - active Integration holds)`;
  non-sellable products publish zero. The existing
  `IntegrationInventoryCapture:AccountingStockSourceVerified` gate is unchanged.
  Product-only reconciliation does not require that stock assumption.
- Unknown remote price/stock is not treated as equality. A supported authoritative
  value can be sent and subsequently read back. Missing mappings/products, SKU
  mismatch and unsupported variant details remain actionable differences. Variant
  stock continues through the existing inventory adapter; parent details are
  never used as a substitute for variant details.

## Delivery and completion

The scenario persists Outbox message IDs in its existing ResultJson. While the
independent Outbox is pending/retrying, the job remains Pending with
`AwaitingOutboxDelivery`, polling at five-second intervals without consuming its
failure budget or creating another message. Existing shop FIFO applies during
this wait; other shops and the Outbox lane continue independently.

After all messages finish, the worker rereads both sources. Completed requires
no remaining differences and no failed delivery. A provider ACK alone is not
enough. An unreflected write, failed/deleted delivery or unsupported difference
ends NeedsAttention. A transient read-back failure retains the persisted message
IDs for retry. Failed sends are not automatically resurrected by read-back.
This is at-least-once delivery, not a distributed transaction or exactly-once
claim. If the process dies between enqueue and saving its checkpoint, existing
pending identical payloads are reused; an already-delivered absolute operation
may be repeated if the remote snapshot still differs.

## Verification and live prerequisites

`dotnet run --project tools/CatalogReconciliationChecks` uses only a uniquely named
`HyperCatalogChecks_<guid>` SQL database and controlled provider/accounting ports.
Set `SYNC_CHECKS_CONNECTION` securely for an authorized local fixture SQL Server.
The tool removes only its exact generated database. It covers queue/capture/
Outbox/read-back, fresh contexts, repeated starts, source ownership, zero values,
active holds, explicit unsupported cases and retained checkpoints. It does not
prove live SDK delivery, deployed host authentication or the native accounting
writer; existing SynchronizationFlowChecks and EndpointIsolationChecks cover
their respective controlled HTTP/authorization boundaries.

Before a real run: valid booth grant with read/write scopes in Integration DB,
enabled scoped connection, approved product mapping, correct field/unit policy,
and a verified stock source are required. Inbound accounting product changes also
require accounting-owned `AccountingProductChangeReceipts`. Native writer,
stock semantics and deployment sign-off remain WorkManagement TODO128. Never
enable the stock flag merely to make a test pass. Stop/drain all old scenario
processors before deploying the shop-lock implementation.
