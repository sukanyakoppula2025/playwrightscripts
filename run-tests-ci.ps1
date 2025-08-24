# PowerShell script for running Playwright tests in CI/CD mode
# This script runs all tests automatically without user interaction

Write-Host "🚀 Playwright Tests - CI/CD Mode" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green
Write-Host ""

# Build the project
Write-Host "🔨 Building project..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful!" -ForegroundColor Green

# Run all tests
Write-Host "🧪 Running all tests..." -ForegroundColor Yellow
Write-Host "This will run in headless mode for CI/CD" -ForegroundColor Cyan

# Set environment variable for headless mode
$env:PLAYWRIGHT_HEADLESS = "true"

# Run the tests
dotnet run --configuration Release --no-build

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ All tests completed successfully!" -ForegroundColor Green
    
    # Check if test-report.html exists
    if (Test-Path "test-report.html") {
        Write-Host "✅ HTML report generated: test-report.html" -ForegroundColor Green
        
        # List screenshots
        if (Test-Path "screenshots") {
            $screenshots = Get-ChildItem "screenshots" -Filter "*.png"
            Write-Host "📸 Screenshots captured: $($screenshots.Count)" -ForegroundColor Cyan
        }
        
        Write-Host "🎉 CI/CD test execution completed successfully!" -ForegroundColor Green
        exit 0
    } else {
        Write-Host "⚠️ HTML report not found" -ForegroundColor Yellow
        exit 0
    }
} else {
    Write-Host "❌ Tests failed!" -ForegroundColor Red
    exit 1
}
