# 🛏️ Bed Bath & Beyond Automation Test Scripts with Playwright C#

This project contains comprehensive automation test scripts for Bed Bath & Beyond account login functionality using Playwright with C#. The scripts provide robust testing capabilities with proper error handling, screenshot capture, and video recording.

## 🚀 Features

- **Automated Bed Bath & Beyond Login**: Complete automation of the account sign-in process
- **Smart Cookie Consent Handling**: Automatic detection and handling of cookie consent dialogs
- **Flexible Element Detection**: Multiple selector strategies for robust element identification
- **Error Handling**: Comprehensive error detection and handling
- **Screenshot Capture**: Automatic screenshots on success, failure, and exceptions
- **Video Recording**: Full test execution recording for debugging
- **Cross-Browser Support**: Configurable browser selection
- **CI/CD Ready**: Environment variable support for automated testing

## 📁 Project Structure

```
├── Tests/
│   └── BedBathBeyondLoginTest.cs      # Bed Bath & Beyond login test
├── BedBathBeyondTestRunner.cs          # Standalone test runner
├── BedBathBeyondTests.csproj           # Project file
├── run-bedbath-tests.bat               # Windows batch file
└── BEDBATH-AUTOMATION-README.md        # This file
```

## 🛠️ Prerequisites

1. **.NET 8.0** or later
2. **Playwright** installed and browsers downloaded
3. **Chrome** browser (or other supported browsers)

### Install Playwright Browsers

```bash
# Install Playwright browsers
pwsh bin/Debug/net8.0/playwright.ps1 install
```

## 🚀 Quick Start

### Option 1: Simple Test Runner

1. **Build the project**:
   ```bash
   dotnet build BedBathBeyondTests.csproj
   ```

2. **Run the test runner**:
   ```bash
   dotnet run --project BedBathBeyondTests.csproj
   ```

3. **Enter your Bed Bath & Beyond account credentials** when prompted

### Option 2: Batch File (Windows)

1. **Double-click** `run-bedbath-tests.bat`
2. **Wait for build** to complete
3. **Enter credentials** when prompted

## 🎯 Test Scenarios

The automation covers:

- ✅ **Cookie Consent Handling**: Automatic detection and acceptance of cookie dialogs
- ✅ **Account Login**: Email and password authentication
- ✅ **Success Detection**: Automatic detection of successful login
- ✅ **Error Handling**: Detection of login failures and error messages
- ✅ **Screenshot Capture**: Visual evidence of test execution
- ✅ **Video Recording**: Full test execution recording

## 🔧 Element Detection Strategy

The test uses multiple selector strategies to ensure robust element detection:

### Sign In Button
```csharp
"button:has-text('Sign In'), a:has-text('Sign In'), [data-testid='sign-in-button']"
```

### Email Input
```csharp
"input[type='email'], input[name='email'], input[id*='email'], [data-testid='email-input']"
```

### Password Input
```csharp
"input[type='password'], input[name='password'], input[id*='password'], [data-testid='password-input']"
```

### Submit Button
```csharp
"button:has-text('Sign In'), button:has-text('Login'), button[type='submit'], [data-testid='sign-in-submit']"
```

## 📸 Screenshot and Video Output

The tests automatically capture:

- **Success Screenshots**: `BedBathBeyond_Login_Success_YYYYMMDD_HHMMSS.png`
- **Failure Screenshots**: `BedBathBeyond_Login_Failed_YYYYMMDD_HHMMSS.png`
- **Error Screenshots**: `BedBathBeyond_Login_Error_YYYYMMDD_HHMMSS.png`
- **Exception Screenshots**: `BedBathBeyond_Login_Exception_YYYYMMDD_HHMMSS.png`
- **Timeout Screenshots**: `BedBathBeyond_Login_Timeout_YYYYMMDD_HHMMSS.png`
- **Videos**: Full test execution recordings in the `videos/` directory

## 🔒 Security Considerations

⚠️ **Important Security Notes**:

1. **Never commit credentials** to source control
2. **Use environment variables** for sensitive data in production
3. **Use test accounts** when possible instead of production accounts
4. **Consider using App Passwords** if available
5. **Enable 2FA** on your account for better security

## 🧪 Test Flow

1. **Navigate** to `https://www.bedbathandbeyond.com/myaccount`
2. **Handle Cookie Consent** if present
3. **Locate Sign In Button** and click if needed
4. **Enter Email** address
5. **Enter Password**
6. **Submit Login** form
7. **Verify Success** by checking for account dashboard elements
8. **Capture Results** with screenshots and logging

## 🐛 Troubleshooting

### Common Issues

1. **Element Not Found**:
   - Bed Bath & Beyond's UI may change - update selectors if needed
   - Increase timeout values
   - Check if the site is accessible from your location

2. **Login Fails**:
   - Verify credentials are correct
   - Check if account is locked or requires verification
   - Ensure account exists and is active

3. **Cookie Consent Issues**:
   - The test automatically handles common cookie consent patterns
   - If new patterns appear, update the `HandleCookieConsentAsync` method

4. **Browser Issues**:
   - Update Playwright to latest version
   - Reinstall browser binaries: `pwsh bin/Debug/net8.0/playwright.ps1 install`
   - Check browser compatibility

### Debug Mode

For debugging, the test runs with:
- **Visible browser** (`Headless = false`)
- **Slow execution** (`SlowMo = 1000ms`)
- **Browser stays open** (`keepBrowserOpen = true`)
- **Full logging** of all actions

## 🚀 CI/CD Integration

### GitHub Actions Example

```yaml
- name: Run Bed Bath & Beyond Tests
  env:
    BEDBATH_EMAIL: ${{ secrets.BEDBATH_EMAIL }}
    BEDBATH_PASSWORD: ${{ secrets.BEDBATH_PASSWORD }}
  run: |
    dotnet build BedBathBeyondTests.csproj
    dotnet test BedBathBeyondTests.csproj
```

### Azure DevOps Example

```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    projects: 'BedBathBeyondTests.csproj'
  env:
    BEDBATH_EMAIL: $(BedBathEmail)
    BEDBATH_PASSWORD: $(BedBathPassword)
```

## 📚 API Reference

### BedBathBeyondLoginTest Class

```csharp
public class BedBathBeyondLoginTest : IDisposable
{
    public async Task<TestResult> RunBedBathBeyondLoginTestAsync(
        string email, 
        string password, 
        bool keepBrowserOpen = false
    );
    
    public async Task DisposeAsync();
    public void Dispose();
}
```

### TestResult Class

```csharp
public class TestResult
{
    public string TestName { get; set; }
    public bool Passed { get; set; }
    public string Message { get; set; }
    public DateTime ExecutionTime { get; set; }
    public TimeSpan Duration { get; set; }
    public List<string> Logs { get; set; }
}
```

## 🔧 Customization

### Adding New Selectors

To add new element selectors, modify the relevant methods:

```csharp
// Add new email input selectors
var emailInput = _page.Locator("input[type='email'], input[name='email'], input[id*='email'], [data-testid='email-input'], YOUR_NEW_SELECTOR");
```

### Handling New Cookie Consent Patterns

```csharp
private async Task HandleCookieConsentAsync(TestResult result)
{
    // Add new cookie consent selectors here
    var cookieSelectors = new[]
    {
        "button:has-text('Accept')",
        "button:has-text('Accept All')",
        "YOUR_NEW_COOKIE_SELECTOR"
    };
    // ... rest of method
}
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is provided as-is for educational and testing purposes. Please ensure you comply with Bed Bath & Beyond's Terms of Service and your organization's security policies.

## 🆘 Support

For issues and questions:

1. Check the troubleshooting section
2. Review Bed Bath & Beyond's current UI structure
3. Verify Playwright compatibility
4. Check browser console for errors
5. Examine the captured screenshots for visual clues

---

**Happy Testing! 🛏️✨**
