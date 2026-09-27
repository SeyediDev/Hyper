# Hyper palette regression checks

The console executable compiles the actual ThemeService, Theme and preference enum
as linked sources, with no database, host or package dependencies. It checks
opaque light surfaces and >= 4.5:1 text contrast, including primary accents on
tints and button hover/active states, for every persisted preference.

From Backend (requires .NET 10):

```powershell
dotnet run --project tests/Hyper.ThemePalette.Tests -- "$env:TEMP/hyper-theme-palettes.json"
```

The optional JSON export feeds the isolated browser fixtures. Use Node and
Playwright from the existing sibling Neo-Bpms frontend test package (install its
locked dependencies with `npm ci` there if needed); Edge must be installed:

```powershell
$env:NODE_PATH = 'E:/SJVS/Projects/Neo-Bpms/tests/Neo.Bpms.UI.MVC.Tests/Frontend/node_modules'
$env:NEO_TEST_BROWSER = 'msedge'
node --test tests/Hyper.ThemePalette.Tests/Palette.browser.test.cjs
```

Set `NEO_BPMS_ROOT` for a different sibling checkout and `HYPER_PALETTES` for a
different export path. Fixtures read the real Razor variable mapping, legacy
Neo dashboard CSS, Bootstrap and shared theme bridge/adapter. They use sample
markup and `page.setContent`; no server, authentication, credentials or production
data are involved. A sample-only teal screenshot is written to the OS temporary
directory as `hyper-teal-light-fixture.png`. It is not a real panel screenshot.

The successful fixture run is separate from live authenticated acceptance and
from a full Hyper dependency-graph build. Do not treat it as either.

## Real Mmenu regression

After generating the same palette JSON, run:

```powershell
node --test tests/Hyper.ThemePalette.Tests/Menu.browser.test.cjs
```

This suite loads Hyper's actual Mmenu plugin, custom-theme.css, RTL/UX styles and
Neo theme bridge into sample DOM matching the host's RTL body. It covers all eight
palettes in light/mobile and dark/desktop configurations, menu contrast and open
bounds, submenu/back/search, focus and live palette changes. It does not contact
a running host. Assertions use computed styles, viewport bounds and plugin interactions;
no real user navigation or credentials are captured.

## Generated list toolbar, pagination and header controls

```powershell
node --test tests/Hyper.ThemePalette.Tests/List.browser.test.cjs
```

Uses the same palette JSON and Node/Playwright setup. Tests eight palettes at
mobile/desktop widths, actual deployed host CSS, semantic bridge and column-filter
script. Verifies contrast and outline SVGs as well as column-filter focus/state,
header alignment and no unintended sort or submit. Sample data only.
