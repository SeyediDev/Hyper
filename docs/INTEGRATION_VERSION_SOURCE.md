# Durable outbound version ownership

`IntegrationVersionOwnership` belongs only to the Integration database. Each row
references one mapping and stores Source, VersionFloor and UpdatedAtUtc.

## Sources

- `0` Unassigned: no row exists; read-only state, not a selectable owner.
- `1` AccountingEvents: inventory-changed and product-changed share one externally
  coordinated, monotonically increasing SourceVersion stream.
- `2` InventoryCapture: verified accounting-catalog polling allocates inventory
  versions. Product event requests are also rejected in this mode.

The first successful enqueue in a **new empty stream** claims its source in the
same SQL transaction as its Outbox message. The mapping's UPDLOCK/HOLDLOCK serializes
selection across processes, even with different API/worker configuration. Prefer
explicit source selection before enabling a production writer. The source label
does not authenticate a producer or replace scoped authorization. Multiple event
producers still need one shared external version allocator.

Existing Outbox history without ownership is not classified by guessed origin:
enqueue returns `VersionSourceUnassigned`. Select its owner explicitly using the
current high-water mark. Do not delete history or reset versions as a workaround.
Automatic capture excludes event-owned mappings and unassigned legacy histories;
direct reconciliation reports an ownership error. Races after candidate selection
are rechecked under the mapping lock. AccountingStockSourceVerified is still required.
After a switch, the first capture establishes a new checkpoint above VersionFloor;
an identical payload from the old source (including a failed message) cannot suppress it.

## Scoped API

`GET` / `PUT`
`/api/integrations/v1/connections/{connectionId}/mappings/{mappingId}/version-source`

Both require the existing authenticated shop/tenant claim and `X-Shop-Id` /
`X-Tenant-Id` selection headers. Headers alone are not authorization. Disabled,
inactive and foreign-scope mappings return 404. GET returns `status`, `source`,
`lastVersion`, `versionFloor` and `errorCode`. PUT accepts:

```json
{
  "source": 2,
  "expectedSource": 1,
  "expectedLastVersion": 123
}
```

Read expected values from GET, stop the old producer, and drain its pending work.
A newer write/owner causes 409 `VersionSourceSnapshotChanged`; pending or leased
messages cause 409 `VersionSourceHasPendingMessages`. Same-source selection is a
no-op after checking expected state. Undefined/zero desired owners return 400.
Success returns 200 and preserves the previous high-water mark as VersionFloor.

New-owner writes must exceed the floor. Old-owner dead letters at/below it cannot
be replayed. Current-owner exact retries retain the original Outbox identity.
All enqueue paths enforce ownership in transactions, not only controllers.
Inventory/product endpoints return 409 for VersionSourceConflict,
VersionSourceUnassigned, SourceVersionConflict and StaleSourceVersion. Rejected
requests create no new deliverable message.

## Deployment and limits

Before updated API/worker startup, run
`docs/schema/ensure-integration-version-ownership.sql` on the database selected by
ConnectionStrings:IntegrationConnection. It only creates the missing table and
does not guess/backfill owners. Fresh schema creation also includes the table.
Deploy updated enqueue paths together: old binaries can bypass this protocol.
Creating the schema/choosing a source does not enable a live writer.

SynchronizationFlowChecks covers repeatable schema application, explicit switches,
scope isolation, pending/leased barriers, legacy history, replay floor, persisted
ownership after context restart and competing first writers. SQL is disposable;
HTTP is intercepted and controller authorization uses synthetic principals.
These are not auth-middleware or real-provider acceptance tests.

This is not the accounting-owned transactional producer/CDC implementation.
Intermediate change capture, price/unit policy, origin/echo provenance and live
acceptance remain INT-006/Task101 work. No accounting tables or Neo.Bpms dependency
are added, and no live writer configuration is enabled.
