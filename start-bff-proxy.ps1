# PowerShell script to start the BFF Reverse Proxy
# This script will start the BFF service and test the reverse proxy functionality

Write-Host "Starting BFF Reverse Proxy..." -ForegroundColor Green

# Check if .NET is available
try {
    $dotnetVersion = dotnet --version
    Write-Host "Using .NET version: $dotnetVersion" -ForegroundColor Yellow
} catch {
    Write-Host "Error: .NET is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Navigate to the BFF project directory
Set-Location "src\Bff"

Write-Host "Starting BFF service on port 7002..." -ForegroundColor Yellow
Write-Host "Make sure the main API is running on port 7058" -ForegroundColor Yellow
Write-Host "" -ForegroundColor White

# Start the BFF service
Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory (Get-Location)

# Wait a moment for the service to start
Start-Sleep -Seconds 5

Write-Host "BFF Reverse Proxy should now be running!" -ForegroundColor Green
Write-Host "" -ForegroundColor White
Write-Host "Test endpoints:" -ForegroundColor Cyan
Write-Host "  BFF Status: https://localhost:7002/api/proxy-test/status" -ForegroundColor White
Write-Host "  Proxy Info: https://localhost:7002/api/proxy-test/proxy-info" -ForegroundColor White
Write-Host "  BFF Swagger: https://localhost:7002/swagger" -ForegroundColor White
Write-Host "" -ForegroundColor White
Write-Host "Proxied endpoints (will forward to main API on port 7058):" -ForegroundColor Cyan
Write-Host "  Agencies: https://localhost:7002/api/agencies" -ForegroundColor White
Write-Host "  Interpreters: https://localhost:7002/api/interpreters" -ForegroundColor White
Write-Host "  Clients: https://localhost:7002/api/clients" -ForegroundColor White
Write-Host "  Appointments: https://localhost:7002/api/appointments" -ForegroundColor White
Write-Host "" -ForegroundColor White
Write-Host "Press any key to open the BFF status page in your browser..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Open the BFF status page in the default browser
Start-Process "https://localhost:7002/api/proxy-test/status"

Write-Host "" -ForegroundColor White
Write-Host "BFF Reverse Proxy is now running and ready to proxy requests!" -ForegroundColor Green
Write-Host "Keep this terminal open to monitor the BFF service." -ForegroundColor Yellow
