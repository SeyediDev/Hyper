# ===============================================
# Development Startup Script
# Run: .\start-dev.ps1
# ===============================================

Write-Host "🚀 Starting Hyper Backend Services..." -ForegroundColor Green
Write-Host ""

# Colors
$ErrorColor = "Red"
$SuccessColor = "Green"
$InfoColor = "Cyan"

# Paths
$BackendRoot = $PSScriptRoot
$CustomerPortalApi = Join-Path $BackendRoot "src\CustomerPortal\Hyper.CustomerPortal.Api"
$AdminPanel = Join-Path $BackendRoot "src\AdminPanel\Hyper.AdminPanel.Web"
$ChannelApi = Join-Path $BackendRoot "src\Channel\Hyper.Channel.Api"

# Check if SQL Server is running
Write-Host "📊 Checking SQL Server..." -ForegroundColor $InfoColor
try {
    $sqlTest = Invoke-Sqlcmd -Query "SELECT 1" -ServerInstance "localhost" -Database "master" -TrustServerCertificate -ErrorAction Stop 2>&1
    Write-Host "✅ SQL Server is running" -ForegroundColor $SuccessColor
} catch {
    Write-Host "⚠️  SQL Server not detected. Using LocalDB..." -ForegroundColor Yellow
    Write-Host "   Make sure SQL Server Express or LocalDB is installed" -ForegroundColor Yellow
}

Write-Host ""

# Function to start a service in a new window
function Start-Service {
    param(
        [string]$Name,
        [string]$Path,
        [string]$Port,
        [string]$Color = "Cyan"
    )
    
    Write-Host "🔄 Starting $Name on port $Port..." -ForegroundColor $Color
    
    $command = "cd '$Path'; dotnet run --urls=http://localhost:$Port"
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", $command `
        -WindowStyle Normal
    
    Write-Host "✅ $Name started in new window (Port: $Port)" -ForegroundColor $SuccessColor
}

# Start services
Write-Host "=" * 60 -ForegroundColor Gray
Write-Host ""

# Customer Portal API
Start-Service -Name "Customer Portal API" -Path $CustomerPortalApi -Port "5000" -Color "Cyan"
Start-Sleep -Seconds 2

# Admin Panel
Start-Service -Name "Admin Panel Web" -Path $AdminPanel -Port "5001" -Color "Magenta"
Start-Sleep -Seconds 2

# Channel API (Optional)
$startChannel = Read-Host "Start Channel API? (y/n) [default: n]"
if ($startChannel -eq "y") {
    Start-Service -Name "Channel API" -Path $ChannelApi -Port "5003" -Color "Yellow"
    Start-Sleep -Seconds 2
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Gray
Write-Host ""
Write-Host "🎉 Services Started!" -ForegroundColor $SuccessColor
Write-Host ""
Write-Host "📍 URLs:" -ForegroundColor $InfoColor
Write-Host "   Customer Portal API: http://localhost:5000" -ForegroundColor White
Write-Host "   Customer Portal Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "   Admin Panel: http://localhost:5001" -ForegroundColor White
if ($startChannel -eq "y") {
    Write-Host "   Channel API: http://localhost:5003" -ForegroundColor White
    Write-Host "   Channel API Swagger: http://localhost:5003/swagger" -ForegroundColor White
}
Write-Host ""
Write-Host "⚙️  Next Steps:" -ForegroundColor $InfoColor
Write-Host "   1. Start Frontend: cd D:\Projects\Hyper\Frontend\CustomerPortal\Hyper.CustomerPortal.Web && npm run dev" -ForegroundColor White
Write-Host "   2. Open http://localhost:3000 in browser" -ForegroundColor White
Write-Host ""
Write-Host "💡 Tip: Each service runs in a separate PowerShell window" -ForegroundColor Yellow
Write-Host "    Close windows to stop services" -ForegroundColor Yellow
Write-Host ""

# Ask if user wants to start frontend
$startFrontend = Read-Host "Start Frontend (Next.js)? (y/n) [default: y]"
if ($startFrontend -ne "n") {
    $frontendPath = "D:\Projects\Hyper\Frontend\CustomerPortal\Hyper.CustomerPortal.Web"
    
    if (Test-Path $frontendPath) {
        Write-Host ""
        Write-Host "🎨 Starting Frontend..." -ForegroundColor $InfoColor
        
        # Check if node_modules exists
        $nodeModulesPath = Join-Path $frontendPath "node_modules"
        if (-not (Test-Path $nodeModulesPath)) {
            Write-Host "📦 Installing npm packages (first time)..." -ForegroundColor Yellow
            Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$frontendPath'; npm install; npm run dev" -WindowStyle Normal
        } else {
            Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$frontendPath'; npm run dev" -WindowStyle Normal
        }
        
        Write-Host "✅ Frontend started at http://localhost:3000" -ForegroundColor $SuccessColor
    } else {
        Write-Host "⚠️  Frontend not found at: $frontendPath" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")



