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

## Native color palettes (2026-09-27)

All eight persisted `ThemePreference` values select a **light color family**.
`ThemeService` now supplies white, opaque cards/inputs, a softly tinted canvas
and title strip, and dark neutral text. Primary accents use darker shades of
lime/green, purple, orange, red, teal, indigo, emerald and cyan so counters, links
and white button labels remain readable. Success/warning/danger/info colors and
secondary series colors retain their existing values. No preference IDs, saved
user choices, API shape or business behavior changed.

Previously every family emitted near-black canvas/input colors and translucent
cards, and completed chart/report colors as dark. The shared bridge correctly
classified these native palettes as dark; the mismatch was in the host palette.
The native Razor variable block now also outranks the legacy OS-dark selector
before the theme API responds. This avoids a dark first palette when the OS is
dark. Neo's standalone light/dark/system preferences remain supported; Hyper does
not currently expose a separate native dark preference.

Validation: 683 source-linked C# palette assertions and 16 isolated Edge cases
(eight real palettes × two OS modes) pass. Cases load the actual legacy dashboard
CSS, Bootstrap, shared bridge and MVC adapter; cover native CSS precedence before
JavaScript, text/button/tab/counter contrast, both header gradient endpoints,
white cards, unchanged sample chart fill and draft retention on palette changes.
The teal sample also exercises a narrow viewport. Domain compilation passed with
zero warnings/errors using existing dependency outputs. These fixtures do not
validate authenticated dashboards, real charts/data, or every generated control.
See [repeatable commands](../tests/Hyper.ThemePalette.Tests/README.md).

Rebuild/restart the AdminPanel host to load the changed Hyper.Domain assembly,
then refresh the page. A process already running at port 44301 keeps its old
assembly until restarted; this change does not stop the user's Visual Studio.
No Skill/MCP contract changed: this is a native host palette correction, not a
new shared Neo theme API or tool.

Host validation detail: the normal targeted AdminPanel build could not copy the
new Hyper.Domain.dll into the running host (MSB3027/MSB3021: Visual Studio and IIS
Express held the destination). A subsequent `dotnet build ... --no-restore
--disable-build-servers -m:1 -t:Compile -p:BuildProjectReferences=false
-p:UseSharedCompilation=false` passed with zero warnings/errors. This verifies
the compile target, including Razor, without replacing the running host binary;
it is not a deployment or a successful complete host build.

## Mmenu palette regression (2026-09-27)

The native light palettes exposed a separate host override in
`wwwroot/Content/custom-theme.css`: menu text remained `#e0e0e0`, while navbar,
search wrapper and footer retained a `#2a2a2a` background. Those menu surface,
text and border tokens now reference the native palette. The search input itself
is styled (not only its wrapper), navigation glyphs use current text color, and
both legacy/current Mmenu selected-item classes receive the same styles. The
main Layout versions the CSS URL so subsequent builds invalidate stale caches.

`Menu.browser.test.cjs` reproduces the original mismatch with the real host
Mmenu JavaScript, custom CSS, RTL assets and the Neo bridge. The teal regression
initially failed at 1.39:1 navbar contrast. The corrected fixture checks all eight
palettes on light/mobile and dark/desktop OS configurations: opening inside the
viewport, readable text/chrome/input/selection, hover, submenu/back navigation,
search results, keyboard focus and a live palette change. Search assertions use
the opened panel because Mmenu keeps offscreen original links and clones results.
This is isolated sample navigation, not authenticated panel acceptance. No shared
Neo Skill or MCP contract changes are needed for the host-only CSS correction.

To see this CSS-only menu fix in the currently running local host, hard-refresh
the page (Ctrl+F5). Unlike the earlier C# palette change, the CSS content does not
require restarting IIS Express. The versioned Razor URL takes effect on the next
host rebuild; hard refresh also handles the existing unversioned URL.

The host Compile target passed with zero warnings/errors using existing dependency
outputs. Menu acceptance here is based on computed colors, geometry and actual
plugin interactions. Off-canvas fixture raster captures were not reliable and
are not used as visual evidence; the user's authenticated screen still needs a
refresh/visual check.
