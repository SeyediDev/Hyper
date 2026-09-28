# Vendor invoice parcel lifecycle checkpoint

The platform handler accepts normalized `preparing`, `shipped`, and `delivered`
commands, case-insensitively with surrounding whitespace removed. It preserves
the existing local delivery encoding (0/1/2). These names are an internal command
vocabulary, not a claim about Basalam webhook payloads or native POS enum values.
Unknown provider codes/statuses wait for mapping; substring matching is removed.

Under the existing per-shop SQL transaction lock:

- Preparing keeps an unshipped invoice cancellable. Shipment/delivery prevent
  stock restoration without physical-return confirmation.
- Delivery can arrive before shipment; lower-state replays are no-ops and cannot
  regress delivery or replace tracking. Equal replays return Duplicate.
- Missing/blank tracking preserves the stored value. A same-state update may fill
  a missing code; a conflicting nonempty code requires review, not replacement.
- Tracking that cannot fit the mapped varchar(18) column requires review. It is
  never silently truncated or transliterated. No database column was widened.
- Missing invoices remain pending so the event can retry after invoice creation.
  Cancelled invoices cannot be revived by a parcel event. Canonical shop scope is
  checked before both parcel and cancellation writes.
- Concurrent cancellation and shipment commit one consistent outcome: cancellation
  wins and restores once, or shipment wins and requires a physical return.

`AccountingOrderChecks` covers these cases with the production handler and mapped
disposable SQL fixture, including concurrent and out-of-order requests. No live
provider mutation or production schema change is involved.

Verified on 2026-09-26: all 60 accounting SQL assertions passed, including 20
new parcel/cancellation cases. The check project and accounting infrastructure
built with zero warnings/errors using isolated artifacts and command-local
`NuGetAudit=false`. This is not a vulnerability audit, a full-solution build or
live Basalam acceptance. The disposable database was removed after the run.

Recheck on 2026-09-29: the current accounting check project builds, but SQL fixture
creation fails before assertions with SQL1785 on
`FK_TBL_Person_TBL_Shop_Shopid1`. Navigation conventions now coexist with the
generated navigationless relationships; this current model regression must be
resolved and the suite rerun. The old60-check success above is historical, not a
current pass. No production mapping or fixture constraint was altered by FLOW-C.
The failed run's empty disposable database was identified and removed. Current
Integration-only SQL/HTTP results are recorded in [FLOW-C](PARCEL_LIFECYCLE.md).

## Limits

This is an order-level delivery guard, not complete multi-parcel aggregation.
`ExternalParcelId` is validated but not durably bound by the accounting handler.
The [FLOW-C integration path](PARCEL_LIFECYCLE.md) now requires an explicit
Integration order/parcel mapping and re-reads provider details with exact-state
normalization before invoking it. This does not add multi-parcel aggregation to
the accounting API or authorize arbitrary mappings. Separate return/correction
workflows and real provider acceptance remain prerequisites for full live lifecycle
acceptance. A stale lower-state event is acknowledged as Duplicate, not audited
as a distinct parcel transition. Correcting a stored tracking code requires an
explicit reconciliation workflow. Financial and stock-source activation gates
remain unchanged.
