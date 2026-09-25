# به نام خدا
# Fast, dependency-free guardrail for the Integration boundary.
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Read-Source([string]$relativePath) {
    return Get-Content -Raw (Join-Path $root $relativePath)
}

function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) { throw "FAILED: $message" }
    Write-Host "PASS: $message"
}

$coreContext = Read-Source 'src/Core/Hyper.Infrastructure/Data/Repository/Hyper/HyperSqlServerContext.cs'
$integrationContext = Read-Source 'src/Core/Hyper.Infrastructure/Data/Repository/Hyper/HyperIntegrationContext.cs'
$oauthStore = Read-Source 'src/Integration/Hyper.Integration.Infrastructure/Features/Integrations/BasalamOAuthStore.cs'
$registration = Read-Source 'src/Integration/Hyper.Integration.Infrastructure/Features/Integrations/IntegrationCustomerRegistration.cs'
$shopAccess = Read-Source 'src/Integration/Hyper.Integration.Infrastructure/Features/Integrations/IntegrationShopAccess.cs'
$simulation = Read-Source 'src/Integration/Hyper.Integration.Infrastructure/Features/Integrations/AdminMerchantSimulationService.cs'
$managementController = Read-Source 'src/Integration/Hyper.Integration.Api/IntegrationManagementController.cs'
$operationsController = Read-Source 'src/Integration/Hyper.Integration.Api/IntegrationOperationsController.cs'
$dashboardController = Read-Source 'src/Integration/Hyper.Integration.Api/IntegrationDashboardController.cs'
$scopeAuthorization = Read-Source 'src/Integration/Hyper.Integration.Infrastructure/Features/Integrations/IntegrationScopeAuthorization.cs'
$domainRoot = Join-Path $root 'src/Integration/Hyper.Integration.Domain'
$domainSources = (Get-ChildItem $domainRoot -Recurse -Filter '*.cs' | Get-Content -Raw) -join "`n"
$schema = Read-Source 'docs/schema/ensure-integration-database.sql'
$legacyOAuthMigration = Read-Source 'src/Core/Hyper.Infrastructure/Migrations/20260914_AddExternalOAuthToken.cs'
$legacyMappingMigration = Read-Source 'src/Core/Hyper.Infrastructure/Migrations/20260921_AddIntegrationCustomerMapping.cs'
$coreInfrastructureProject = Read-Source 'src/Core/Hyper.Infrastructure/Hyper.Infrastructure.csproj'
$integrationInfrastructureProject = Read-Source 'src/Integration/Hyper.Integration.Infrastructure/Hyper.Integration.Infrastructure.csproj'

Assert-True ($coreContext -notmatch 'DbSet<.*(Integration|Externalintegration|Inventoryreservation|VwIntegration)') `
    'core HyperSqlServerContext does not expose Integration DbSets'
Assert-True ($coreContext -match 'modelBuilder\.Ignore<SqlExternalintegrationconnections>') `
    'legacy generated Integration mappings are ignored by the core model'
Assert-True ($integrationContext -match 'DbSet<IntegrationCustomerMapping>') `
    'customer mapping is owned by HyperIntegrationContext'
Assert-True ($oauthStore -match 'BasalamOAuthStore\(HyperIntegrationContext') `
    'OAuth store uses the Integration context'
Assert-True ($oauthStore -notmatch 'HyperContextCommand') `
    'OAuth store has no core command-context dependency'
Assert-True ($registration -notmatch 'HyperSqlServerContext|Hyper\.Domain') `
    'Integration customer orchestration depends only on the accounting port'
Assert-True ($shopAccess -notmatch 'HyperSqlServerContext|Hyper\.Domain' -and $simulation -notmatch 'HyperSqlServerContext|Hyper\.Domain') `
    'shop access and simulation orchestration depend only on the platform shop port'
Assert-True ($managementController -match '\[Authorize\]' -and $operationsController -match '\[Authorize\]' -and $dashboardController -match '\[Authorize\]') `
    'management, operations and dashboard API controllers require authentication'
Assert-True ((Read-Source 'src/Integration/Hyper.Integration.Contracts/IntegrationApiContracts.cs') -notmatch 'IntegrationConnectionCreateRequest[\s\S]{0,300}CredentialsJson') `
    'connection create contract does not carry raw credentials'
Assert-True ($scopeAuthorization -match 'integration_scope' -and $scopeAuthorization -notmatch 'X-Shop-Id|X-Tenant-Id') `
    'scope authorization is claim-based and does not trust request headers'
Assert-True ($domainSources -notmatch 'Hyper\.Domain|Hyper\.Infrastructure|HyperSqlServerContext') `
    'Integration Domain has no platform or Infrastructure dependency'
Assert-True ($schema -match 'IntegrationCustomerMappings' -and $schema -match 'IntegrationScenarioJobs' -and $schema -match 'IntegrationConnection') `
    'owned schema provisions the independent Integration database and durable scenario queue'
Assert-True ($legacyOAuthMigration -notmatch 'CreateTable|ExternalOAuthTokens' -and $legacyMappingMigration -notmatch 'CreateTable|IntegrationCustomerMappings') `
    'legacy platform migrations are no-op and cannot create Integration tables'
Assert-True ($coreInfrastructureProject.Contains('Compile Remove="Features\Integrations\**\*.cs"')) `
    'shared Hyper.Infrastructure no longer compiles Integration implementations'
Assert-True ((Test-Path (Join-Path $root 'src/Integration/Hyper.Integration.Infrastructure/Features/Integrations/IntegrationServiceRegistration.cs')) `
    -and -not (Test-Path (Join-Path $root 'src/Core/Hyper.Infrastructure/Features/Integrations/IntegrationServiceRegistration.cs'))) `
    'Integration Infrastructure owns the physical Integration implementation sources'

Write-Host 'Integration architecture checks passed.'
