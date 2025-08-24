@echo off
echo ========================================
echo    Gmail Automation Test Runner
echo ========================================
echo.

echo Building Gmail tests project...
dotnet build GmailTests.csproj

if %ERRORLEVEL% NEQ 0 (
    echo ❌ Build failed! Please check for errors.
    pause
    exit /b 1
)

echo ✅ Build successful!
echo.

echo Running Gmail Login Test...
echo Note: You will be prompted for your Gmail credentials.
echo.

dotnet run --project GmailTests.csproj

echo.
echo Test completed. Check the screenshots and videos folders for results.
pause
