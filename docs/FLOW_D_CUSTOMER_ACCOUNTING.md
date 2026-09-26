# FLOW-D customer-to-invoice checkpoint

Accounting owns customer/person, detail-account and invoice persistence.
Integration continues to own the external-customer mapping and calls the existing
accounting contract. No accounting entity or new persistence dependency moves to
Integration or AdminPanel.

## Behavior

- Resolve and validate reject invalid field lengths/empty identity before writes.
  The actual shop must exist and its canonical tenant must match the command.
- Legacy null/blank tenants are accepted only under the shop's canonical
  `shop:<id>` scope. An explicitly tenanted shop does not adopt unscoped people.
- Customer type/role come from `IntegrationCustomer` configuration; there are no
  invented production defaults. Detail accounts must belong to the same shop and
  tenant, have the configured entity type, and not reference another person.
  A legacy null back-reference is accepted; new accounts receive the person ID.
- Customer creation and account linking commit in one accounting transaction.
  Resolution uses the same per-shop accounting lock as invoice creation to
  serialize concurrent API requests before matching/creating people.
- A national-identity match can recover the existing customer after a lost
  Integration mapping write. Conflicting identities, multiple matches and
  mobile-only existing matches still require review: a phone number alone is not
  permission to merge people. No distributed transaction or universal replay
  recovery is claimed.
- A new invoice rechecks customer role and detail-account consistency before
  debiting inventory. A completed invoice replay retains its original committed
  result; it does not create another invoice or debit.

## Verification

Run `tools/AccountingOrderChecks` with `ACCOUNTING_TEST_SQL` supplied through an
environment variable. It creates and deletes only a randomly named disposable
`HyperAccountingChecks_*` database. The fixture includes real mapped person,
detail-account, shop, product, fiscal-period and invoice tables/relationships;
unrelated legacy tables are excluded. This is not full production-schema or
native POS acceptance.

Checks cover canonical/legacy scope, invalid input, concurrent resolution,
ambiguous/mobile-only identities, account shop/type/reference mismatches,
customer-to-invoice replay, and previous stock/cancellation/concurrency cases.

Verified on 2026-09-26: all 40 accounting SQL checks and all 32 accounting
authentication checks passed. Both check projects (including the accounting host)
built with zero warnings/errors using isolated artifacts and command-local
`NuGetAudit=false`; this is not a vulnerability audit or a full-solution build.
The first SQL run exposed missing product detail-account seed data after enabling
the real account relationships. The fixture now seeds those accounts; no mapped
foreign keys were removed. The disposable databases were cleaned up by the tool.

## Remaining scope

FLOW-D is not complete: real customer conventions must be configured on the
accounting host, and native POS/stock-card, financial numbering, discounts,
commission, shipping, tax and warehouse policies still need their own acceptance.
Stock-source verification remains disabled unless explicitly approved after that
acceptance. No live IdP, Basalam, production accounting database or deployment is
changed by these checks. Transitive Core/Neo.Bpms removal is separate work.
