@echo off
echo 🚀 Playwright Tests - CI/CD Mode
echo =================================
echo.

echo 🔨 Building project...
dotnet build --configuration Release --no-restore

if %ERRORLEVEL% neq 0 (
    echo ❌ Build failed!
    exit /b 1
)

echo ✅ Build successful!
echo.

echo 🧪 Running all tests...
echo This will run in headless mode for CI/CD
echo.

set PLAYWRIGHT_HEADLESS=true
dotnet run --configuration Release --no-build

if %ERRORLEVEL% equ 0 (
    echo ✅ All tests completed successfully!
    
    if exist "test-report.html" (
        echo ✅ HTML report generated: test-report.html
        
        if exist "screenshots" (
            dir /b screenshots\*.png | find /c ".png" > temp_count.txt
            set /p screenshot_count=<temp_count.txt
            del temp_count.txt
            echo 📸 Screenshots captured: %screenshot_count%
        )
        
        echo 🎉 CI/CD test execution completed successfully!
        exit /b 0
    ) else (
        echo ⚠️ HTML report not found
        exit /b 0
    )
) else (
    echo ❌ Tests failed!
    exit /b 1
)
