# Accounting HTTP delivery and durable retries

`HyperyekAccountingApiClient` performs one HTTP attempt per invocation. It does
not sleep/retry accounting commands internally. `IntegrationScenarioQueue` owns
durable attempts, scheduling, leases and inbox completion. This avoids multiplying
network retries or changing command identity between worker attempts.

HTTP 408, 429, 500, 502, 503 and 504 raise a retryable
`IntegrationProviderException` with code `AccountingApi_<status>`. Both delta and
HTTP-date Retry-After are carried to the worker; a past date means zero additional
delay, not immediate bypass of backoff. The existing queue takes the greater of
Retry-After and its 1/5/15/60/1440-minute schedule, up to six total attempts.

A scheduled transient failure leaves the original job and inbox Pending with no
completion timestamp. Recovery updates those same records; a new worker context
can continue after restart. Exhausted attempts become DeadLetter/failed inbox.
Accounting must still deduplicate lost-acknowledgment command replays; see
[product receipts](ACCOUNTING_PRODUCT_CHANGES.md) and the order command contract.

Other command HTTP errors remain permanent rejected results. Read errors use
the same transient classification, while other failed reads raise nonretryable
errors; single-shop 404 still returns null. Transport exceptions and caller
cancellation propagate to the existing worker policy, rather than being reported
as business rejection. Capture's periodic rescan is not a durable command retry.

Command acknowledgments must match the implemented accounting protocol:

| HTTP | Body status |
| --- | --- |
| 200 | Applied (1) |
| 409 | Duplicate (2) |
| 202 | PendingDependency (3) |
| 422 | Rejected (4) |

An empty, malformed, undefined or mismatched command result is a nonretryable
`AccountingApiInvalidResponse`, never a successful acknowledgment. StockCommitted
is accepted only on Applied/Duplicate, not on a policy-dependent/rejected result.
Duplicate keeps its reference and stock acknowledgment so a lost HTTP response
does not cause a second stock debit or leave an already-consumed hold active.
Raw HTTP error bodies and request URLs are not included in transport error codes.

## Verification and limits

`tools/SynchronizationFlowChecks/AccountingHttpChecks.cs` intercepts HTTP to test
temporary/permanent statuses, Retry-After, command-result matching, cancellation
and absence of hidden immediate retries. The SQL flow suite additionally checks
503/429 scheduling, no early resend, fresh-context recovery with the same command
identity/version, and finite retry exhaustion. No real provider calls occur.

This stage does not implement a distributed circuit breaker, proactive provider
rate limiter, correlation-header policy or token refresh changes. Those remain
in ADP-HTTP/provider work. It changes no authorization, schema, live endpoint
configuration, accounting policy or Neo.Bpms reference. Platform-manager approval
item 128 continues to govern the native writer and live accounting deployment.
