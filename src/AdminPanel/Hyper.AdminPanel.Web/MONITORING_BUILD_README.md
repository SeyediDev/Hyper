# Monitoring Micro-frontend Build Guide

## Overview

The monitoring micro-frontend is a Next.js application that provides an advanced monitoring dashboard. This guide explains how to build and deploy it.

## Prerequisites

- Node.js (v18 or higher)
- npm (v9 or higher)

## Project Structure

```
Hyper.AdminPanel.Web/
├── wwwroot/
│   └── monitoring/          # Built output (deployed files)
├── monitoring-source/       # Next.js source code (if exists)
└── build-monitoring.ps1     # Build script
```

## Building the Micro-frontend

### Option 1: Using the Build Script (Recommended)

Run the PowerShell build script:

```powershell
cd "D:\Projects\Hyper\Backend\src\AdminPanel\Hyper.AdminPanel.Web"
.\build-monitoring.ps1
```

The script will:
1. Check if source code exists
2. Install dependencies (if needed)
3. Build the Next.js application
4. Copy output to `wwwroot/monitoring`

### Option 2: Manual Build

If you have the source code in a different location:

1. Navigate to the monitoring source directory
2. Install dependencies:
   ```bash
   npm install
   ```
3. Build the application:
   ```bash
   npm run build
   ```
4. Copy the output to `wwwroot/monitoring`

## Next.js Configuration

For static export (required for micro-frontend), ensure `next.config.js` includes:

```javascript
/** @type {import('next').NextConfig} */
const nextConfig = {
  output: 'export',
  basePath: '/monitoring',
  assetPrefix: '/monitoring',
  trailingSlash: true,
}

module.exports = nextConfig
```

## Creating a New Monitoring Micro-frontend

If you need to create a new Next.js project for monitoring:

```bash
# Create Next.js app
npx create-next-app@latest monitoring-source --typescript --tailwind --app

# Navigate to project
cd monitoring-source

# Configure for static export
# Edit next.config.js as shown above

# Build
npm run build
```

## Accessing the Monitoring Dashboard

After building, the monitoring dashboard is available at:
- **Simple version**: `/Monitoring/Index`
- **Advanced version**: `/Monitoring/Advanced` (loads `/monitoring/index.html`)

## Troubleshooting

### Build Script Fails

1. Ensure Node.js and npm are installed and in PATH
2. Check that source code exists in `monitoring-source/`
3. Verify `package.json` exists in source directory

### Files Not Copying

1. Check that build output directory exists (`out` or `dist`)
2. Verify write permissions for `wwwroot/monitoring`
3. Ensure output directory is not locked by another process

### Micro-frontend Not Loading

1. Verify files are in `wwwroot/monitoring/`
2. Check browser console for errors
3. Ensure basePath is correctly configured in Next.js config

## Notes

- The build script automatically handles dependency installation
- Build output is copied to `wwwroot/monitoring` for deployment
- The micro-frontend is served as static files (no server-side rendering)


