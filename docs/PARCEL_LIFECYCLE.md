# FLOW-C: parcel lifecycle and tracking

Integration owns webhook intake, provider translation, queue and command receipts.
Only the Accounting API writes invoice delivery/tracking. There is no new BPMS
dependency outside AdminPanel and no new accounting-table write in Integration.
The existing `Worker -> Integration.Infrastructure -> Hyper.Infrastructure ->
Hyper.Domain -> Neo.Bpms.Domain` reference remains a boundary violation tracked
under `ARCH-NEO-001`; FLOW-C neither introduced nor removed it. The user was
notified; approval/architecture work is separate from this parcel change.

## Provider contract

Verified against the official [webhook guide](https://developers.basalam.com/docs/services/webhook)
and Basalam PHP SDK commit `44d49e4b5247022e0df45bccaef04cf2281ad20a`:
[service](https://github.com/basalam/php-sdk/blob/44d49e4b5247022e0df45bccaef04cf2281ad20a/src/Basalam/OrderProcessing/OrderProcessingService.php),
[parcel](https://github.com/basalam/php-sdk/blob/44d49e4b5247022e0df45bccaef04cf2281ad20a/src/Basalam/OrderProcessing/Models/ParcelResponse.php),
[status codes](https://github.com/basalam/php-sdk/blob/44d49e4b5247022e0df45bccaef04cf2281ad20a/src/Basalam/OrderProcessing/Models/ParcelStatus.php),
[receipt](https://github.com/basalam/php-sdk/blob/44d49e4b5247022e0df45bccaef04cf2281ad20a/src/Basalam/OrderProcessing/Models/PostReceipt.php),
[shipping method](https://github.com/basalam/php-sdk/blob/44d49e4b5247022e0df45bccaef04cf2281ad20a/src/Basalam/OrderProcessing/Models/ShippingMethod.php),
[gateway config](https://github.com/basalam/php-sdk/blob/44d49e4b5247022e0df45bccaef04cf2281ad20a/src/Basalam/Config/Config.php).

The SDK default production gateway is `https://openapi.basalam.com`.
`GET /v1/vendor-parcels/{id}` supplies `vendor.id`, `order.id`, `status.id`,
`is_delivered`, `shipping_method.current.id`, and `post_receipt.tracking_code`.
Order-item identity is not order identity. Translated status titles are not codes.

Our conservative accounting translation (not a provider-defined accounting rule):

| Remote state | Normalized accounting command |
| --- | --- |
| 3237 and not delivered | preparing |
| 3238 and not delivered | shipped |
| 3238 or 3195 with explicit is_delivered=true | delivered |
| New order3739, cancellation, exceptions, unknown/inconsistent state | NeedsAttention |

There is no automatic financial cancellation, return or stock adjustment from
these parcel states. Those workflows need their own owner-approved contracts.

## Inbound: Basalam to accounting

The exact `VENDOR_PARCEL_CHANGES` or normalized `parcel.status_changed` event is
stored in Inbox and queued as Sale. The worker reads current provider details
with the connection grant; webhook order/status/tracking fields cannot override
them. Returned parcel ID and vendor must match. A trusted `ExternalOrderMappings`
row must bind connection/shop/order/parcel to a positive accounting invoice ID.
Missing/mismatched binding stays `ParcelInvoiceMappingRequired`. We do not infer
that one parcel represents every parcel of an order.

Only then is the normalized command sent through the Accounting API. Its precise
pending error (including overlong/conflicting tracking) is retained in the job
differences. The existing accounting handler provides monotonic delivery guards,
duplicate detection, cancellation protection and varchar(18) tracking checks;
see [accounting verification](ACCOUNTING_PARCEL_VERIFICATION.md).

## Outbound: explicit preparation/posting

Authenticated routes (same shop/tenant policy as other Integration endpoints):

`POST /api/integrations/v1/connections/{connectionId}/parcels/commands`

Headers: `X-Shop-Id`, `X-Tenant-Id` and the ordinary authenticated identity.
Preparation body:

```json
{"requestId":"11111111-1111-1111-1111-111111111111","parcelId":"70","orderId":"700","target":1}
```

Posting body (use real approved IDs/method/tracking, not these fixture values):

```json
{"requestId":"22222222-2222-2222-2222-222222222222","parcelId":"70","orderId":"700","target":2,"shippingMethod":3198,"trackingCode":"TRACK-2"}
```

Read via `GET .../parcels/commands/{requestId}`. HTTP202 means queued/accepted,
not posted. Actor is captured from authenticated identity. Invalid data returns
400, ownership absence404, conflicting request/binding409. Direct service callers
receive stable conflict codes; the public controller returns a bounded error.

Receipt and existing Sale/Manual queue entry commit atomically. Exact retries
return the same receipt; a new ID cannot bypass the unique connection/parcel/target
guard. Before sending, worker re-reads provider identity and verifies current
binding/scope. Preparation permits new->preparing; posting permits preparing->posted
with the explicitly supplied current shipping method. Already-satisfied requests
only read/confirm; they do not regress state or send again.

The committed send marker precedes POST. SDK preparation/posted calls disable
transport retries. A timeout,503,crash or stale readback moves to GET-only
verification on retry. Only a matching state and requested tracking complete the
receipt. Exhausted verification is actionable, never an automatic second POST.
Arbitrary tracking corrections/resending need an explicit future reconciliation
workflow, not deleting receipts. The SDK's legacy generic parcel PATCH methods
are not used by this path.

## Deployment and checks

Apply `docs/schema/ensure-integration-parcel-commands.sql` to the Integration DB
before deploying API and worker together. Existing enum values and queue schema
are unchanged. No accounting schema is changed. Existing scenario dashboard
shows Sale/Manual jobs and differences; this stage adds no dedicated parcel UI.

`tools/SynchronizationFlowChecks/ParcelLifecycleChecks.cs` exercises production
SDK/adapter, ingress, SQL queue, receipt/API service and accounting HTTP boundary
against an isolated SQL database and intercepted transports. It also runs the
actual additive schema twice. `tools/EndpointIsolationChecks` includes both new
protected routes for all existing identity modes.

Live acceptance still requires a real scoped grant (`vendor.parcel.read` and
`vendor.parcel.write`), approved order/invoice/parcel mapping and a parcel that
may genuinely be prepared/posted. The native accounting writer is not available
in this repository: platform-manager approval remains required for automatic
accounting event emission and multi-parcel invoice policy. Public webhook docs
still do not prove delivery metadata; retain the acceptance gate documented in
[webhook delivery](BASALAM_WEBHOOK_DELIVERY.md). Controlled tests are not live
Basalam acceptance and must not mark that gate complete.

## Verification checkpoint: 2026-09-29

- Full project-reference builds of `EndpointIsolationChecks` (including the API)
  and `Hyper.IntegrationWorker.Host` exited0, zero warnings/errors.
- `SynchronizationFlowChecks` exited0: **337 checks passed**, including63 parcel
  checks. The final test-project build used `--no-restore -m:1 -v:minimal
  -p:NuGetAudit=false -p:BuildProjectReferences=false` after building production
  dependencies. Run `dotnet tools/SynchronizationFlowChecks/bin/Debug/net10.0/SynchronizationFlowChecks.dll`.
  `SYNC_CHECKS_CONNECTION` may select an authorized local SQL fixture server;
  its default is local integrated authentication. Only its unique disposable
  database is created/deleted. Provider/accounting HTTP is intercepted, not live.
- `dotnet tools/EndpointIsolationChecks/bin/Debug/net10.0/EndpointIsolationChecks.dll --sql`
  exited0: **220 passed,0 failed**, with `ISOLATION_TEST_SQL` supplied to the
  process. Its disposable database was removed. See that tool's README for scope.
- Early synchronization fixture runs failed because a pending accounting result
  used HTTP200 instead of202, then because the new fixture ran before an existing
  global inbox-count assertion. Both test harness issues were corrected; the
  final full run above passed. Production HTTP acceptance was not relaxed.
- The additive script was applied twice in the isolated SQL suite and once to
  local `HyperyekIntegration`. Verified the table, both uniqueness constraints,
  enabled state/target check and NO_ACTION connection FK. Local receipt and
  connection counts were0. No host restart/deployment or provider write occurred.
- **Current accounting regression is not passed:** `AccountingOrderChecks`
  built successfully, but its executable failed before assertions with SQL1785,
  `FK_TBL_Person_TBL_Shop_Shopid1`, while creating its disposable schema. Source
  inspection finds newly exposed navigation conventions alongside navigationless
  generated relationships (for example `SqlTblPerson.Shop`). This needs an
  accounting-model fix and fresh out-of-order/tracking verification; fixture
  constraints were not weakened to hide it. The failed run's database contained
  no user tables and was removed explicitly after checking its exact identity.
  The historical2026-09-26 accounting pass is not a pass for today's model.

Before live acceptance: resolve that model regression, then run the production
accounting checks; obtain an approved invoice/order/parcel binding, a scoped grant
and genuine preparation/posting authorization; verify delivered metadata and
tracking against both owners. Never use product IDs as parcel IDs or invent a
tracking number. `INT-006.PLATFORM-APPROVAL` remains the native-writer TODO.
