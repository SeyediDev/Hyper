# Build script for Monitoring Micro-frontend
# This script builds the Next.js monitoring micro-frontend

$ErrorActionPreference = "Stop"

# Paths
$MonitoringSourcePath = ".\monitoring-source"
$MonitoringOutputPath = ".\wwwroot\monitoring"
$ProjectRoot = Split-Path -Parent $PSScriptRoot

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building Monitoring Micro-frontend" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if source code exists
if (-not (Test-Path $MonitoringSourcePath)) {
    Write-Host "⚠️  Monitoring source code not found at: $MonitoringSourcePath" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To build the monitoring micro-frontend:" -ForegroundColor Yellow
    Write-Host "1. Create a Next.js project in '$MonitoringSourcePath'" -ForegroundColor Yellow
    Write-Host "2. Configure it to output to '$MonitoringOutputPath'" -ForegroundColor Yellow
    Write-Host "3. Run this script again" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Alternatively, if you have the source code elsewhere, update this script." -ForegroundColor Yellow
    exit 0
}

# Check if Node.js is installed
try {
    $nodeVersion = node --version
    Write-Host "✓ Node.js found: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Node.js is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install Node.js from https://nodejs.org/" -ForegroundColor Red
    exit 1
}

# Check if npm is installed
try {
    $npmVersion = npm --version
    Write-Host "✓ npm found: $npmVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ npm is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Building monitoring micro-frontend..." -ForegroundColor Cyan
Write-Host ""

# Navigate to source directory
Push-Location $MonitoringSourcePath

try {
    # Install dependencies if node_modules doesn't exist
    if (-not (Test-Path ".\node_modules")) {
        Write-Host "Installing dependencies..." -ForegroundColor Yellow
        npm install
        if ($LASTEXITCODE -ne 0) {
            throw "npm install failed"
        }
    }
    
    # Build the project
    Write-Host ""
    Write-Host "Building Next.js application..." -ForegroundColor Yellow
    
    # Check if next.config.js exists and has output: 'export'
    $nextConfigExists = Test-Path ".\next.config.js"
    if ($nextConfigExists) {
        $nextConfig = Get-Content ".\next.config.js" -Raw
        if ($nextConfig -notmatch "output.*export") {
            Write-Host "⚠️  Warning: next.config.js should have 'output: export' for static export" -ForegroundColor Yellow
        }
    }
    
    # Build command
    npm run build
    
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }
    
    Write-Host ""
    Write-Host "✓ Build completed successfully!" -ForegroundColor Green
    
    # Copy build output to wwwroot/monitoring
    if (Test-Path ".\out") {
        Write-Host ""
        Write-Host "Copying build output to wwwroot/monitoring..." -ForegroundColor Yellow
        
        # Create output directory if it doesn't exist
        if (-not (Test-Path $MonitoringOutputPath)) {
            New-Item -ItemType Directory -Path $MonitoringOutputPath -Force | Out-Null
        }
        
        # Copy files
        Copy-Item -Path ".\out\*" -Destination $MonitoringOutputPath -Recurse -Force
        
        Write-Host "✓ Files copied successfully!" -ForegroundColor Green
    } elseif (Test-Path ".\dist") {
        Write-Host ""
        Write-Host "Copying build output to wwwroot/monitoring..." -ForegroundColor Yellow
        
        if (-not (Test-Path $MonitoringOutputPath)) {
            New-Item -ItemType Directory -Path $MonitoringOutputPath -Force | Out-Null
        }
        
        Copy-Item -Path ".\dist\*" -Destination $MonitoringOutputPath -Recurse -Force
        
        Write-Host "✓ Files copied successfully!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Warning: Build output directory not found (expected 'out' or 'dist')" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host ""
    Write-Host "✗ Build failed: $_" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Monitoring micro-frontend is available at:" -ForegroundColor Cyan
Write-Host "  /monitoring/index.html" -ForegroundColor White
Write-Host ""


