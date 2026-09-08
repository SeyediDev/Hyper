# PowerShell Script برای اجرای اسکریپت SeedRewardAssetData.sql
# این اسکریپت connection string را از appsettings پنل ادمین Hyper می‌خواند و اسکریپت SQL را اجرا می‌کند

param(
    [string]$ScriptPath = "SeedRewardAssetData.sql"
)

# مسیر فایل‌های appsettings پنل ادمین Hyper
$adminPanelPath = Join-Path $PSScriptRoot "..\..\..\..\AdminPanel\Hyper.AdminPanel.Web"
$appSettingsDevPath = Join-Path $adminPanelPath "appsettings.Development.json"
$appSettingsPath = Join-Path $adminPanelPath "appsettings.json"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Running Seed RewardAsset Data Script" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# خواندن connection string از appsettings (اول appsettings.json اصلی، سپس Development)
$connectionString = $null
$appSettingsFile = $null

# اول appsettings.json اصلی را چک می‌کنیم (برای پروژه Hyper)
# استفاده از regex برای استخراج connection string بدون نیاز به parse کامل JSON (به خاطر کامنت‌ها)
if (Test-Path $appSettingsPath) {
    Write-Host "Reading connection string from: appsettings.json" -ForegroundColor Yellow
    $appSettingsContent = Get-Content $appSettingsPath -Raw
    
    # استخراج connection string با regex (پشتیبانی از کامنت‌های JSON)
    # جستجو در بخش ConnectionStrings برای "Domain" connection string
    if ($appSettingsContent -match '(?s)"ConnectionStrings"\s*:\s*\{[^}]*"Domain"\s*:\s*"([^"]+)"') {
        $connectionString = $matches[1]
        $appSettingsFile = $appSettingsPath
        Write-Host "Found 'Domain' connection string" -ForegroundColor Green
    }
    elseif ($appSettingsContent -match '(?s)"ConnectionStrings"\s*:\s*\{[^}]*"default"\s*:\s*"([^"]+)"') {
        $connectionString = $matches[1]
        $appSettingsFile = $appSettingsPath
        Write-Host "Using 'default' connection string as fallback" -ForegroundColor Yellow
    }
}

# اگر در appsettings.json پیدا نشد، Development را چک می‌کنیم
if (-not $connectionString -and (Test-Path $appSettingsDevPath)) {
    Write-Host "Reading connection string from: appsettings.Development.json" -ForegroundColor Yellow
    try {
        $appSettingsContent = Get-Content $appSettingsDevPath -Raw
        if ($appSettingsContent -match '(?s)"ConnectionStrings"\s*:\s*\{[^}]*"Domain"\s*:\s*"([^"]+)"') {
            $connectionString = $matches[1]
            $appSettingsFile = $appSettingsDevPath
            Write-Host "Found 'Domain' connection string in Development" -ForegroundColor Green
        }
        elseif ($appSettingsContent -match '(?s)"ConnectionStrings"\s*:\s*\{[^}]*"default"\s*:\s*"([^"]+)"') {
            $connectionString = $matches[1]
            $appSettingsFile = $appSettingsDevPath
            Write-Host "Using 'default' connection string as fallback" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "Warning: Could not parse Development appsettings" -ForegroundColor Yellow
    }
}

if (-not $connectionString) {
    Write-Host "Error: Connection string not found in appsettings files!" -ForegroundColor Red
    Write-Host "Checked files:" -ForegroundColor Yellow
    Write-Host "  - $appSettingsDevPath" -ForegroundColor Yellow
    Write-Host "  - $appSettingsPath" -ForegroundColor Yellow
    exit 1
}

Write-Host "Using connection string from: $appSettingsFile" -ForegroundColor Green
Write-Host ""

# استخراج اطلاعات از connection string (پشتیبانی از فرمت‌های مختلف)
$server = $null
$database = $null
$user = $null
$password = $null

Write-Host "Parsing connection string..." -ForegroundColor Yellow

# فرمت 1: Server=localhost,1433;Database=HyperBpmsDev;User Id=sa;Password=...
if ($connectionString -match "Server=([^;]+)") {
    $server = $matches[1]
}
# فرمت 2: Data Source=.;Initial Catalog=Hyper;User ID=sa;Password=...
elseif ($connectionString -match "Data Source=([^;]+)") {
    $server = $matches[1]
}

# Database
if ($connectionString -match "Database=([^;]+)") {
    $database = $matches[1]
}
elseif ($connectionString -match "Initial Catalog=([^;]+)") {
    $database = $matches[1]
}

# User (پشتیبانی از User Id و User ID)
if ($connectionString -match "User Id=([^;]+)") {
    $user = $matches[1]
}
elseif ($connectionString -match "User ID=([^;]+)") {
    $user = $matches[1]
}

# Password
if ($connectionString -match "Password=([^;]+)") {
    $password = $matches[1]
    # حذف TrustServerCertificate و سایر پارامترها از password
    if ($password -match "^([^;]+)") {
        $password = $matches[1]
    }
}

if (-not $server -or -not $database -or -not $user -or -not $password) {
    Write-Host "Error: Could not parse connection string!" -ForegroundColor Red
    Write-Host "Parsed values:" -ForegroundColor Yellow
    Write-Host "  Server: $server" -ForegroundColor Yellow
    Write-Host "  Database: $database" -ForegroundColor Yellow
    Write-Host "  User: $user" -ForegroundColor Yellow
    Write-Host "  Password: $(if($password){'***'}else{'NOT FOUND'})" -ForegroundColor Yellow
    exit 1
}

Write-Host "Connection Details:" -ForegroundColor Cyan
Write-Host "  Server: $server" -ForegroundColor White
Write-Host "  Database: $database" -ForegroundColor White
Write-Host "  User: $user" -ForegroundColor White
Write-Host ""

# Check if SqlServer module exists
if (-not (Get-Module -ListAvailable -Name SqlServer)) {
    Write-Host "Installing SqlServer module..." -ForegroundColor Yellow
    Install-Module -Name SqlServer -Scope CurrentUser -Force -AllowClobber
}

Import-Module SqlServer -ErrorAction SilentlyContinue

# Reading SQL script
$scriptPath = Join-Path $PSScriptRoot $ScriptPath
if (-not (Test-Path $scriptPath)) {
    Write-Host "Error: SQL script file not found: $scriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "Reading SQL script from: $scriptPath" -ForegroundColor Yellow
$sqlScript = Get-Content $scriptPath -Raw -Encoding UTF8

# Building connection string for Invoke-Sqlcmd
$sqlConnectionString = "Server=$server;Database=$database;User Id=$user;Password=$password;TrustServerCertificate=True;"

Write-Host ""
Write-Host "Executing SQL script..." -ForegroundColor Yellow
Write-Host ""

try {
    # Execute script
    Invoke-Sqlcmd -ConnectionString $sqlConnectionString -Query $sqlScript -QueryTimeout 60 -ErrorAction Stop
    
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Green
    Write-Host "Script executed successfully!" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "RewardAsset data has been created successfully!" -ForegroundColor Cyan
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Red
    Write-Host "Error executing script:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "================================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please make sure:" -ForegroundColor Yellow
    Write-Host "  1. SQL Server is running" -ForegroundColor Yellow
    Write-Host "  2. Connection string is correct" -ForegroundColor Yellow
    Write-Host "  3. User has access to database" -ForegroundColor Yellow
    Write-Host "  4. Rewards and Customers exist in database" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}



































