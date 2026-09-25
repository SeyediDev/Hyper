# به نام خدا
# Verifies that Neo.Bpms is restricted to the AdminPanel boundary.
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) { throw "FAILED: $message" }
    Write-Host "PASS: $message"
}

$srcRoot = Join-Path $root 'src'
$nonPanelRoots = @(
    (Join-Path $srcRoot 'Core'),
    (Join-Path $srcRoot 'Integration')
)
$nonPanelFiles = foreach ($dir in $nonPanelRoots) {
    if (Test-Path $dir) { Get-ChildItem $dir -Recurse -File -Include *.cs,*.csproj,*.props,*.targets }
}
$bpmsDirectReferences = @($nonPanelFiles | Select-String -Pattern 'Neo\.Bpms' -SimpleMatch:$false)

if ($bpmsDirectReferences.Count -gt 0) {
    Write-Host 'Detected Neo.Bpms references outside AdminPanel:'
    $bpmsDirectReferences | ForEach-Object { Write-Host (" - {0}:{1}" -f $_.Path, $_.LineNumber) }
}

Assert-True ($bpmsDirectReferences.Count -eq 0) `
    'Neo.Bpms has no direct source or project reference outside AdminPanel'

$integrationProjects = Get-ChildItem (Join-Path $srcRoot 'Integration') -Recurse -Filter '*.csproj'
foreach ($project in $integrationProjects) {
    $content = Get-Content -LiteralPath $project.FullName -Raw
    Assert-True ($content -notmatch 'Neo\.Bpms') `
        "Integration project $($project.Name) does not reference Neo.Bpms"
}

Write-Host 'Neo boundary checks passed.'
