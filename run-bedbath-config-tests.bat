@echo off
echo ========================================
echo    Bed Bath & Beyond Config Tests
echo ========================================
echo.

echo Building Bed Bath & Beyond tests project...
dotnet build BedBathBeyondTests.csproj

if %ERRORLEVEL% NEQ 0 (
    echo ❌ Build failed! Please check for errors.
    pause
    exit /b 1
)

echo ✅ Build successful!
echo.

echo Running Bed Bath & Beyond Login Test with Configuration...
echo Note: Using pre-configured credentials from BedBathBeyondTestConfig.cs
echo.

dotnet run --project BedBathBeyondTests.csproj -- BedBathBeyondConfigTestRunner

echo.
echo Test completed. Check the screenshots and videos folders for results.
pause
