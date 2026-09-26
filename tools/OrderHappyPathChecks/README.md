# Order happy-path smoke check (E2E-103)

```powershell
dotnet run --project tools/OrderHappyPathChecks --artifacts-path .artifacts/order-happy
```

Runs production `IntegrationBusinessEventDispatcher` and
`HyperyekAccountingApiClient` against an EF in-memory inbox/product/customer
mapping fixture. Checks paid sale -> mapped reservation -> accounting request,
successful cancellation -> release of the same reservation, acknowledged stock
write -> committed hold, and customer purchase -> accounting route.

Per the user's decision, this deliberately does not create a SQL database or
expand into failure/concurrency/security testing. Reservation persistence and
the remote accounting response are probes; this proves orchestration order and
HTTP serialization, not SQL locking, real stock mutation, a posted accounting
document, deployed-host authentication or live Basalam delivery. No network or
business data writes are performed. The broader E2E-103 acceptance remains
separate from this limited happy-path result.

Verified on 2026-09-26: targeted `dotnet build` with the same project/artifacts
path exited 0 with 0 warnings/errors. Running
`dotnet .artifacts/order-happy/bin/OrderHappyPathChecks/debug/OrderHappyPathChecks.dll`
exited 0: **9 happy-path checks passed**. No production fix was required by these
scenarios on baseline `0452abc`.
