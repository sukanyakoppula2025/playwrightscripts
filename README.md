# 🚀 Google Search Playwright Tests - Console Application

A C# console application that runs Playwright automation tests against `www.google.co.uk` using Chrome browser in visible mode.

## ✨ Features

- **Direct Console Execution** - Run tests directly from command line
- **Visible Chrome Browser** - Watch automation happen in real-time
- **5 Test Scenarios** - Comprehensive Google search testing
- **Screenshot Capture** - Automatic screenshots on success/failure
- **Video Recording** - Full test execution videos
- **Detailed Logging** - Step-by-step execution logs
- **Cross-Platform** - Works on Windows, macOS, and Linux

## 🧪 Available Tests

1. **Navigation Test** - Navigate to Google UK and verify page loads
2. **Search Test** - Search for "Playwright C#" and verify results
3. **Suggestions Test** - Check search suggestions functionality
4. **Images Tab Test** - Navigate to Google Images tab
5. **Empty Search Test** - Test empty search behavior

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- Chrome browser installed
- Windows 10/11, macOS, or Linux

### Installation

1. **Clone or download** this project
2. **Navigate** to the project directory
3. **Install Playwright browsers**:
   ```bash
   dotnet tool install --global Microsoft.Playwright.CLI
   playwright install
   ```

### Running Tests

#### Option 1: Interactive Mode (Recommended)
```bash
dotnet run
```
Then follow the prompts to select tests.

#### Option 2: Batch File (Windows)
```bash
.\run-tests.bat
```

#### Option 3: Direct Execution
```bash
dotnet run --no-build
```

## 📱 Test Execution

### Single Test
1. Run the application: `dotnet run`
2. Enter test number (1-5)
3. Watch Chrome open and execute the test
4. View results in console
5. Check screenshots and videos in output folders

### All Tests
1. Run the application: `dotnet run`
2. Type `all` when prompted
3. Watch all tests execute sequentially
4. View summary report at the end

## 📁 Output Files

### Screenshots
- **Location**: `screenshots/` folder
- **Format**: PNG files with timestamp
- **Naming**: `TestName_YYYYMMDD_HHMMSS.png`
- **Failure**: `TestName_FAILED_YYYYMMDD_HHMMSS.png`

### Videos
- **Location**: `videos/` folder
- **Format**: WebM files
- **Content**: Full test execution recording

### Console Output
- **Real-time logs** during execution
- **Test results** (PASSED/FAILED)
- **Duration** and performance metrics
- **Error details** if tests fail

## 🔧 Configuration

### Browser Settings
The application is configured to:
- **Launch Chrome** in visible mode (non-headless)
- **Use system Chrome** browser
- **1-second delays** between actions for visibility
- **1920x1080** viewport size
- **Record videos** automatically
- **Take screenshots** on completion

### Customization
Edit `Tests/GoogleSearchTestRunner.cs` to modify:
- Browser launch options
- Viewport size
- Video recording settings
- Screenshot capture behavior

## 🐛 Troubleshooting

### Common Issues

#### "Chrome browser not found"
- Ensure Chrome is installed
- Check if Chrome is in PATH
- Try updating Chrome to latest version

#### "Test execution fails"
- Check internet connection
- Verify Google UK is accessible
- Review console logs for specific errors
- Check screenshots for visual clues

#### "Build errors"
- Ensure .NET 8.0 SDK is installed
- Run `dotnet restore` before building
- Check for missing dependencies

### Debug Mode
- Tests run in visible mode by default
- Watch browser actions in real-time
- Check console logs for detailed information
- Screenshots capture the final state

## 📚 Project Structure

```
GoogleSearchTests/
├── Program.cs                 # Main console application
├── Tests/
│   └── GoogleSearchTestRunner.cs  # Test execution logic
├── screenshots/              # Test screenshots
├── videos/                   # Test recordings
├── run-tests.bat            # Windows batch file
└── README.md                # This file
```

## 🎯 Test Scenarios Explained

### 1. Navigation Test
- **Purpose**: Verify basic connectivity to Google UK
- **Actions**: Navigate to google.co.uk
- **Validation**: Check page title contains "Google"
- **Use Case**: Basic connectivity testing

### 2. Search Test
- **Purpose**: Test search functionality
- **Actions**: Search for "Playwright C#"
- **Validation**: Verify search results appear
- **Use Case**: Core search functionality testing

### 3. Suggestions Test
- **Purpose**: Test search suggestions
- **Actions**: Type partial search query
- **Validation**: Check for suggestion dropdown
- **Use Case**: Autocomplete functionality

### 4. Images Tab Test
- **Purpose**: Test navigation to Images
- **Actions**: Click Images tab
- **Validation**: Verify redirect to images page
- **Use Case**: Tab navigation testing

### 5. Empty Search Test
- **Purpose**: Test empty search handling
- **Actions**: Submit empty search
- **Validation**: Check page behavior
- **Use Case**: Edge case handling

## 🚀 Performance Tips

- **Single Tests**: Faster execution, good for development
- **All Tests**: Comprehensive validation, good for CI/CD
- **Screenshots**: Automatically captured, no performance impact
- **Videos**: Recorded in background, minimal overhead

## 🔒 Security Notes

- **No data collection** from Google
- **Local execution** only
- **Screenshots/videos** stored locally
- **No external API calls** beyond Google search

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch
3. **Add** new test scenarios
4. **Test** thoroughly
5. **Submit** a pull request

## 📄 License

This project is open source and available under the MIT License.

## 🆘 Support

For issues or questions:
1. Check the troubleshooting section
2. Review console logs and screenshots
3. Ensure all prerequisites are met
4. Try running a simple test first

---

**Happy Testing! 🎬✨**
