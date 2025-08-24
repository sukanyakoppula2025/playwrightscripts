# 📧 Gmail Automation Test Scripts with Playwright C#

This project contains comprehensive automation test scripts for Gmail login functionality using Playwright with C#. The scripts provide robust testing capabilities with proper error handling, screenshot capture, and video recording.

## 🚀 Features

- **Automated Gmail Login**: Complete automation of the Gmail sign-in process
- **Multiple Test Implementations**: Both basic and enhanced versions available
- **Configuration Management**: Flexible configuration through code or environment variables
- **Error Handling**: Comprehensive error detection and handling
- **Screenshot Capture**: Automatic screenshots on success, failure, and exceptions
- **Video Recording**: Full test execution recording for debugging
- **2FA Support**: Basic 2FA detection and handling options
- **Cross-Browser Support**: Configurable browser selection
- **CI/CD Ready**: Environment variable support for automated testing

## 📁 Project Structure

```
├── Tests/
│   ├── GmailLoginTest.cs              # Basic Gmail login test
│   └── GmailLoginTestEnhanced.cs      # Enhanced version with configuration
├── GmailTestConfig.cs                  # Configuration class
├── GmailLoginTestRunner.cs             # Standalone test runner
└── GMAIL-AUTOMATION-README.md          # This file
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
   dotnet build
   ```

2. **Run the basic test runner**:
   ```bash
   dotnet run --project GmailLoginTestRunner.cs
   ```

3. **Enter your Gmail credentials** when prompted

### Option 2: Enhanced Test with Configuration

1. **Create a configuration**:
   ```csharp
   var config = new GmailTestConfig
   {
       Email = "your.email@gmail.com",
       Password = "your-password",
       Headless = false,
       SlowMo = 1000,
       TakeScreenshots = true,
       RecordVideo = true
   };
   ```

2. **Run the enhanced test**:
   ```csharp
   using var test = new GmailLoginTestEnhanced(config);
   var result = await test.RunGmailLoginTestAsync();
   ```

## ⚙️ Configuration Options

### Basic Settings

| Property | Default | Description |
|----------|---------|-------------|
| `Email` | "" | Gmail email address |
| `Password` | "" | Gmail password |
| `Headless` | false | Run browser in headless mode |
| `SlowMo` | 1000 | Delay between actions (ms) |
| `Timeout` | 30000 | Element wait timeout (ms) |

### Browser Settings

| Property | Default | Description |
|----------|---------|-------------|
| `BrowserChannel` | "chrome" | Browser to use (chrome, msedge) |
| `ViewportWidth` | 1920 | Browser viewport width |
| `ViewportHeight` | 1080 | Browser viewport height |
| `RecordVideo` | true | Enable video recording |
| `VideoDirectory` | "videos/" | Directory for video files |

### Test Behavior

| Property | Default | Description |
|----------|---------|-------------|
| `KeepBrowserOpen` | false | Keep browser open after test |
| `TakeScreenshots` | true | Enable screenshot capture |
| `Handle2FA` | false | Enable 2FA handling |
| `GmailUrl` | "https://mail.google.com" | Gmail URL to test |

## 🔧 Environment Variables

For CI/CD environments, you can configure the test using environment variables:

```bash
# Gmail credentials
GMAIL_EMAIL=your.email@gmail.com
GMAIL_PASSWORD=your-password

# Test configuration
GMAIL_HEADLESS=true
GMAIL_SLOWMO=500
GMAIL_TIMEOUT=30000
GMAIL_BROWSER_CHANNEL=chrome

# Browser settings
GMAIL_VIEWPORT_WIDTH=1920
GMAIL_VIEWPORT_HEIGHT=1080
GMAIL_RECORD_VIDEO=true
GMAIL_VIDEO_DIR=videos/
GMAIL_SCREENSHOT_DIR=screenshots/

# Test behavior
GMAIL_KEEP_BROWSER_OPEN=false
GMAIL_TAKE_SCREENSHOTS=true
GMAIL_HANDLE_2FA=false
```

## 📸 Screenshot and Video Output

The tests automatically capture:

- **Success Screenshots**: `Gmail_Login_Success_YYYYMMDD_HHMMSS.png`
- **Failure Screenshots**: `Gmail_Login_Failed_YYYYMMDD_HHMMSS.png`
- **2FA Screenshots**: `Gmail_Login_2FA_Required_YYYYMMDD_HHMMSS.png`
- **Error Screenshots**: `Gmail_Login_Error_YYYYMMDD_HHMMSS.png`
- **Exception Screenshots**: `Gmail_Login_Exception_YYYYMMDD_HHMMSS.png`
- **Videos**: Full test execution recordings in the `videos/` directory

## 🔒 Security Considerations

⚠️ **Important Security Notes**:

1. **Never commit credentials** to source control
2. **Use environment variables** for sensitive data in production
3. **Consider using App Passwords** instead of regular passwords
4. **Enable 2FA** on your Gmail account for better security
5. **Use test accounts** when possible instead of production accounts

### Using App Passwords

If you have 2FA enabled, create an App Password:

1. Go to Google Account settings
2. Navigate to Security → 2-Step Verification
3. Create an App Password for "Mail"
4. Use the generated password in your tests

## 🧪 Test Scenarios

The automation covers:

- ✅ **Successful Login**: Standard email/password authentication
- ⚠️ **2FA Detection**: Automatic detection of 2FA requirements
- ❌ **Error Handling**: Invalid credentials, network issues, etc.
- 📱 **Responsive Design**: Different viewport sizes
- 🌐 **Cross-Browser**: Chrome, Edge, and other supported browsers

## 🐛 Troubleshooting

### Common Issues

1. **Element Not Found**:
   - Gmail's UI may change - update selectors if needed
   - Increase timeout values
   - Check if Gmail is accessible from your location

2. **Login Fails**:
   - Verify credentials are correct
   - Check if 2FA is required
   - Ensure account isn't locked

3. **Browser Issues**:
   - Update Playwright to latest version
   - Reinstall browser binaries: `pwsh bin/Debug/net8.0/playwright.ps1 install`
   - Check browser compatibility

4. **Performance Issues**:
   - Reduce `SlowMo` value
   - Enable headless mode for faster execution
   - Optimize viewport size

### Debug Mode

For debugging, set these configuration options:

```csharp
var config = new GmailTestConfig
{
    Headless = false,        // See browser actions
    SlowMo = 2000,          // Slower execution
    KeepBrowserOpen = true,  // Keep browser open
    TakeScreenshots = true,  // Capture screenshots
    RecordVideo = true       // Record video
};
```

## 🚀 CI/CD Integration

### GitHub Actions Example

```yaml
- name: Run Gmail Tests
  env:
    GMAIL_EMAIL: ${{ secrets.GMAIL_EMAIL }}
    GMAIL_PASSWORD: ${{ secrets.GMAIL_PASSWORD }}
    GMAIL_HEADLESS: true
    GMAIL_SLOWMO: 500
  run: |
    dotnet build
    dotnet test
```

### Azure DevOps Example

```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
  env:
    GMAIL_EMAIL: $(GmailEmail)
    GMAIL_PASSWORD: $(GmailPassword)
    GMAIL_HEADLESS: true
```

## 📚 API Reference

### GmailLoginTest Class

```csharp
public class GmailLoginTest
{
    public async Task<TestResult> RunGmailLoginTestAsync(
        string email, 
        string password, 
        bool keepBrowserOpen = false
    );
    
    public async Task DisposeAsync();
}
```

### GmailLoginTestEnhanced Class

```csharp
public class GmailLoginTestEnhanced
{
    public GmailLoginTestEnhanced(GmailTestConfig config);
    
    public async Task<TestResult> RunGmailLoginTestAsync();
    
    public async Task DisposeAsync();
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
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public List<string> Logs { get; set; }
}
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is provided as-is for educational and testing purposes. Please ensure you comply with Gmail's Terms of Service and your organization's security policies.

## 🆘 Support

For issues and questions:

1. Check the troubleshooting section
2. Review Gmail's current UI structure
3. Verify Playwright compatibility
4. Check browser console for errors

---

**Happy Testing! 🎉**
