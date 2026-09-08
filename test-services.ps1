# Test Services Script
Write-Host "`n==================================" -ForegroundColor Cyan
Write-Host "   Testing Hyper Services" -ForegroundColor Cyan
Write-Host "==================================`n" -ForegroundColor Cyan

# Test Customer Portal API
Write-Host "1. Testing Customer Portal API..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "   ✅ Customer Portal API is UP (Port 5000)" -ForegroundColor Green
    }
} catch {
    Write-Host "   ❌ Customer Portal API is DOWN (Port 5000)" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# Test Admin Panel
Write-Host "`n2. Testing Admin Panel..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5001" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    Write-Host "   ✅ Admin Panel is UP (Port 5001)" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Admin Panel is DOWN (Port 5001)" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# Test Frontend
Write-Host "`n3. Testing Frontend..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:3000" -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
    Write-Host "   ✅ Frontend is UP (Port 3000)" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Frontend is DOWN (Port 3000)" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n==================================" -ForegroundColor Cyan
Write-Host "   Test Complete!" -ForegroundColor Cyan
Write-Host "==================================`n" -ForegroundColor Cyan

Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  - Open Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  - Open Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "  - Open Admin Panel: http://localhost:5001`n" -ForegroundColor White



