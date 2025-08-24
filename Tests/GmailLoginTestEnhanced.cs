using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    public class GmailLoginTestEnhanced : IDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;
        private readonly GmailTestConfig _config;

        public GmailLoginTestEnhanced(GmailTestConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<TestResult> RunGmailLoginTestAsync()
        {
            var startTime = DateTime.UtcNow;
            var result = new TestResult
            {
                TestName = "Gmail Login Test (Enhanced)",
                ExecutionTime = startTime,
                Logs = new List<string>()
            };

            try
            {
                // Validate configuration
                if (!_config.IsValid())
                {
                    var errors = _config.GetValidationErrors();
                    result.Passed = false;
                    result.Message = $"Configuration validation failed: {string.Join(", ", errors)}";
                    return result;
                }

                result.Logs.Add($"Starting Gmail Login Test for: {_config.Email}");
                result.Logs.Add($"Configuration: Headless={_config.Headless}, SlowMo={_config.SlowMo}ms, Timeout={_config.Timeout}ms");
                
                // Initialize Playwright
                _playwright = await Playwright.CreateAsync();
                result.Logs.Add("Playwright initialized");

                // Launch browser with configuration
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = _config.Headless,
                    SlowMo = _config.SlowMo,
                    Channel = _config.BrowserChannel
                });
                result.Logs.Add($"{_config.BrowserChannel} browser launched (Headless: {_config.Headless})");

                // Create browser context
                var contextOptions = new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = _config.ViewportWidth, Height = _config.ViewportHeight }
                };

                if (_config.RecordVideo)
                {
                    contextOptions.RecordVideoDir = _config.VideoDirectory;
                    contextOptions.RecordVideoSize = new RecordVideoSize { Width = _config.ViewportWidth, Height = _config.ViewportHeight };
                }

                _context = await _browser.NewContextAsync(contextOptions);
                result.Logs.Add($"Browser context created with viewport {_config.ViewportWidth}x{_config.ViewportHeight}");

                // Create page
                _page = await _context.NewPageAsync();
                result.Logs.Add("New page created");

                // Navigate to Gmail
                await _page.GotoAsync(_config.GmailUrl);
                result.Logs.Add($"Navigated to {_config.GmailUrl}");

                // Wait for page load
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                result.Logs.Add("Page loaded successfully");

                // Handle sign-in flow
                await HandleSignInFlowAsync(result);

                // Wait for successful login or handle errors
                await WaitForLoginCompletionAsync(result);

                // Clean up if not keeping browser open
                if (!_config.KeepBrowserOpen)
                {
                    await CleanupAsync();
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, result);
            }

            result.Duration = DateTime.UtcNow - result.ExecutionTime;
            result.Logs.Add($"Test completed in {result.Duration.TotalSeconds:F2} seconds");

            return result;
        }

        private async Task HandleSignInFlowAsync(TestResult result)
        {
            // Check if we need to sign in
            var signInButton = _page.Locator("button:has-text('Sign in')").First;
            if (await signInButton.CountAsync() > 0)
            {
                result.Logs.Add("Sign in button found, proceeding with login");
                await signInButton.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            else
            {
                result.Logs.Add("Already on login page or signed in");
            }

            // Enter email
            var emailInput = _page.Locator("input[type='email']");
            await emailInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = _config.Timeout });
            await emailInput.FillAsync(_config.Email);
            result.Logs.Add("Email entered successfully");

            // Click Next after email
            var nextButton = _page.Locator("text=Next");
            await nextButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            result.Logs.Add("Clicked Next after email");

            // Wait and enter password
            // Gmail initially shows a hidden password field, wait for the visible one
            var passwordInput = _page.Locator("input[type='password']:not([aria-hidden='true'])");
            await passwordInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = _config.Timeout });
            await passwordInput.FillAsync(_config.Password);
            result.Logs.Add("Password entered successfully");

            // Click Next after password
            var passwordNextButton = _page.Locator("text=Next");
            await passwordNextButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            result.Logs.Add("Clicked Next after password");

            // Wait for login processing
            await Task.Delay(_config.LoginWaitTime);
            result.Logs.Add($"Waited {_config.LoginWaitTime}ms for login processing");
        }

        private async Task WaitForLoginCompletionAsync(TestResult result)
        {
            try
            {
                // Wait for Gmail inbox to load
                await _page!.WaitForSelectorAsync("div[role='main']", new PageWaitForSelectorOptions { Timeout = _config.Timeout });
                result.Logs.Add("Successfully navigated to Gmail inbox");
                
                if (_config.TakeScreenshots)
                {
                    var screenshotPath = $"{_config.ScreenshotDirectory}Gmail_Login_Success_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Success screenshot saved: {screenshotPath}");
                }

                result.Passed = true;
                result.Message = "Gmail login completed successfully";
            }
            catch (TimeoutException)
            {
                await HandleLoginTimeoutAsync(result);
            }
        }

        private async Task HandleLoginTimeoutAsync(TestResult result)
        {
            // Check for 2FA
            var twoFactorInput = _page!.Locator("input[type='tel']");
            if (await twoFactorInput.CountAsync() > 0)
            {
                result.Logs.Add("2FA input field detected");
                if (_config.Handle2FA)
                {
                    result.Logs.Add("2FA handling enabled - waiting for manual input");
                    // Wait for user to manually enter 2FA code
                    await _page.WaitForSelectorAsync("div[role='main']", new PageWaitForSelectorOptions { Timeout = 60000 });
                    result.Passed = true;
                    result.Message = "Gmail login completed with 2FA";
                }
                else
                {
                    result.Passed = false;
                    result.Message = "2FA authentication required - test cannot complete automatically";
                    
                    if (_config.TakeScreenshots)
                    {
                        var screenshotPath = $"{_config.ScreenshotDirectory}Gmail_Login_2FA_Required_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        result.Logs.Add($"2FA screenshot saved: {screenshotPath}");
                    }
                }
            }
            else
            {
                // Check for error messages
                await CheckForErrorMessagesAsync(result);
            }
        }

        private async Task CheckForErrorMessagesAsync(TestResult result)
        {
            var errorSelectors = new[]
            {
                "div[role='alert']",
                "div[data-error]",
                "span[data-error]",
                ".error-message",
                ".alert-error"
            };

            foreach (var selector in errorSelectors)
            {
                var errorElement = _page!.Locator(selector);
                if (await errorElement.CountAsync() > 0)
                {
                    var errorText = await errorElement.TextContentAsync();
                    result.Logs.Add($"Error message found: {errorText}");
                    result.Passed = false;
                    result.Message = $"Login failed: {errorText}";
                    
                    if (_config.TakeScreenshots)
                    {
                        var screenshotPath = $"{_config.ScreenshotDirectory}Gmail_Login_Error_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        result.Logs.Add($"Error screenshot saved: {screenshotPath}");
                    }
                    return;
                }
            }

            // No specific error found
            result.Logs.Add("Login timeout - inbox not loaded within expected time");
            result.Passed = false;
            result.Message = "Login timeout - inbox not loaded within expected time";
            
            if (_config.TakeScreenshots)
            {
                var screenshotPath = $"{_config.ScreenshotDirectory}Gmail_Login_Timeout_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                result.Logs.Add($"Timeout screenshot saved: {screenshotPath}");
            }
        }

        private async Task HandleExceptionAsync(Exception ex, TestResult result)
        {
            result.Passed = false;
            result.Message = $"Test failed with exception: {ex.Message}";
            result.Logs.Add($"Exception occurred: {ex}");
            
            if (_page != null && _config.TakeScreenshots)
            {
                try
                {
                    var screenshotPath = $"{_config.ScreenshotDirectory}Gmail_Login_Exception_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Exception screenshot saved: {screenshotPath}");
                }
                catch (Exception screenshotEx)
                {
                    result.Logs.Add($"Failed to take exception screenshot: {screenshotEx.Message}");
                }
            }
        }

        private async Task CleanupAsync()
        {
            try
            {
                if (_page != null)
                {
                    await _page.CloseAsync();
                    _page = null;
                }
                
                if (_context != null)
                {
                    await _context.CloseAsync();
                    _context = null;
                }
                
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                    _browser = null;
                }
                
                if (_playwright != null)
                {
                    _playwright.Dispose();
                    _playwright = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }
        }

        public async Task DisposeAsync()
        {
            await CleanupAsync();
        }

        public void Dispose()
        {
            DisposeAsync().Wait();
        }
    }
}
