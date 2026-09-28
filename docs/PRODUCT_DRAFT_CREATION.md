# Explicit accounting-to-Basalam product draft

This implements the first direction of task `SYNC-PRODUCT-CREATE-001`: an
explicit, scoped accounting product creates a separate Basalam **unpublished**
draft. It does not merge unrelated products, process unmapped notifications,
create an accounting product, publish a listing, or enable live stock.
Rice `60438766` and Hyper's vacuum bag are different products; do not map them.

## Deploy and call

For hold/adapt policy, durable missing-data worklist, repair UI and audited
corrections use [product readiness](PRODUCT_READINESS.md). The route below remains
the strict low-level draft API; it does not guess required metadata.

Apply [the additive Integration schema](schema/ensure-integration-product-creations.sql)
on the **Integration database**, before deploying the API/worker/mapping service.
The table is Integration-owned and does not touch accounting. No Neo.Bpms service
is used. Stop/drain old scenario workers before upgrading; item 9 is new and old
workers must not consume it. Existing shop-level FIFO processes creation jobs.
The script also expands the legacy scenario-item check constraint to 1..9.

`POST /api/integrations/v1/connections/{connectionId}/product-drafts`

Body: `shopId`, `tenantId`, `connectionId`, new UUID `requestId`, `hyperProductId`,
positive Basalam `categoryId`, nonnegative `preparationDays`, positive
`packageWeight`, optional `description` (max 10000) and positive `photoId` already
uploaded to Basalam. Category, weight, preparation and photo are explicit approved
metadata, never guessed from the accounting ID or another listing.

Normal authenticated shop/tenant scope is required. Missing/invalid data returns
400; scope denial 401/403; foreign connection 404; conflicting identity, existing
mapping/creation or unavailable source 409. HTTP202 is acceptance, not publication.
Title and integer base price are snapshotted from the scoped accounting catalog
API; they must still match before sending. No currency/unit conversion is applied.
The booth identity is pinned in the receipt; changing the connection's booth
after approval cannot silently redirect the creation.

`GET /api/integrations/v1/connections/{connectionId}/product-drafts/{requestId}`
with `X-Shop-Id` and `X-Tenant-Id` under the same authenticated scope returns
`requestId`, `jobId`, `status`, `errorCode`, `externalProductId`, `mappingId`.
Repeating POST with the identical request returns the same receipt/job. A changed
body for that request conflicts. Another UUID for the same connection/source
product also conflicts, including after a failed or uncertain send.

The new SDK capability `IProductDraftService` sends official JSON fields with
`status=3790` (unpublished), `stock=0`, `is_wholesale=false`; these are deliberate
draft values, **not** accounting stock assertions. It omits absent optional fields.
Creation requires HTTP201 and a positive returned ID. A separate GET verifies
vendor identity, parent/no-variants, name, price and zero stock before the mapping
and completed receipt commit atomically. Existing reconciliation can then update
the mapped product; activating stock still requires TODO128 sign-off. Publishing
the draft is a separate merchant action, not part of this endpoint.

## Recovery and limits

One receipt per connection/source product prevents concurrent duplicate creation.
Before POST a committed send marker is stored. Only this request disables SDK
HTTP retries, independently of global `MaxRetries`. A lost response, timeout,
rejection, malformed success, shutdown or crash after the marker yields
`CreationOutcomeUnknown`/NeedsAttention and **never** a second automatic POST.
This deliberately favors no duplicate listings over automatic retry; it is not
exactly-once delivery and can require investigation even when no request was sent.
Credential validation/refresh happens before that marker, so failures known to
precede creation retain the normal queue retry policy.
Do not delete/reset receipts or change request IDs to bypass this guard.

Once the returned identity is saved, transient GET errors retry only read-back.
Read-back mismatch and mapping conflicts remain actionable with the external ID
retained. Manual mapping cannot race a pending/uncertain creation through the
mapping API. Database administrators and direct table writers must respect the
same constraints; there is no remote transaction or universal writer lock.
Recovery UI/explicit approved adoption of an uncertain remote ID is not implemented
yet; inspect the booth and receipt before planning that operation. Never infer
identity just from a matching name. Product-create webhooks arriving before the
mapping exists may need the existing scoped replay after mapping confirmation.
The completion transaction rechecks the current connection's booth/shop/tenant
and readiness after the remote GET. A concurrent identity change retains the
returned product ID for investigation and does not commit a mapping.

The older SDK `CreateProductAsync(ProductWriteRequest)` is unchanged and is **not**
used by this flow; it does not supply the complete new draft contract. No existing
consumer is silently switched to the new behavior.

## Validation

- `tools/CatalogReconciliationChecks`: real disposable SQL, production API service,
  scenario queue/processor, receipt/mapping transaction; controlled provider and
  accounting ports. Includes concurrent starts, stale approval, lost POST response,
  crash marker, GET retry without POST, and mapping-race refusal.
- `tools/BasalamTransportChecks`: intercepted HTTP verifies draft JSON and no
  automatic POST retry on 408/429/503, even with MaxRetries5.
- `tools/EndpointIsolationChecks`: actual ASP.NET routing/authentication with
  controlled services; both draft routes covered for seven identity modes.
- `tools/SynchronizationFlowChecks`: production Basalam adapter and token vault
  with intercepted HTTP, including created-product identity and booth validation.

Verified 2026-09-28: CatalogReconciliationChecks **110**, EndpointIsolationChecks
with SQL **178**, SynchronizationFlowChecks **274**, BasalamTransportChecks **81**
passed (643 total). The Integration.Infrastructure build with `--no-restore -m:1
-p:NuGetAudit=false` completed with zero warnings/errors. These are targeted
checks, not a full solution build or browser acceptance.
The additive schema was applied to local `HyperyekIntegration`; its creation
receipt table was verified empty. No API/worker deployment or real product
creation was performed by these checks.

These are not live acceptance. Valid booth OAuth, approved publication metadata
and deployment are required before creating the vacuum bag. The reverse direction
(Basalam rice to accounting) remains a separate accounting-owner API task.

Source checked 2026-09-28: [official Basalam OpenAPI CreateProductSchema](https://github.com/basalam/python-sdk/blob/main/openapi_data/core.json).
Required fields are name/category_id/status/preparation_days/package_weight;
the draft flow additionally requires a valid accounting price and fixes stock0.
