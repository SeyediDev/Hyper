# Integration queue dashboard (DASH-203)

Open **عمليات یکسان‌سازی** (`/MerchantSimulation/Dashboard`) after selecting a shop.
The page shows recent incoming webhooks and outgoing messages, plus separate lists
for failed webhooks and dead-letter outgoing messages. Each list is limited to 30
rows. Failure totals cover all matching rows, so they can exceed the displayed count.
Old failures remain visible even after newer successful messages arrive.

The existing dashboard API response adds `RecentInbox`, `FailedInbox`, and
`DeadLetterOutbox` (JSON casing follows the host serializer). Existing fields and
request parameters are unchanged. Inbox statuses: 0 pending, 1 processed, 2 failed.
Outbox statuses: 0 pending/retry, 1 sending, 2 delivered, 3 dead letter.
Inbox ordering is receipt time descending, then ID descending; outbox ordering is
ID descending. Lists and totals are scoped through connections belonging to the
requested shop AND tenant. No payload JSON or credentials are included in the new
response records. Razor encodes event IDs, names, and error text.

These are read-only diagnostic lists. They do not retry or replay messages.

Validation: `dotnet run --project tools/DashboardChecks` uses SQL Server local
temporary tables and synthetic data, with a fake catalog port. It makes no calls
to Basalam or accounting and writes no permanent business records. Set
`DASHBOARD_CHECK_CONNECTION` to override the default local Windows-authenticated
connection to tempdb. Tests cover scope isolation, bounded results, stable ordering,
older failures, empty scope, and API projection without raw payloads.
