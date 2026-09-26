# PLATFORM-API-001: accounting read boundary

The accounting API owns product data, gross accounting stock and sellability.
Integration inventory capture and scenario comparison now use
`IIntegrationPlatformCatalogPort` through `HyperyekAccountingApiClient`, not SQL
against `TBL_Product` or `TBL_Shop` in the Integration database. Active holds,
mapping serialization, source versions and the outbox remain in Integration.

The existing versioned catalog response adds `CanSell`. Accounting computes it
from enabled, sellable, online-sellable, stockable and non-service flags. The
catalog checks the shop's canonical tenant as well as the product tenant. A
missing `CanSell` field defaults to false: older servers do not silently authorize
new reservations. Upgrade the accounting provider before Integration consumers;
otherwise the fail-closed consumer will refuse reservations and publish zero
available inventory. No production deployment or stock-verification setting is
changed by this implementation.

Capture and comparison serialize their gross-stock/active-hold reads with the
same Integration shop lock as reservation creation, release and consumption. This
prevents an ACK removing a hold between the two reads. It is not a distributed
transaction with accounting or protection against stale absolute POS writers.
Provider unavailability fails the operation without inventing or enqueuing stock.
Reconciliation remains eventually consistent with subsequent accounting changes.

## Executable checks

Use separate artifact paths for simultaneous worktrees. Supply local SQL
credentials through environment variables, never as command arguments:

- `SynchronizationFlowChecks`: `SYNC_CHECKS_CONNECTION`; real Integration
  handlers and HTTP adapter, controlled accounting/Basalam transport responses,
  disposable Integration-only database with no accounting tables. Covers capture,
  inventory comparison, active/consumed/other-shop holds, unavailable accounting,
  non-sellable stock and old catalog responses in addition to the two event paths.
- `ReservationChecks`: `RESERVATION_TEST_SQL`; rejects a non-sellable product and
  reruns the existing atomic hold/replay/ACK checks on disposable SQL data.
- `AccountingOrderChecks`: `ACCOUNTING_TEST_SQL`; actual accounting read/command
  handler and mapped SQL fixture, including tenant scope, canonical tenantless
  shops and all five sellability flags. Unrelated legacy tables and relationships
  are excluded; this is not full production-schema acceptance.

Verified on 2026-09-26: 38 synchronization-flow, 16 reservation and 22 accounting
SQL assertions passed (76 total). All three test projects built with zero warnings
and errors. The accounting build required command-local `-p:NuGetAudit=false`
because the NuGet audit endpoint was unavailable; vulnerability auditing is not
part of this result. No full-solution build or live-service acceptance is claimed.

## Remaining acceptance

Host service authentication and client credential/configuration wiring still
need verification before the independent HTTP services can be deployed. The
legacy Core project graph still brings a transitive Neo.Bpms dependency; this
change neither adds that dependency nor claims to remove it. CMD-102's native
POS, stock-card, numbering, tax/warehouse and physical-return acceptance limits
remain unchanged. No live Basalam writes, production schema changes or deployment
are authorized by these fixture results. AdminPanel's legacy entity access is
intentionally unchanged.
