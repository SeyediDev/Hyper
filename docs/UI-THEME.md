# Shared Neo theme in Hyper AdminPanel

Hyper uses two Razor shells. Both must load the canonical assets exposed by
Neo.Bpms.UI.MVC; updating the library alone does not update host overrides.

- `Views/Shared/Layout/CommonIncludes.cshtml` loads `neo-theme.css`,
  `neo-theme-mvc.css` and `neo-theme.js` after the legacy base assets.
- `Views/Shared/_HyperAdminLayout.cshtml` loads `neo-theme.css`, then
  `hyper-admin.css`, `hyper-admin-theme.css`, the native `_ThemeVariables` partial
  (outside preview), and `neo-theme.js`.
- `_ThemeVariables` remains the palette authority. Do not add another theme store.
  The native theme selector retains its existing reload behavior.
- The shell adapter maps surfaces, inputs, tables, text and focus to semantic
  Neo tokens. Chart series and status meanings remain unchanged. Grid cards allow
  tables to scroll internally on narrow screens rather than widening the page.

The existing `/AdminDashboard/Preview` is anonymous only in Development and uses
sample data. It deliberately skips session palette lookup and stays independent
of business APIs. Never expose this preview in production or use it as proof that
protected workflows have been validated.

## Validation (2026-09-26)

The AdminPanel built and ran in Development on `http://localhost:5000` with the
existing local SQL option. Login and the three Neo theme assets returned HTTP 200;
asset contents matched the library source. The user signed in and the real main
dashboard displayed. No business data mutation was performed for this UI check.

Three Edge tests passed against the local preview: light/dark text contrast,
unchanged chart-series colors, mobile menu/keyboard focus, independently scrollable
tables and 30-day period navigation. The first mobile run found a grid min-content
overflow; the scoped card fix passed the rerun. Sample screenshots were inspected.

Run from the sibling Neo-Bpms repository's existing frontend test package:

```powershell
Set-Location E:/SJVS/Projects/Neo-Bpms/tests/Neo.Bpms.UI.MVC.Tests/Frontend
$env:NEO_HYPER_URL = 'http://localhost:5000'
$env:NEO_TEST_BROWSER = 'msedge'
npm run test:hyper-theme
```

The host must already be running in Development. Tests enforce a loopback URL.
Preview coverage is separate from authenticated report acceptance, data operations,
production cache/CSP and exhaustive host palette coverage. Never commit session
cookies, real account screenshots or browser profiles as test fixtures.

Final host-override check: after adding the main-shell includes, a targeted
`dotnet build ... --no-restore --disable-build-servers -m:1
-p:BuildProjectReferences=false -p:UseSharedCompilation=false` succeeded with zero
warnings/errors using existing dependency outputs. A preceding full-graph rebuild
encountered access-denied errors in Neo.Application's temporary build files; it
was stopped without changing permissions or other workers' processes.
The rebuilt executable returned HTTP 200 for login and preview, with canonical
assets and the host adapter present. The signed-in dashboard had been observed
before this restart; a post-restart authenticated report/filter acceptance run
was not completed because the in-app browser tab could not reattach. This remains
separate from the passing isolated column-filter regression suite.
