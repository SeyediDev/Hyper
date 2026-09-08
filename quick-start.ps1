# ===============================================
# Quick Start - Customer Portal Only
# ===============================================

Write-Host "`n🚀 Quick Start - Customer Portal`n" -ForegroundColor Cyan

$CustomerPortalApi = "$PSScriptRoot\src\CustomerPortal\Hyper.CustomerPortal.Api"
$Frontend = "D:\Projects\Hyper\Frontend\CustomerPortal\Hyper.CustomerPortal.Web"

Write-Host "Starting Customer Portal API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$CustomerPortalApi'; Write-Host 'Customer Portal API' -ForegroundColor Cyan; dotnet run --urls=http://localhost:5000"

Write-Host "`n✅ Customer Portal API starting on http://localhost:5000`n" -ForegroundColor Green

Start-Sleep -Seconds 3

if (Test-Path $Frontend) {
    Write-Host "Starting Frontend (Next.js)..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$Frontend'; Write-Host 'Customer Portal Frontend' -ForegroundColor Magenta; npm run dev"
    Write-Host "`n✅ Frontend starting on http://localhost:3000`n" -ForegroundColor Green
} else {
    Write-Host "⚠️  Frontend path not found: $Frontend" -ForegroundColor Yellow
}

Write-Host "`n📍 URLs:" -ForegroundColor Cyan
Write-Host "   API: http://localhost:5000" -ForegroundColor White
Write-Host "   Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "   Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "`n💡 Close PowerShell windows to stop services`n" -ForegroundColor Yellow



