@echo off
echo.
echo ========================================
echo    Starting Club Backend Services
echo ========================================
echo.

REM Customer Portal API
echo Starting Customer Portal API on http://localhost:5000
start "Customer Portal API" cmd /k "cd /d %~dp0src\CustomerPortal\Club.CustomerPortal.Api && dotnet run --urls=http://localhost:5000"
timeout /t 2 /nobreak >nul

REM Admin Panel
echo Starting Admin Panel on http://localhost:5001
start "Admin Panel Web" cmd /k "cd /d %~dp0src\AdminPanel\Club.AdminPanel.Web && dotnet run --urls=http://localhost:5001"
timeout /t 2 /nobreak >nul

REM Frontend
echo.
echo Starting Frontend on http://localhost:3000
start "Customer Portal Frontend" cmd /k "cd /d D:\Projects\Club\Frontend\CustomerPortal\Club.CustomerPortal.Web && npm run dev"

echo.
echo ========================================
echo    Services Started!
echo ========================================
echo.
echo URLs:
echo   - Customer Portal API: http://localhost:5000
echo   - Admin Panel: http://localhost:5001  
echo   - Frontend: http://localhost:3000
echo.
echo Close CMD windows to stop services
echo.
pause


