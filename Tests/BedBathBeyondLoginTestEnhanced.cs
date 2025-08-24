using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    public class BedBathBeyondLoginTestEnhanced : IDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;
        private readonly BedBathBeyondTestConfig _config;

        public BedBathBeyondLoginTestEnhanced(BedBathBeyondTestConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<TestResult> RunBedBathBeyondLoginTestAsync()
        {
            var startTime = DateTime.UtcNow;
            var result = new TestResult
            {
                TestName = "Bed Bath & Beyond Login Test (Enhanced)",
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

                result.Logs.Add($"Starting Bed Bath & Beyond Login Test for: {_config.Email}");
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

                // Enable incognito mode if configured
                if (_config.IncognitoMode)
                {
                    contextOptions.IsMobile = false;
                    // Note: Playwright automatically handles incognito mode for Chromium
                    result.Logs.Add("Creating incognito/private browser context");
                }

                _context = await _browser.NewContextAsync(contextOptions);
                result.Logs.Add($"Browser context created with viewport {_config.ViewportWidth}x{_config.ViewportHeight}");

                // Create page
                _page = await _context.NewPageAsync();
                result.Logs.Add("New page created");

                // Navigate to Bed Bath & Beyond account page
                await _page.GotoAsync(_config.AccountUrl);
                result.Logs.Add($"Navigated to {_config.AccountUrl}");

                // Wait for the page to load with more flexible strategy
                try
                {
                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 15000 });
                    result.Logs.Add("Page loaded successfully (NetworkIdle)");
                }
                catch (TimeoutException)
                {
                    // Fallback to DOMContentLoaded if NetworkIdle times out
                    try
                    {
                        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = 10000 });
                        result.Logs.Add("Page loaded successfully (DOMContentLoaded)");
                    }
                    catch (TimeoutException)
                    {
                        result.Logs.Add("Page load timeout, continuing with current state");
                    }
                }

                // Additional wait for page to stabilize
                await Task.Delay(_config.PageLoadWaitTime);
                result.Logs.Add($"Waited {_config.PageLoadWaitTime}ms for page to stabilize");

                // Handle cookie consent if enabled
                if (_config.HandleCookieConsent)
                {
                    await HandleCookieConsentAsync(result);
                }

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
            // First, try to find the email input field directly - this indicates we're on the login page
            var emailInput = _page!.Locator("#login-email");
            if (await emailInput.CountAsync() > 0)
            {
                result.Logs.Add("Email input field found - already on login page");
            }
            else
            {
                // If email input not found, we need to click a sign-in button to get to the login page
                result.Logs.Add("Email input not found - looking for sign-in button to navigate to login page");
                
                // Look for a "Sign In" button (avoiding Facebook OAuth)
                var signInButton = _page.Locator("button:has-text('Sign In'), a:has-text('Sign In'):not(:has-text('Facebook'))").First;
                if (await signInButton.CountAsync() > 0)
                {
                    var buttonText = await signInButton.TextContentAsync() ?? "Unknown";
                    result.Logs.Add($"Sign in button found: '{buttonText}' - clicking to navigate to login page");
                    
                    try
                    {
                        await signInButton.ClickAsync();
                        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 10000 });
                        result.Logs.Add("Clicked sign-in button, waiting for login page to load");
                        
                        // Wait for the email input to appear
                        await emailInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
                        result.Logs.Add("Email input field now visible on login page");
                    }
                    catch (Exception ex)
                    {
                        result.Logs.Add($"Error navigating to login page: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    result.Logs.Add("No sign-in button found - may already be on login page");
                }
            }

            // Check if any popup windows opened (like Facebook OAuth) BEFORE filling the form
            var pages = _context!.Pages;
            if (pages.Count > 1)
            {
                result.Logs.Add($"⚠️  Detected {pages.Count} pages - popup may have opened");
                
                // Close any popup windows and focus back to main page
                for (int i = 1; i < pages.Count; i++)
                {
                    var popup = pages[i];
                    var popupUrl = popup.Url;
                    result.Logs.Add($"Closing popup window: {popupUrl}");
                    await popup.CloseAsync();
                }
                
                // Wait a moment for focus to return to main page
                await Task.Delay(2000);
                result.Logs.Add("Closed popup windows, focus returned to main page");
                
                // Wait for the page to stabilize after popup closure
                await Task.Delay(1000);
                result.Logs.Add("Waited for page to stabilize after popup closure");
            }

            // Now enter email using the specific ID AFTER closing popups
            try
            {
                await emailInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = _config.Timeout });
                await emailInput.FillAsync(_config.Email);
                result.Logs.Add("Email entered successfully");
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Failed to enter email: {ex.Message}");
                return;
            }

            // Wait and enter password using the specific ID AFTER closing popups
            var passwordInput = _page.Locator("#login-password");
            try
            {
                await passwordInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = _config.Timeout });
                await passwordInput.FillAsync(_config.Password);
                result.Logs.Add("Password entered successfully");
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Failed to enter password: {ex.Message}");
                return;
            }

            // Submit the form directly instead of clicking a button
            try
            {
                // Check if we're already on the account dashboard (form may have auto-submitted)
                var currentUrl = _page.Url;
                result.Logs.Add($"Current URL after form entry: {currentUrl}");
                
                // Check if we're actually on an account page or still on login page
                if (currentUrl.Contains("dashboard") || currentUrl.Contains("account") || currentUrl.Contains("profile"))
                {
                    result.Logs.Add("Already on account page - form may have auto-submitted");
                    return;
                }
                else
                {
                    result.Logs.Add("Still on login page - submitting form directly");
                    
                    // Submit the form with name "login" directly
                    var loginForm = _page.Locator("form[name='login']");
                    if (await loginForm.CountAsync() > 0)
                    {
                        result.Logs.Add("Found login form, submitting directly...");
                        await loginForm.EvaluateAsync("form => form.submit()");
                        result.Logs.Add("Form submitted successfully");
                        
                        // Wait for form submission to complete
                        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 15000 });
                        result.Logs.Add("Form submission completed, waiting for page to load");
                    }
                    else
                    {
                        result.Logs.Add("Login form not found, trying alternative form selectors...");
                        
                        // Try alternative form selectors
                        var alternativeForm = _page.Locator("form:has(input[name='email']), form:has(input[name='password'])");
                        if (await alternativeForm.CountAsync() > 0)
                        {
                            result.Logs.Add("Found alternative form, submitting...");
                            await alternativeForm.EvaluateAsync("form => form.submit()");
                            result.Logs.Add("Alternative form submitted successfully");
                            
                            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 15000 });
                            result.Logs.Add("Alternative form submission completed");
                        }
                        else
                        {
                            result.Logs.Add("No form found - login may have happened automatically");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Form submission failed: {ex.Message}");
                
                // Check if we're already on the account page (form may have auto-submitted)
                var currentUrl = _page.Url;
                if (currentUrl.Contains("dashboard") || currentUrl.Contains("account") || currentUrl.Contains("profile"))
                {
                    result.Logs.Add("Form appears to have auto-submitted - continuing with verification");
                }
                else
                {
                    result.Logs.Add("Form submission failed and not on account page");
                    return;
                }
            }

            // Wait for login processing
            await Task.Delay(2000);
            result.Logs.Add("Waited for login processing");
        }

        private async Task WaitForLoginCompletionAsync(TestResult result)
        {
            try
            {
                // First, check the current URL to see where we actually are
                var currentUrl = _page!.Url;
                result.Logs.Add($"Current URL before waiting for dashboard: {currentUrl}");
                
                // Wait for either the account dashboard to load or for error messages
                await _page.WaitForSelectorAsync("div[class*='account'], div[class*='dashboard'], div[class*='profile'], [data-testid='account-dashboard']", new PageWaitForSelectorOptions { Timeout = _config.Timeout });
                result.Logs.Add("Successfully navigated to account dashboard");
                
                // Double-check the URL to confirm we're really on an account page
                var finalUrl = _page.Url;
                result.Logs.Add($"Final URL after dashboard detection: {finalUrl}");
                
                if (_config.TakeScreenshots)
                {
                    var screenshotPath = $"{_config.ScreenshotDirectory}BedBathBeyond_Login_Success_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Success screenshot saved: {screenshotPath}");
                }

                result.Passed = true;
                result.Message = "Bed Bath & Beyond login completed successfully";
            }
            catch (TimeoutException)
            {
                // Check for error messages
                await CheckForErrorMessagesAsync(result);
            }
        }

        private async Task HandleCookieConsentAsync(TestResult result)
        {
            try
            {
                // Common cookie consent button selectors
                var cookieSelectors = new[]
                {
                    "button:has-text('Accept')",
                    "button:has-text('Accept All')",
                    "button:has-text('Accept Cookies')",
                    "button:has-text('I Accept')",
                    "button:has-text('OK')",
                    "button:has-text('Got It')",
                    "[data-testid='accept-cookies']",
                    ".cookie-accept",
                    "#cookie-accept"
                };

                foreach (var selector in cookieSelectors)
                {
                    var cookieButton = _page!.Locator(selector);
                    if (await cookieButton.CountAsync() > 0)
                    {
                        result.Logs.Add("Cookie consent dialog found, accepting cookies");
                        await cookieButton.ClickAsync();
                        await Task.Delay(1000); // Wait for dialog to close
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Cookie consent handling failed: {ex.Message}");
            }
        }

        private async Task CheckForErrorMessagesAsync(TestResult result)
        {
            // Common error message selectors for Bed Bath & Beyond
            var errorSelectors = new[]
            {
                "div[class*='error']",
                "div[class*='alert']",
                "span[class*='error']",
                "p[class*='error']",
                "[data-testid='error-message']",
                ".error-message",
                ".alert-error",
                ".form-error",
                "div[role='alert']"
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
                        var screenshotPath = $"{_config.ScreenshotDirectory}BedBathBeyond_Login_Error_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        result.Logs.Add($"Error screenshot saved: {screenshotPath}");
                    }
                    return;
                }
            }

            // Check if we're still on the login page (indicating login failed)
            var currentUrl = _page!.Url;
            if (currentUrl.Contains("login") || currentUrl.Contains("signin") || currentUrl.Contains("myaccount"))
            {
                result.Logs.Add("Login appears to have failed - still on login/account page");
                result.Passed = false;
                result.Message = "Login failed - still on login/account page";
                
                if (_config.TakeScreenshots)
                {
                    var screenshotPath = $"{_config.ScreenshotDirectory}BedBathBeyond_Login_Failed_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Failure screenshot saved: {screenshotPath}");
                }
            }
            else
            {
                result.Logs.Add("Login timeout - account dashboard not loaded within expected time");
                result.Passed = false;
                result.Message = "Login timeout - account dashboard not loaded within expected time";
                
                if (_config.TakeScreenshots)
                {
                    var screenshotPath = $"{_config.ScreenshotDirectory}BedBathBeyond_Login_Timeout_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Timeout screenshot saved: {screenshotPath}");
                }
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
                    var screenshotPath = $"{_config.ScreenshotDirectory}BedBathBeyond_Login_Exception_{DateTime.Now:yyyyMMdd_HHmmss}.png";
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
