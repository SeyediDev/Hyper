# E2E-101: reservation verification

The verifier builds the current production reservation service, dispatcher,
Integration context and mappings from source. It uses SQL Server transactions,
the existing unique keys and a fake accounting/catalog port. It does not contact
Basalam or create accounting invoices.

Run from Backend with `RESERVATION_TEST_SQL` set securely to a SQL Server
connection that can create a disposable database:

```powershell
dotnet run --project tools/ReservationChecks --artifacts-path .artifacts/reservation-isolated
```

The tool generates a `HyperReservationChecks_<guid>` database and removes only
that database in `finally`. No existing database tables are cleared.

Coverage: multi-product orders, repeated product aggregation, exact replay,
changed replay content, unverified stock refusal, idempotent release,
connection/tenant/case-sensitive order identity, competing orders on separate
SQL connections, unpaid orders, and replay after an ambiguous accounting timeout.

Reservations use a scoped SHA-256 order prefix plus a product suffix to preserve
the existing `(ShopId, ReservationKey)` unique key. Reserve and release serialize
through a transaction-owned SQL application lock per shop. Pending or ambiguous
accounting outcomes retain their hold for replay/reconciliation. Confirmed order
cancellation releases the hold. Shipping alone must not release stock until the
accounting stock deduction is known to have committed.

Accounting replies now include `StockCommitted` (default false for old servers).
An Applied/Duplicate reply with this flag transitions the reservation to status 2
(consumed). Such rows are not subtracted again from accounting stock, but remain
available for replay and content verification. A timeout leaves status 0 intact.
HTTP 409 response bodies must be preserved so replay carries this acknowledgement.

Limits and remaining acceptance work:

- These locks coordinate Integration reservations only. An atomic operation in
  the accounting owner must coordinate native/POS stock writers before claiming
  global overselling prevention.
- Catalog stock must be verified gross stock (`AccountingStockSourceVerified`).
  Full sellability flags and native reservations require an owner-side contract.
- Accounting idempotency/cancellation now include connection identity; see
  `ACCOUNTING_ORDER_VERIFICATION.md` for transaction tests and activation limits.
- Historical `order:<external-id>` reservations require explicit reconciliation;
  they are not silently reassigned or released by the new scoped identity.
- Inventory capture and scenario processing still contain raw legacy-table SQL;
  absence of a typed accounting DbContext reference does not complete that boundary.
- `E2E-101` does not certify stock consumption, physical returns, or the customer
  purchase E1/E2 policy. These require accounting-side acceptance.
