using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    public class BedBathBeyondFurnitureNavigationTest : IDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        public async Task<TestResult> RunFurnitureNavigationTestAsync()
        {
            var startTime = DateTime.UtcNow;
            var result = new TestResult
            {
                TestName = "Bed Bath & Beyond Furniture Navigation Test",
                ExecutionTime = startTime,
                Logs = new List<string>()
            };

            try
            {
                result.Logs.Add("Starting Bed Bath & Beyond Furniture Navigation Test");
                
                // Initialize Playwright
                _playwright = await Playwright.CreateAsync();
                result.Logs.Add("Playwright initialized");

                // Launch browser
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 1000
                });
                result.Logs.Add("Chrome browser launched");

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

                // Create page
                _page = await _context.NewPageAsync();
                result.Logs.Add("New page created");

                // Navigate to main Bed Bath & Beyond page
                await _page.GotoAsync("https://www.bedbathandbeyond.com/");
                result.Logs.Add("Navigated to main Bed Bath & Beyond page");

                // Wait for the page to load
                try
                {
                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 15000 });
                    result.Logs.Add("Page loaded successfully (NetworkIdle)");
                }
                catch (TimeoutException)
                {
                    await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = 10000 });
                    result.Logs.Add("Page loaded successfully (DOMContentLoaded)");
                }

                // Wait for page to stabilize
                await Task.Delay(2000);
                result.Logs.Add("Waited for page to stabilize");

                // Handle cookie consent if present
                await HandleCookieConsentAsync(result);

                // Find and hover over the Furniture menu item
                result.Logs.Add("Looking for Furniture menu item...");
                
                // Try multiple selectors for the Furniture menu
                var furnitureMenu = _page.Locator("a:has-text('Furniture'), [data-testid*='furniture'], .nav-item:has-text('Furniture'), .menu-item:has-text('Furniture')").First;
                
                if (await furnitureMenu.CountAsync() > 0)
                {
                    var menuText = await furnitureMenu.TextContentAsync() ?? "Unknown";
                    result.Logs.Add($"Found Furniture menu: '{menuText}'");
                    
                    // Hover over the Furniture menu to show submenu
                    await furnitureMenu.HoverAsync();
                    result.Logs.Add("Hovered over Furniture menu");
                    
                    // Wait for submenu to appear
                    await Task.Delay(1000);
                    result.Logs.Add("Waited for submenu to appear");
                    
                    // Now look for "Sofas and Couches" submenu item
                    result.Logs.Add("Looking for 'Sofas and Couches' submenu item...");
                    
                    var sofasSubmenu = _page.Locator("a:has-text('Sofas and Couches'), a:has-text('Sofas & Couches'), [data-testid*='sofas'], .submenu-item:has-text('Sofas')").First;
                    
                    if (await sofasSubmenu.CountAsync() > 0)
                    {
                        var submenuText = await sofasSubmenu.TextContentAsync() ?? "Unknown";
                        result.Logs.Add($"Found 'Sofas and Couches' submenu: '{submenuText}'");
                        
                        // Click on the Sofas and Couches submenu
                        await sofasSubmenu.ClickAsync();
                        result.Logs.Add("Clicked on 'Sofas and Couches' submenu");
                        
                        // Wait for navigation to complete
                        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 15000 });
                        result.Logs.Add("Navigation to Sofas and Couches page completed");
                        
                        // Verify we're on the correct page
                        var currentUrl = _page.Url;
                        result.Logs.Add($"Current URL after navigation: {currentUrl}");
                        
                        // Check if page contains sofa-related content
                        var pageContent = await _page.ContentAsync();
                        if (pageContent.Contains("sofa", StringComparison.OrdinalIgnoreCase) || 
                            pageContent.Contains("couch", StringComparison.OrdinalIgnoreCase) ||
                            currentUrl.Contains("sofa") || currentUrl.Contains("couch"))
                        {
                            result.Logs.Add("Successfully navigated to Sofas and Couches page");
                            
                            // Take screenshot of success
                            var screenshotPath = $"screenshots/BedBathBeyond_Furniture_Navigation_Success_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                            result.Logs.Add($"Success screenshot saved: {screenshotPath}");
                            
                            result.Passed = true;
                            result.Message = "Successfully navigated to Furniture > Sofas and Couches page";
                        }
                        else
                        {
                            result.Logs.Add("Page navigation may have failed - content doesn't match expectations");
                            result.Passed = false;
                            result.Message = "Navigation completed but page content doesn't match expectations";
                        }
                    }
                    else
                    {
                        result.Logs.Add("'Sofas and Couches' submenu not found");
                        result.Passed = false;
                        result.Message = "Could not find Sofas and Couches submenu item";
                    }
                }
                else
                {
                    result.Logs.Add("Furniture menu not found");
                    result.Passed = false;
                    result.Message = "Could not find Furniture menu item";
                }
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Test failed with exception: {ex.Message}");
                result.Passed = false;
                result.Message = $"Test failed: {ex.Message}";
                
                if (_page != null)
                {
                    try
                    {
                        var screenshotPath = $"screenshots/BedBathBeyond_Furniture_Navigation_Error_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        result.Logs.Add($"Error screenshot saved: {screenshotPath}");
                    }
                    catch (Exception screenshotEx)
                    {
                        result.Logs.Add($"Failed to take error screenshot: {screenshotEx.Message}");
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
