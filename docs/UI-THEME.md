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

## Generated list controls (2026-09-27)

The host's later `!important` overrides still painted list toolbar buttons and
active page numbers charcoal, with dark accents on top. The final scoped list
adapter in `custom-theme.css` now uses semantic surface/text/accent/on-accent
pairs for the toolbar, add button and pagination, including hover/focus states.
Outline SVGs keep `fill: none`; this fixes the grid/settings glyph becoming a
solid square. Column filter layout, 28px buttons and 16px stroked SVGs work with
both earlier classless and current Razor SVGs even when deployed RTL/LTR bundles
lack the newer control rules. Existing header-filter click handlers are retained.

`List.browser.test.cjs` combines the actual host custom CSS and deployed RTL
bundle with Neo's bridge and ColumnFilter.js. It covers all eight palettes at
390/1366px, contrast, outline rendering, single-row header alignment, filter focus,
active-state updates and absence of accidental sorting/form submission. The
contrast helper handles the browser's `color(srgb ...)` values from color-mix.
Sample-only DOM is used, not live shop records. Load the static CSS with Ctrl+F5;
no C# change or IIS restart is required for this fix. Skill/MCP contracts unchanged.

## Report filter delivery and legacy forms (2026-09-27)

The running HTTPS host returned 404 for the ColumnFilter.js URL requested by
form, report and dashboard Views. CopyFilterAssets referenced a nonexistent
Neo-Bpms wwwroot folder. It now copies the canonical CommonAssets script. Linked
Content also includes that file in build/publish output, including a clean checkout.
The generated host copy is ignored. After copying, the same public URL returned
200 JavaScript and the host/source SHA256 hashes matched. MSBuild Content evaluation
confirmed one linked item with PreserveNewest for output and publishing.

Report has a separate legacy anchor toolbar. Its hard-coded green failed the
isolated contrast check at 2.96:1. The host adapter now gives those controls semantic
surface/accent, hover and focus colors without turning outlined SVGs into solids.
The Report filter surface and apply button use matching semantic colors.

Neo-Bpms also clamps the Report popup to viewport dimensions instead of imposing
1800/2160px minimum widths, and corrects six legacy form Views (bulk create/edit/work
item, command, custom and iframe) to the same versioned controls-modern.css URL as
Report. The previous legacy CSS URL returned 404; the shared URL returned 200.
Those Razor changes require rebuilding the host with the updated Neo-Bpms dependency.
Static host CSS and the copied filter script can be loaded now with Ctrl+F5.

Report.browser.test.cjs is isolated sample markup, not authenticated UI acceptance.
These fixes do not change Skill/MCP contracts, so no tool schema was rewritten.

The full Report inline CSS exposed another runtime issue: focus ran during the
visibility transition's initial hidden frame. ColumnFilter now waits for layout
before choosing/focusing the existing control, still waiting for jQuery animations
and preserving widget instances and values. The Report fixture reproduced the
failure before the fix and passed the mobile/desktop focus checks after it.

Validation completed on 2026-09-28: the combined Edge run passed all 33 tests
(17 shared column-filter tests, 16 Report palette/viewport cases), exit code 0.
Neo.Bpms.UI.MVC Compile target passed with zero warnings/errors and existing
references. Hyper Compile was blocked by CS0103 (`api` undefined) in the concurrently
modified WorkManagementPageController.cs:25; that unrelated file was not changed
by this task. No full host rebuild/deployment or authenticated Report acceptance
is claimed. Earlier exploratory test runs exposed the visibility timing failure;
final tests use the bounded frame wait and close their own browser connection.
The csproj delivery change was included in concurrent Hyper commit b1246cd;
the remaining owned changes are committed separately.
