# به نام خدا
# Static architecture guardrail for the independent domain boundaries.
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) { throw "FAILED: $message" }
    Write-Host "PASS: $message"
}

function Read-Project([string]$relativePath) {
    return Get-Content -LiteralPath (Join-Path $root $relativePath) -Raw
}

function Read-CSharp([string]$relativeDirectory) {
    $directory = Join-Path $root $relativeDirectory
    return ((Get-ChildItem -LiteralPath $directory -Recurse -Filter '*.cs' |
        Get-Content -Raw) -join "`n")
}

function Assert-NoForbiddenReference([string]$name, [string]$content, [string[]]$patterns) {
    foreach ($pattern in $patterns) {
        Assert-True ($content -notmatch $pattern) "$name does not reference $pattern"
    }
}

$integrationContractsProject = Read-Project 'src/Integration/Hyper.Integration.Contracts/Hyper.Integration.Contracts.csproj'
$integrationDomainProject = Read-Project 'src/Integration/Hyper.Integration.Domain/Hyper.Integration.Domain.csproj'
$integrationContractsSource = Read-CSharp 'src/Integration/Hyper.Integration.Contracts'
$integrationDomainSource = Read-CSharp 'src/Integration/Hyper.Integration.Domain'
$workContractsProject = Read-Project 'src/WorkManagement/Hyper.WorkManagement.Contracts/Hyper.WorkManagement.Contracts.csproj'
$workDomainProject = Read-Project 'src/WorkManagement/Hyper.WorkManagement.Domain/Hyper.WorkManagement.Domain.csproj'
$workContractsSource = Read-CSharp 'src/WorkManagement/Hyper.WorkManagement.Contracts'
$workDomainSource = Read-CSharp 'src/WorkManagement/Hyper.WorkManagement.Domain'

Assert-True ($integrationContractsProject -notmatch '<ProjectReference') `
    'Integration.Contracts has no project dependency'
Assert-True ($workContractsProject -notmatch '<ProjectReference') `
    'WorkManagement.Contracts has no project dependency'
Assert-True ($integrationDomainProject -notmatch '<ProjectReference' -and
    $integrationDomainProject -notmatch 'Hyper.Infrastructure|Hyperyek.Accounting|WorkManagement|Neo\.Bpms') `
    'Integration.Domain has no cross-domain project dependency'
Assert-True ($workDomainProject -match 'Hyper.WorkManagement.Contracts' -and
    $workDomainProject -notmatch 'Hyper.Integration|Hyperyek.Accounting|Hyper.Infrastructure|Neo\.Bpms') `
    'WorkManagement.Domain depends only on its own contracts'

$forbiddenCrossDomain = @('Hyper\.Integration', 'Hyperyek\.Accounting', 'Hyper\.Infrastructure', 'Neo\.Bpms')
Assert-NoForbiddenReference 'Integration.Contracts source' $integrationContractsSource @('Hyperyek\.Accounting', 'Hyper\.Infrastructure', 'WorkManagement', 'Neo\.Bpms')
Assert-NoForbiddenReference 'Integration.Domain source' $integrationDomainSource @('Hyper\.Infrastructure', 'Hyperyek\.Accounting', 'WorkManagement', 'Neo\.Bpms')
Assert-NoForbiddenReference 'WorkManagement.Contracts source' $workContractsSource @('Hyper\.Integration', 'Hyperyek\.Accounting', 'Hyper\.Infrastructure', 'Neo\.Bpms')
Assert-NoForbiddenReference 'WorkManagement.Domain source' $workDomainSource @('Hyper\.Integration', 'Hyperyek\.Accounting', 'Hyper\.Infrastructure', 'Neo\.Bpms')

$integrationInfrastructureProject = Read-Project 'src/Integration/Hyper.Integration.Infrastructure/Hyper.Integration.Infrastructure.csproj'
$integrationApiProject = Read-Project 'src/Integration/Hyper.Integration.Api/Hyper.Integration.Api.csproj'
$adminPanelProject = Read-Project 'src/AdminPanel/Hyper.AdminPanel.Web/Hyper.AdminPanel.Web.csproj'
Assert-True ($integrationInfrastructureProject -match 'Hyper.Integration.Contracts' -and
    $integrationInfrastructureProject -match 'Hyper.Integration.Domain') `
    'Integration.Infrastructure references the Integration contracts and domain'
Assert-True ($integrationApiProject -match 'Hyper.Integration.Contracts' -and
    $integrationApiProject -notmatch 'Hyper.WorkManagement') `
    'Integration.Api does not depend on WorkManagement'
Assert-True ($adminPanelProject -match 'Hyper.WorkManagement.Contracts') `
    'AdminPanel is the allowed WorkManagement consumer boundary'

Write-Host 'Domain dependency checks passed.'
