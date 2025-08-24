using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    public class BedBathBeyondLoginTest : IDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        public async Task<TestResult> RunBedBathBeyondLoginTestAsync(string email, string password, bool keepBrowserOpen = false)
        {
            var startTime = DateTime.UtcNow;
            var result = new TestResult
            {
                TestName = "Bed Bath & Beyond Login Test",
                ExecutionTime = startTime,
                Logs = new List<string>()
            };

            try
            {
                result.Logs.Add($"Starting Bed Bath & Beyond Login Test for: {email}");
                
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

                // Create browser context
                var contextOptions = new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
                };

                // Enable incognito mode for clean sessions
                contextOptions.IsMobile = false;
                result.Logs.Add("Creating incognito/private browser context");

                _context = await _browser.NewContextAsync(contextOptions);
                result.Logs.Add("Browser context created");

                // Create new page for each test
                if (_page != null)
                {
                    await _page.CloseAsync();
                }
                
                _page = await _context.NewPageAsync();
                result.Logs.Add("New page created");

                // Navigate to Bed Bath & Beyond account page
                await _page.GotoAsync("https://www.bedbathandbeyond.com/myaccount");
                result.Logs.Add("Navigated to Bed Bath & Beyond account page");

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
                await Task.Delay(2000);
                result.Logs.Add("Waited for page to stabilize");

                // Handle cookie consent if present
                await HandleCookieConsentAsync(result);

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

                // Look for sign in button and click if needed
                var signInButton = _page.Locator("a.swh_Account_signInTile, a[data-tid*='SIGNIN'], button:has-text('Sign In'), a:has-text('Sign In')").First;
                if (await signInButton.CountAsync() > 0)
                {
                    var buttonText = await signInButton.TextContentAsync() ?? "Unknown";
                    var buttonClass = await signInButton.GetAttributeAsync("class") ?? "Unknown";
                    var buttonHref = await signInButton.GetAttributeAsync("href") ?? "Unknown";
                    
                    result.Logs.Add($"Sign in button found: Text='{buttonText}', Class='{buttonClass}', Href='{buttonHref}'");
                    
                    // Check if this is a Facebook or other OAuth button
                    if (buttonText.Contains("Facebook") || buttonClass.Contains("facebook") || buttonHref.Contains("facebook"))
                    {
                        result.Logs.Add("⚠️  Facebook OAuth button detected - looking for main Bed Bath & Beyond sign-in button");
                        // Try to find the main Bed Bath & Beyond sign-in button instead
                        var mainSignInButton = _page.Locator("a:has-text('Sign In'):not(:has-text('Facebook')), button:has-text('Sign In'):not(:has-text('Facebook')), [data-tid*='SIGNIN']:not(:has-text('Facebook'))").First;
                        if (await mainSignInButton.CountAsync() > 0)
                        {
                            signInButton = mainSignInButton;
                            buttonText = await signInButton.TextContentAsync() ?? "Unknown";
                            result.Logs.Add($"Main sign-in button found: Text='{buttonText}'");
                        }
                    }
                    
                    result.Logs.Add("Proceeding with login");
                    
                    // Wait for the button to be visible and clickable
                    try
                    {
                        await signInButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
                        result.Logs.Add("Sign in button is visible, clicking...");
                        await signInButton.ClickAsync();
                        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 10000 });
                    }
                    catch (TimeoutException)
                    {
                        result.Logs.Add("Sign in button not visible, trying to force click...");
                        // Try to force click if the button exists but is hidden
                        await signInButton.ClickAsync(new LocatorClickOptions { Force = true });
                        await Task.Delay(2000); // Wait for page to respond
                    }
                }
                else
                {
                    result.Logs.Add("Already on login page or signed in");
                }

                // First, try to find the email input field directly - this indicates we're on the login page
                var emailInput = _page.Locator("#login-email");
                if (await emailInput.CountAsync() > 0)
                {
                    result.Logs.Add("Email input field found - already on login page");
                }
                else
                {
                    // If email input not found, we need to click a sign-in button to get to the login page
                    result.Logs.Add("Email input not found - looking for sign-in button to navigate to login page");
                    
                    // Look for a "Sign In" button (avoiding Facebook OAuth)
                    var mainSignInButton = _page.Locator("button:has-text('Sign In'), a:has-text('Sign In'):not(:has-text('Facebook'))").First;
                    if (await mainSignInButton.CountAsync() > 0)
                    {
                        var buttonText = await mainSignInButton.TextContentAsync() ?? "Unknown";
                        result.Logs.Add($"Sign in button found: '{buttonText}' - clicking to navigate to login page");
                        
                        try
                        {
                            await mainSignInButton.ClickAsync();
                            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 10000 });
                            result.Logs.Add("Clicked sign-in button, waiting for login page to load");
                            
                            // Wait for the email input to appear
                            await emailInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
                            result.Logs.Add("Email input field now visible on login page");
                        }
                        catch (Exception ex)
                        {
                            result.Logs.Add($"Error navigating to login page: {ex.Message}");
                            return result;
                        }
                    }
                    else
                    {
                        result.Logs.Add("No sign-in button found - may already be on login page");
                    }
                }

                // Now enter email using the specific ID
                try
                {
                    await emailInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
                    await emailInput.FillAsync(email);
                    result.Logs.Add("Email entered successfully");
                }
                catch (Exception ex)
                {
                    result.Logs.Add($"Failed to enter email: {ex.Message}");
                    return result;
                }

                // Wait and enter password using the specific ID for login form only
                var passwordInput = _page.Locator("#login-password");
                try
                {
                    await passwordInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
                    await passwordInput.FillAsync(password);
                    result.Logs.Add("Password entered successfully");
                }
                catch (Exception ex)
                {
                    result.Logs.Add($"Failed to enter password: {ex.Message}");
                    return result;
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
                        return result;
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
                        return result;
                    }
                }

                // Wait for successful login or handle errors
                try
                {
                    // Wait for either the account dashboard to load or for error messages
                    await _page.WaitForSelectorAsync("div[class*='account'], div[class*='dashboard'], div[class*='profile'], [data-testid='account-dashboard']", new PageWaitForSelectorOptions { Timeout = 30000 });
                    result.Logs.Add("Successfully navigated to account dashboard");
                    
                    // Take screenshot of successful login
                    var screenshotPath = $"screenshots/BedBathBeyond_Login_Success_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Screenshot saved: {screenshotPath}");

                    result.Passed = true;
                    result.Message = "Bed Bath & Beyond login completed successfully";
                }
                catch (TimeoutException)
                {
                    // Check for error messages
                    await CheckForErrorMessagesAsync(result);
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
                        var screenshotPath = $"screenshots/BedBathBeyond_Login_Exception_{DateTime.Now:yyyyMMdd_HHmmss}.png";
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
                    
                    // Take screenshot of failed login
                    var screenshotPath = $"screenshots/BedBathBeyond_Login_Error_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Error screenshot saved: {screenshotPath}");
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
                
                // Take screenshot of failed login
                var screenshotPath = $"screenshots/BedBathBeyond_Login_Failed_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                result.Logs.Add($"Failure screenshot saved: {screenshotPath}");
            }
            else
            {
                result.Logs.Add("Login timeout - account dashboard not loaded within expected time");
                result.Passed = false;
                result.Message = "Login timeout - account dashboard not loaded within expected time";
                
                // Take screenshot of timeout
                var screenshotPath = $"screenshots/BedBathBeyond_Login_Timeout_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                result.Logs.Add($"Timeout screenshot saved: {screenshotPath}");
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
