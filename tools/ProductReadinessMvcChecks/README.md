# Product preparation form checks

Run from Backend:

```powershell
dotnet build tools/ProductReadinessMvcChecks -m:1 -p:NuGetAudit=false
dotnet tools/ProductReadinessMvcChecks/bin/Debug/net10.0/ProductReadinessMvcChecks.dll
```

This separate test entry point uses the real compiled MerchantSimulation controller
and Products/partial Razor views. Authentication, selected shop and readiness
services are synthetic. It never loads the production Program/configuration,
starts workers, registers SQL, reads credentials, or calls Basalam/accounting.
Only an ephemeral loopback address is bound. The fixture cookie/sign-in route
exists only inside this executable, never the production host.

Checks exercise actual form model binding, nullable fields and explicit zero,
antiforgery, admin authorization, authenticated actor, immutable scope, foreign
connection and expired/tampered simulation rejection, checkbox+hidden-value
binding, post-redirect-get, queued vs completed status, HTML encoding, and the
empty connection state. This complements the real-SQL service/worker tests in
CatalogReconciliationChecks, not a substitute for them.

Append `--serve` to keep the already-checked fixture available for ten minutes.
Open its printed `BROWSER_FIXTURE_URL` using the browser. The header identifies
the synthetic data; changes affect fixture memory only. This mode does not grant
access to real user identities or data. Closing a browser tab does not stop the
host; it exits automatically after the bounded lifetime.

If an unrelated dependency prevents rebuilding the full panel, an explicit
fallback tests an **existing compiled panel**:

```powershell
dotnet build tools/ProductReadinessMvcChecks -m:1 -p:NuGetAudit=false -p:UseBuiltPanel=true
dotnet tools/ProductReadinessMvcChecks/bin/Debug/net10.0/ProductReadinessMvcChecks.dll --serve
```

This requires the panel and dependency DLLs in its configuration/framework output
directory. It never certifies current source or a fresh full build; rebuild the
panel after source changes before relying on this fallback. Both modes use the
same compiled controller/views, with a clearly marked test-only layout shell.

Verified 2026-09-28 using `UseBuiltPanel=true`:17 checks passed with exit0. Browser
repair submission also showed queued1/attention0/completed0 and a read-only
submitted item. The real Neo exception filter sends anonymous requests to
`Account/Login` with HTTP302; authenticated non-admins receive403. This fixture
does not substitute a different production authorization policy.

The full-graph build failed in Neo-Bpms on missing RestSharp and SQL Management
Objects asset references. Fallback success must not hide that build limitation.
The initial anonymous-status expectation was corrected after inspecting the real
base-controller filter. Failed assertions now return1 instead of invoking
Windows unhandled-exception reporting and locking the DLL.
