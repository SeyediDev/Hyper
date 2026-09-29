# CMD-102: accounting order and stock transaction

The platform-owned handler commits the invoice, its lines and conditional stock
decrements in one SQL transaction. A failed line rolls back all prior decrements.
Only fully paid booth orders are accepted. The paid amount uses the contract's
Paid=1 value. Unit prices/quantities and invoice totals must fit the mapped SQL
precision; totals requiring discount/tax/shipping policy are rejected rather than
silently replaced by a different total.

The order marker hashes shop, tenant, connection and the exact external order ID.
It also stores a command fingerprint. A shop-level transaction lock serializes
online apply/cancel operations. Equal replays return the committed invoice without
another debit; changed-content replays are rejected. Legacy unscoped markers need
explicit reconciliation. Do not edit these markers in the invoice description.

Confirmed cancellation before shipping restores line quantities in the same
transaction as cancellation. Repeated cancellation cannot restore twice. Shipping
and delivery require physical return confirmation before stock is restored.
Marketplace product snapshots never replace accounting stock.
Product field application now has its own atomic receipt and ordering protocol;
see [CMD-101 product changes](ACCOUNTING_PRODUCT_CHANGES.md) and its required
accounting-database schema script before deploying the updated product handler.

`AccountingCommandResult.StockCommitted` is true only for successful/duplicate
committed orders. Integration consumes its hold on this acknowledgement, including
on replay after a timeout. It retains holds for ambiguous outcomes.

## ORD-201: Integration order receipt

After an Applied or Duplicate vendor-order response, Integration stores the
acknowledged positive numeric invoice ID in `ExternalOrderMappings`, scoped by
the persisted connection, shop, tenant and exact external order identity.
Preflight rejects scope mismatches and conflicting existing identities before
reservation or the accounting call. The receipt repeats those checks under a
short serializable Integration transaction and connection-row lock. SQL's
case-insensitive order lookup is followed by an ordinal identity comparison.
Equal concurrent receipts converge; a different invoice never overwrites one
already acknowledged for that order. No transaction spans the accounting call.

The receipt uses fresh untracked reads and updates only invoice/time fields;
unrelated tracked changes are not saved. A new row leaves `ExternalParcelId`
null, because an incoming parcel ID does not verify membership of the complete
invoice. Existing approved parcel bindings and status are preserved. An explicit
incoming parcel that contradicts a nonnull binding is rejected; an omitted
parcel leaves the binding intact. Parcel membership verification remains part
of the parcel lifecycle acceptance scope.

A stock-committed Applied/Duplicate response consumes the reservation before
the local mapping write. If the receipt fails after that acknowledgement,
replaying the same order can repair it from the owner's Duplicate response;
Integration does not cancel the invoice or release the consumed hold. A valid
Applied/Duplicate response with `StockCommitted=false` still records its invoice
without consuming the hold. Pending, rejected, transport-failed or malformed
invoice acknowledgements create no mapping.

This change does not backfill historical Completed jobs that lack mappings.
Redelivery of their original webhook remains deduplicated, and the current
Integration API does not expose a generic replay of Completed sale jobs. Any
historical reconciliation needs a separately scoped maintenance operation;
the receipt recovery described above applies to replay of unfinished work.

`IntegrationBusinessEventDispatcher` always defaults to the real SQL receipt
store. Its optional infrastructure receipt boundary allows
`OrderHappyPathChecks` to observe call ordering and scope with EF InMemory; that
check does not verify persistence or SQL concurrency. `OrderMappingChecks`
exercises the production receipt store against an isolated SQL database:

```powershell
# Set ORDER_MAPPING_SQL_TEST_CONNECTION securely to a local test SQL Server.
dotnet run --project tools/OrderMappingChecks --artifacts-path .artifacts/order-mapping-checks -- --sql
```

The relational harness creates a fresh GUID-named database, refuses cleanup
unless its own CREATE was acknowledged, and removes only that exact generated
database. It uses the production Integration model without weakening its
constraints. It does not invoke live providers or the native accounting schema.
Execution evidence must distinguish this relational fixture from native POS,
external provider and complete parcel-lifecycle acceptance.

## Verification

Set `ACCOUNTING_TEST_SQL` securely to a local test SQL Server connection and run:

```powershell
dotnet run --project tools/AccountingOrderChecks --artifacts-path .artifacts/reservation-validation
```

The tool creates and removes only a random `HyperAccountingChecks_<guid>` database.
It uses the production handler, DbContext and column/key mappings for the order,
line, product, customer and fiscal-period tables. Unrelated legacy tables and their
relationships are excluded from the disposable fixture. This is not a test of the
entire production accounting schema, triggers, invoice numbering, or stock cards.

Tests cover rollback, concurrency, replay, cancellation, connection isolation,
unpaid orders, paid amounts, shipped-order refusal, read-handler DI, and a competing
native-style conditional stock writer. ReservationChecks separately verifies the
stock-committed acknowledgement through the real HTTP client and dispatcher.

Verified on 2026-09-26: all 18 AccountingOrderChecks SQL assertions passed,
including long cancellation descriptions within the production column limit and
idempotent replay with numerically equal decimal representations. All 15
ReservationChecks SQL/dispatcher assertions also passed. Both fixtures use
disposable databases; these results are not live Basalam or native POS acceptance.

## Activation prerequisite

`AccountingInventory:AccountingStockSourceVerified` defaults to false. Enable it
only after ASM-001 confirms that ACCOUNTINGSTOCK_ is authoritative gross stock and
that another invoice/stock-card writer will not debit these API invoices again.
The Integration stock-source verification setting is still required separately.
No production setting is enabled by this change.

The native POS writer is not present in this repository. The SQL race test models
a conditional relative decrement; it does not prove the actual POS uses it. A POS
that overwrites a stale absolute balance or ignores available stock can still
break the invariant. That writer must adopt the same accounting transaction/stock
protocol (or equivalent conditional update) and undergo acceptance testing before
claiming global overselling protection. Native reservation, warehouse/stock-card,
tax/discount and physical-return policies remain separate acceptance work.
