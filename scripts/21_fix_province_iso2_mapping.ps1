# PowerShell script to execute SQL script for fixing province ISO2 codes
# This script reads the connection string from appsettings and executes the SQL script

$ErrorActionPreference = "Stop"

# Get the script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlScriptPath = Join-Path $scriptDir "21_fix_province_iso2_mapping.sql"

# Read connection string from appsettings.Development.json
$appSettingsPath = Join-Path $scriptDir "..\src\AdminPanel\Hyper.AdminPanel.Web\appsettings.Development.json"
if (-not (Test-Path $appSettingsPath)) {
    Write-Error "appsettings.Development.json not found at: $appSettingsPath"
    exit 1
}

$appSettings = Get-Content $appSettingsPath | ConvertFrom-Json
$connectionString = $appSettings.ConnectionStrings.DefaultConnection

if ([string]::IsNullOrWhiteSpace($connectionString)) {
    Write-Error "Connection string not found in appsettings"
    exit 1
}

# Try Integrated Security first if SQL auth fails
$useIntegratedSecurity = $false
if ($connectionString -match 'User Id=sa') {
    Write-Host "SQL Authentication detected. Trying Integrated Security as fallback..." -ForegroundColor Yellow
    $useIntegratedSecurity = $true
    # Replace SQL auth with Integrated Security
    $connectionString = $connectionString -replace 'User Id=[^;]+;', '' -replace 'Password=[^;]+;', '' -replace ';', ';Integrated Security=True;'
}

Write-Host "Connecting to database..." -ForegroundColor Cyan
Write-Host "Connection String: $($connectionString -replace 'Password=[^;]+', 'Password=***')" -ForegroundColor Gray

# Read SQL script
if (-not (Test-Path $sqlScriptPath)) {
    Write-Error "SQL script not found at: $sqlScriptPath"
    exit 1
}

$sqlScript = Get-Content $sqlScriptPath -Raw -Encoding UTF8

# Execute SQL script using System.Data.SqlClient
try {
    Add-Type -AssemblyName System.Data
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Connected successfully!" -ForegroundColor Green
    Write-Host "Executing SQL script..." -ForegroundColor Cyan
    
    # Split script by GO statements and execute each batch
    $batches = $sqlScript -split '\bGO\b', [System.StringSplitOptions]::RemoveEmptyEntries
    
    foreach ($batch in $batches) {
        $batch = $batch.Trim()
        if ([string]::IsNullOrWhiteSpace($batch)) {
            continue
        }
        
        $command = $connection.CreateCommand()
        $command.CommandText = $batch
        $command.CommandTimeout = 300
        
        try {
            # For SELECT statements, read and display results
            if ($batch -match '^\s*SELECT') {
                $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
                $dataset = New-Object System.Data.DataSet
                $adapter.Fill($dataset) | Out-Null
                
                if ($dataset.Tables.Count -gt 0 -and $dataset.Tables[0].Rows.Count -gt 0) {
                    Write-Host "`nResults:" -ForegroundColor Yellow
                    $dataset.Tables[0] | Format-Table -AutoSize
                }
            } else {
                $rowsAffected = $command.ExecuteNonQuery()
                if ($rowsAffected -gt 0) {
                    Write-Host "Rows affected: $rowsAffected" -ForegroundColor Green
                }
            }
        } catch {
            Write-Warning "Error executing batch: $_"
            Write-Host "Batch: $($batch.Substring(0, [Math]::Min(200, $batch.Length)))..." -ForegroundColor Gray
        } finally {
            if ($command) { $command.Dispose() }
        }
    }
    
    Write-Host "`nScript executed successfully!" -ForegroundColor Green
    
} catch {
    Write-Error "Failed to execute script: $_"
    exit 1
} finally {
    if ($connection -and $connection.State -eq [System.Data.ConnectionState]::Open) {
        $connection.Close()
        Write-Host "Connection closed." -ForegroundColor Gray
    }
}


