# PowerShell Script to Execute Promotion Scenarios Data Setup
# This script executes PromotionScenarios_DataSetup.sql

param(
    [string]$ConnectionString = "",
    [string]$OutputFile = "promotion_scenarios_data_setup_output.txt"
)

# Get connection string from appsettings.json if not provided
if ([string]::IsNullOrEmpty($ConnectionString)) {
    $appSettingsPath = "Backend\src\AdminPanel\Hyper.AdminPanel.Web\appsettings.json"
    if (Test-Path $appSettingsPath) {
        $appSettings = Get-Content $appSettingsPath | ConvertFrom-Json
        $ConnectionString = $appSettings.ConnectionStrings.Domain
    }
    else {
        Write-Host "Error: Connection string not provided and appsettings.json not found" -ForegroundColor Red
        exit 1
    }
}

if ([string]::IsNullOrEmpty($ConnectionString)) {
    Write-Host "Error: Connection string is empty" -ForegroundColor Red
    exit 1
}

$scriptPath = Join-Path $PSScriptRoot "PromotionScenarios_DataSetup.sql"

if (-not (Test-Path $scriptPath)) {
    Write-Host "Error: SQL script not found at $scriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Promotion Scenarios Data Setup" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Script: $scriptPath" -ForegroundColor Yellow
Write-Host "Output: $OutputFile" -ForegroundColor Yellow
Write-Host ""

try {
    # Load SQL Server module
    Import-Module SqlServer -ErrorAction SilentlyContinue
    
    if (-not (Get-Module -Name SqlServer)) {
        Write-Host "Installing SqlServer module..." -ForegroundColor Yellow
        Install-Module -Name SqlServer -Scope CurrentUser -Force -AllowClobber
        Import-Module SqlServer
    }

    # Read SQL script
    $sqlScript = Get-Content $scriptPath -Raw -Encoding UTF8

    Write-Host "Executing SQL script..." -ForegroundColor Green
    
    # Execute script and capture output
    $result = Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $sqlScript -QueryTimeout 0 -ErrorAction Stop
    
    # Save output to file
    $result | Out-File -FilePath $OutputFile -Encoding UTF8
    
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host "Data setup completed successfully!" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host "Output saved to: $OutputFile" -ForegroundColor Yellow
    Write-Host ""
    
    # Show last 50 lines of output
    Write-Host "Last 50 lines of output:" -ForegroundColor Cyan
    Get-Content $OutputFile | Select-Object -Last 50
    
    return $true
}
catch {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "Data setup failed!" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    
    if (Test-Path $OutputFile) {
        Write-Host "Error details saved to: $OutputFile" -ForegroundColor Yellow
        Get-Content $OutputFile | Select-Object -Last 50
    }
    
    return $false
}


