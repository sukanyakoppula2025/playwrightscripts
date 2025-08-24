using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    public class GmailLoginTest : IDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        public async Task<TestResult> RunGmailLoginTestAsync(string email, string password, bool keepBrowserOpen = false)
        {
            var startTime = DateTime.UtcNow;
            var result = new TestResult
            {
                TestName = "Gmail Login Test",
                ExecutionTime = startTime,
                Logs = new List<string>()
            };

            try
            {
                result.Logs.Add($"Starting Gmail Login Test for: {email}");
                
                // Initialize Playwright if not already done
                if (_playwright == null)
                {
                    _playwright = await Playwright.CreateAsync();
                    result.Logs.Add("Playwright initialized");
                }

                // Launch browser if not already done
                if (_browser == null)
                {
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = false, // Make browser visible for debugging
                        SlowMo = 1000, // 1 second delay between actions
                        Channel = "chrome" // Use Chrome browser
                    });
                    result.Logs.Add("Chrome browser launched");
                }

                // Create new context for each test (to avoid state conflicts)
                if (_context != null)
                {
                    await _context.CloseAsync();
                }
                
                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                    RecordVideoDir = "videos/",
                    RecordVideoSize = new RecordVideoSize { Width = 1920, Height = 1080 }
                });
                result.Logs.Add("Browser context created");

                // Create new page for each test
                if (_page != null)
                {
                    await _page.CloseAsync();
                }
                
                _page = await _context.NewPageAsync();
                result.Logs.Add("New page created");

                // Navigate to Gmail
                await _page.GotoAsync("https://mail.google.com");
                result.Logs.Add("Navigated to Gmail");

                // Wait for the page to load
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                result.Logs.Add("Page loaded successfully");

                // Check if we need to sign in or if already signed in
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

                // Wait for email input field and enter email
                var emailInput = _page.Locator("input[type='email']");
                await emailInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
                await emailInput.FillAsync(email);
                result.Logs.Add("Email entered successfully");

                // Click Next button after email
                var nextButton = _page.Locator("text=Next");
                await nextButton.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                result.Logs.Add("Clicked Next after email");

                // Wait for password input field and enter password
                // Gmail initially shows a hidden password field, wait for the visible one to appear
                var passwordInput = _page.Locator("input[type='password']:not([aria-hidden='true'])");
                await passwordInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
                await passwordInput.FillAsync(password);
                result.Logs.Add("Password entered successfully");

                // Click Next button after password
                var passwordNextButton = _page.Locator("text=Next");
                await passwordNextButton.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                result.Logs.Add("Clicked Next after password");

                // Wait for Gmail inbox to load (check for inbox element)
                try
                {
                    // Wait for either the inbox to load or for 2FA prompt
                    await _page.WaitForSelectorAsync("div[role='main']", new PageWaitForSelectorOptions { Timeout = 30000 });
                    result.Logs.Add("Successfully navigated to Gmail inbox");
                    
                    // Take screenshot of successful login
                    var screenshotPath = $"screenshots/Gmail_Login_Success_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Screenshot saved: {screenshotPath}");

                    result.Passed = true;
                    result.Message = "Gmail login completed successfully";
                }
                catch (TimeoutException)
                {
                    // Check if 2FA is required
                    var twoFactorInput = _page.Locator("input[type='tel']");
                    if (await twoFactorInput.CountAsync() > 0)
                    {
                        result.Logs.Add("2FA input field detected - manual intervention required");
                        result.Passed = false;
                        result.Message = "2FA authentication required - test cannot complete automatically";
                        
                        // Take screenshot of 2FA prompt
                        var screenshotPath = $"screenshots/Gmail_Login_2FA_Required_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        result.Logs.Add($"2FA screenshot saved: {screenshotPath}");
                    }
                    else
                    {
                        // Check for other error messages
                        var errorMessage = _page.Locator("div[role='alert']");
                        if (await errorMessage.CountAsync() > 0)
                        {
                            var errorText = await errorMessage.TextContentAsync();
                            result.Logs.Add($"Error message found: {errorText}");
                            result.Passed = false;
                            result.Message = $"Login failed: {errorText}";
                        }
                        else
                        {
                            result.Logs.Add("Login timeout - inbox not loaded");
                            result.Passed = false;
                            result.Message = "Login timeout - inbox not loaded within expected time";
                        }
                        
                        // Take screenshot of failed login
                        var screenshotPath = $"screenshots/Gmail_Login_Failed_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        result.Logs.Add($"Failure screenshot saved: {screenshotPath}");
                    }
                }

                // Clean up if not keeping browser open
                if (!keepBrowserOpen)
                {
                    await CleanupAsync();
                }
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Message = $"Test failed with exception: {ex.Message}";
                result.Logs.Add($"Exception occurred: {ex}");
                
                // Take screenshot on exception
                if (_page != null)
                {
                    try
                    {
                        var screenshotPath = $"screenshots/Gmail_Login_Exception_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        result.Logs.Add($"Exception screenshot saved: {screenshotPath}");
                    }
                    catch (Exception screenshotEx)
                    {
                        result.Logs.Add($"Failed to take exception screenshot: {screenshotEx.Message}");
                    }
                }
            }

            result.Duration = DateTime.UtcNow - result.ExecutionTime;
            result.Logs.Add($"Test completed in {result.Duration.TotalSeconds:F2} seconds");

            return result;
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
                // Log cleanup errors but don't throw
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
