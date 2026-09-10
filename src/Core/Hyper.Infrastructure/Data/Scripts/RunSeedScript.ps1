# PowerShell Script برای اجرای اسکریپت SeedPointsDistributionData.sql
# این اسکریپت connection string را از appsettings پنل ادمین Hyper می‌خواند و اسکریپت SQL را اجرا می‌کند

param(
    [string]$ScriptPath = "SeedPointsDistributionData.sql"
)

# مسیر فایل‌های appsettings پنل ادمین Hyper
$adminPanelPath = Join-Path $PSScriptRoot "..\..\..\..\AdminPanel\Hyper.AdminPanel.Web"
$appSettingsDevPath = Join-Path $adminPanelPath "appsettings.Development.json"
$appSettingsPath = Join-Path $adminPanelPath "appsettings.json"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Running Seed Points Distribution Data Script" -ForegroundColor Cyan
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
    # جستجو در بخش ConnectionStrings برای "default", "DefaultConnection", یا "Domain"
    # استفاده از regex برای پیدا کردن بخش ConnectionStrings و سپس استخراج connection string
    if ($appSettingsContent -match '(?s)"ConnectionStrings"\s*:\s*\{[^}]*"default"\s*:\s*"([^"]+)"') {
        $connectionString = $matches[1]
        $appSettingsFile = $appSettingsPath
    }
    elseif ($appSettingsContent -match '(?s)"ConnectionStrings"\s*:\s*\{[^}]*"DefaultConnection"\s*:\s*"([^"]+)"') {
        $connectionString = $matches[1]
        $appSettingsFile = $appSettingsPath
    }
    elseif ($appSettingsContent -match '(?s)"ConnectionStrings"\s*:\s*\{[^}]*"Domain"\s*:\s*"([^"]+)"') {
        $connectionString = $matches[1]
        $appSettingsFile = $appSettingsPath
    }
}

# اگر در appsettings.json پیدا نشد، Development را چک می‌کنیم
if (-not $connectionString -and (Test-Path $appSettingsDevPath)) {
    Write-Host "Reading connection string from: appsettings.Development.json" -ForegroundColor Yellow
    try {
        $appSettings = Get-Content $appSettingsDevPath | ConvertFrom-Json
        $connectionString = $appSettings.ConnectionStrings.DefaultConnection
        if ($connectionString) {
            $appSettingsFile = $appSettingsDevPath
        }
    }
    catch {
        Write-Host "Warning: Could not parse Development appsettings, trying regex..." -ForegroundColor Yellow
        $appSettingsContent = Get-Content $appSettingsDevPath -Raw
        if ($appSettingsContent -match '"DefaultConnection"\s*:\s*"([^"]+)"') {
            $connectionString = $matches[1]
            $appSettingsFile = $appSettingsDevPath
        }
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
Write-Host "Connection string (first 50 chars): $($connectionString.Substring(0, [Math]::Min(50, $connectionString.Length)))..." -ForegroundColor Gray

# فرمت 1: Server=localhost,1433;Database=HyperBpmsDev;User Id=sa;Password=...
if ($connectionString -match "Server=([^;]+)") {
    $server = $matches[1]
}
# فرمت 2: Data Source=.;Initial Catalog=Hyperyek;User ID=sa;Password=...
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

# Password (باید تا آخر string باشد، نه فقط تا ;)
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
    Write-Host "Connection string format not recognized." -ForegroundColor Yellow
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
    Invoke-Sqlcmd -ConnectionString $sqlConnectionString -Query $sqlScript -QueryTimeout 30 -ErrorAction Stop
    
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Green
    Write-Host "Script executed successfully!" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Pie chart should now have 5 different sections:" -ForegroundColor Cyan
    Write-Host "  1. XP Hyper: ~10,000,000 (largest)" -ForegroundColor White
    Write-Host "  2. Purchase Points: ~1,800,000" -ForegroundColor White
    Write-Host "  3. Referral Points: ~800,000" -ForegroundColor White
    Write-Host "  4. Survey Points: ~450,000" -ForegroundColor White
    Write-Host "  5. Lifetime Value Points: ~1,200,000" -ForegroundColor White
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
    Write-Host "  4. TenantId, CustomerTenantId, EventLogId values in script are correct" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}


