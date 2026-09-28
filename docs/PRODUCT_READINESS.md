# Product readiness, remediation and adaptation

Policies are persisted per connection and direction in the Integration database:

- `Hold=1` (default): invalid destination data stays in a durable worklist.
- `Adapt=2`: run only explicitly enabled rules, retain before/after values, then
  validate again. Unresolved issues still stay in the worklist.

The initial rules are Basalam description trimming and opt-in truncation to10000
UTF-16 code units without splitting surrogate pairs. Truncation is lossy and must
be explicitly enabled. No guessed identity, price, stock, currency, category,
weight, preparation time or photo is allowed. Source facts come from accounting
API, never direct accounting table writes. Digikala/other creation and accounting
creation remain unsupported destinations with actionable errors in either mode;
policy settings do not invent destination capabilities.

## Collection, audit and execution

Catalog product reconciliation collects unmapped parent products in both
directions. Inbound product webhooks collect missing parent mappings before
retaining the existing NeedsAttention result. Discovery alone never starts a
creation. Variant-specific cases remain in scenario differences, not merged into
parent worklist items. Already mapped updates and the TODO128 stock gate are
unchanged.

`IntegrationProductPreparations` is unique per connection/direction/source.
`IntegrationProductPreparationHistory` stores immutable revisions with actor,
UTC time, original input, prepared input, effective policy, issues and changes.
`IntegrationProductPreparationPolicies` holds current policy and updater.
Policy changes do not reevaluate existing rows silently: recheck explicitly with
the current revision. Collector discovery is hold-only and never overwrites user
edits or adds duplicate history.

Preparation serializes under a connection lock and atomically commits revision,
audit, creation receipt and scenario job. Retried identical commands deduplicate;
stale changed edits return409. Submitted data is immutable. Unknown remote
outcomes cannot be resent by changing metadata/request IDs. The existing worker
checks source/booth, records one POST marker, retries GET only, and verifies the
returned identity before mapping.

`AdaptedCompleted` counts only corrected rows whose remote creation was verified
and mapped (receipt state3), never merely queued/rejected/uncertain submissions.
Totals cover both directions across the connection, independent of page size.
Execution error codes appear alongside preparation issues. Reading the board
never makes provider calls.

## API

Base: `/api/integrations/v1/connections/{connectionId}/product-readiness`.
All routes require authenticated shop/tenant scope plus `X-Shop-Id` and
`X-Tenant-Id`. The actor comes from authenticated identity, not body JSON.

- `GET ?skip=0&take=25` returns board, items and history; take1..100.
- `GET policy/{direction}`: direction1 ToPlatform, direction2 ToAccounting.
- `PUT policy/{direction}`: `{ "mode": 2, "trimDescription": true,
  "truncateDescription": false }`.
- `POST`: `{ "expectedRevision": 0, "input": { "direction": 1,
  "sourceProductId": "213896", "categoryId": null, "preparationDays": null,
  "packageWeight": null, "description": null, "photoId": null } }`.

Missing business metadata returns HTTP200 with a persisted NeedsAttention item,
not a claim of success. Repair using its current revision and same source and
direction. Complete input immediately queues an unpublished Basalam draft. Raw
description is bounded to50000; larger inputs or invalid transport/scope/identity
are rejected instead of retained. Destination metadata must be approved, never
filled from an invented global default.

The older `/product-drafts` route remains strict low-level explicit creation.
Use the new readiness API or panel for remediation/audit; old receipts are not
retroactively counted here. Neither route publishes a listing publicly.

## AdminPanel

Open **رفع نقص کالا** in the sidebar or **رفع نقص و آماده‌سازی کالا** in the
domain menu (`MerchantSimulation/Products`). Select merchant, scoped connection
and policy direction. The screen exposes counts, repair forms, field error codes,
corrections and full revision history. Forms require admin authorization,
anti-forgery, unexpired simulation and a matching current context ticket. Client
shop/tenant cannot override the selected context. Submitted rows are read-only;
blocked rows can be repaired/rechecked. Token renewal uses the existing login
flow. Unsupported destinations require an approved adapter, not a checkbox.

## Deploy and test

Apply `ensure-integration-product-creations.sql`, then
`docs/schema/ensure-integration-product-preparation.sql` on the Integration DB
before deploying API, worker and AdminPanel together. No accounting tables change.
The additive script is tested twice against a disposable SQL database.

`tools/CatalogReconciliationChecks` tests production readiness, collector, draft
API, worker and EF mappings: policies, no invented facts, repair, audit, revision
conflicts, concurrency, confirmed-only metrics, uncertain POST, isolated scopes,
unsupported destinations, collection and rollback on queue failure.
`tools/EndpointIsolationChecks` covers four new protected actions for seven
identity modes. These fixtures are not live Basalam or accounting acceptance.

Verified 2026-09-28: CatalogReconciliationChecks **144 passed** (real disposable
SQL, including schema upgrade twice); EndpointIsolationChecks `--sql` **206
passed** (178 HTTP/controller +28 SQL). Both exited0 and removed their own fixture
databases. AdminPanel build with project references completed with zero warnings
and errors. Local `HyperyekIntegration` has all three new tables, initially empty.
No live product creation, host restart/deployment or browser acceptance was done.
