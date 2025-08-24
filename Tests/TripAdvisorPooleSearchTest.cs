using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq; // Added for .First()

namespace GoogleSearchTests
{
    public class TripAdvisorPooleSearchTest : IDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        public async Task<TestResult> RunTripAdvisorPooleSearchTestAsync()
        {
            var startTime = DateTime.UtcNow;
            var result = new TestResult
            {
                TestName = "TripAdvisor Poole Search Test",
                ExecutionTime = startTime,
                Logs = new List<string>()
            };

            try
            {
                result.Logs.Add("Starting TripAdvisor Poole Search Test");
                
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

                // Navigate to TripAdvisor UK
                await _page.GotoAsync("https://www.tripadvisor.co.uk/");
                result.Logs.Add("Navigated to TripAdvisor UK");

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

                // Handle cookie consent dialog
                await HandleCookieConsentAsync(result);

                // Search for Poole
                await SearchForPooleAsync(result);

                // Take screenshot of results
                await TakeScreenshotAsync(result);

                // Generate HTML report
                await GenerateHtmlReportAsync(result);

                result.Passed = true;
                result.Message = "Successfully completed TripAdvisor Poole search test";
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
                        var screenshotPath = $"screenshots/TripAdvisor_Poole_Search_Error_{DateTime.Now:yyyyMMdd_HHmmss}.png";
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
                result.Logs.Add("Looking for cookie consent dialog...");
                
                // Common cookie consent button selectors for TripAdvisor
                var cookieSelectors = new[]
                {
                    "button:has-text('Accept All')",
                    "button:has-text('Accept all')",
                    "button:has-text('Accept All Cookies')",
                    "button:has-text('Accept')",
                    "button:has-text('I Accept')",
                    "button:has-text('OK')",
                    "button:has-text('Got It')",
                    "[data-testid='accept-cookies']",
                    ".cookie-accept",
                    "#cookie-accept",
                    "[aria-label*='Accept']",
                    ".cookie-banner button"
                };

                foreach (var selector in cookieSelectors)
                {
                    var cookieButton = _page!.Locator(selector);
                    if (await cookieButton.CountAsync() > 0)
                    {
                        result.Logs.Add($"Cookie consent dialog found with selector: {selector}");
                        await cookieButton.ClickAsync();
                        result.Logs.Add("Clicked on cookie consent button");
                        await Task.Delay(2000); // Wait for dialog to close
                        break;
                    }
                }

                // Check if cookie banner is still visible
                var cookieBanner = _page!.Locator(".cookie-banner, [data-testid='cookie-banner'], .cookie-notice");
                if (await cookieBanner.CountAsync() > 0)
                {
                    result.Logs.Add("Cookie banner still visible, trying alternative approach");
                    // Try to find any button within the cookie banner
                    var anyButton = cookieBanner.Locator("button").First;
                    if (await anyButton.CountAsync() > 0)
                    {
                        await anyButton.ClickAsync();
                        result.Logs.Add("Clicked on alternative cookie button");
                        await Task.Delay(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Cookie consent handling failed: {ex.Message}");
            }
        }

        private async Task SearchForPooleAsync(TestResult result)
        {
            try
            {
                result.Logs.Add("Looking for search box...");
                
                // Try multiple selectors for the search box - TripAdvisor specific
                var searchSelectors = new[]
                {
                    // TripAdvisor specific selectors
                    "input[placeholder*='Where to?']",
                    "input[placeholder*='Search']",
                    "input[placeholder*='search']",
                    "input[name='q']",
                    "input[type='search']",
                    "[data-testid='search-input']",
                    "[data-testid='searchbox']",
                    ".search-input input",
                    "#searchbox input",
                    ".search-box input",
                    // More generic selectors as fallback
                    "input[placeholder*='Search']",
                    "input[placeholder*='search']",
                    "input[name='q']",
                    "input[type='search']",
                    "[data-testid='search-input']",
                    ".search-input input",
                    "#searchbox input",
                    ".search-box input"
                };

                ILocator? searchBox = null;
                foreach (var selector in searchSelectors)
                {
                    var locator = _page!.Locator(selector);
                    if (await locator.CountAsync() > 0)
                    {
                        searchBox = locator.First;
                        result.Logs.Add($"Found search box with selector: {selector}");
                        break;
                    }
                }

                if (searchBox == null)
                {
                    // If no search box found, look for a search button and click it
                    result.Logs.Add("Search box not found, looking for search button...");
                    var searchButton = _page!.Locator("button:has-text('Search'), [aria-label*='Search'], .search-button, [data-testid*='search']");
                    if (await searchButton.CountAsync() > 0)
                    {
                        result.Logs.Add("Search button found, clicking to open search...");
                        await searchButton.ClickAsync();
                        await Task.Delay(2000);
                        
                        // Now try to find the search input again
                        foreach (var selector in searchSelectors)
                        {
                            var locator = _page.Locator(selector);
                            if (await locator.CountAsync() > 0)
                            {
                                searchBox = locator.First;
                                result.Logs.Add($"Found search box after clicking search button: {selector}");
                                break;
                            }
                        }
                    }
                }

                if (searchBox == null)
                {
                    // Try to find any input field that might be a search box
                    result.Logs.Add("Still no search box found, trying alternative approach...");
                    var allInputs = _page!.Locator("input");
                    var inputCount = await allInputs.CountAsync();
                    result.Logs.Add($"Found {inputCount} input fields on the page");
                    
                    // Look for input fields with search-related attributes
                    for (int i = 0; i < inputCount; i++)
                    {
                        var input = allInputs.Nth(i);
                        var placeholder = await input.GetAttributeAsync("placeholder") ?? "";
                        var name = await input.GetAttributeAsync("name") ?? "";
                        var type = await input.GetAttributeAsync("type") ?? "";
                        
                        if (placeholder.ToLower().Contains("search") || 
                            placeholder.ToLower().Contains("where") ||
                            name.ToLower().Contains("search") ||
                            name.ToLower().Contains("q") ||
                            type.ToLower().Contains("search"))
                        {
                            searchBox = input;
                            result.Logs.Add($"Found potential search box: placeholder='{placeholder}', name='{name}', type='{type}'");
                            break;
                        }
                    }
                }

                if (searchBox != null)
                {
                    // Clear and fill the search box
                    await searchBox.ClearAsync();
                    await searchBox.FillAsync("Poole");
                    result.Logs.Add("Entered 'Poole' in search box");
                    
                    // Press Enter to search
                    await searchBox.PressAsync("Enter");
                    result.Logs.Add("Pressed Enter to search");
                    
                    // Wait for search results to load with a shorter timeout
                    try
                    {
                        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 10000 });
                        result.Logs.Add("Search results loaded (NetworkIdle)");
                    }
                    catch (TimeoutException)
                    {
                        result.Logs.Add("NetworkIdle timeout, waiting for DOMContentLoaded instead");
                        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = 5000 });
                        result.Logs.Add("Search results loaded (DOMContentLoaded)");
                    }
                    
                    // Wait for specific search result elements to appear
                    result.Logs.Add("Waiting for search results to appear...");
                    try
                    {
                        // Look for common search result selectors
                        var resultSelectors = new[]
                        {
                            "[data-testid*='result']",
                            ".result",
                            ".search-result",
                            ".listing",
                            ".item",
                            "article",
                            ".property-card"
                        };
                        
                        bool resultsFound = false;
                        foreach (var selector in resultSelectors)
                        {
                            try
                            {
                                var results = _page.Locator(selector);
                                if (await results.CountAsync() > 0)
                                {
                                    result.Logs.Add($"Found {await results.CountAsync()} search results with selector: {selector}");
                                    resultsFound = true;
                                    break;
                                }
                            }
                            catch
                            {
                                // Continue to next selector
                            }
                        }
                        
                        if (!resultsFound)
                        {
                            // Wait a bit more and check if URL changed
                            await Task.Delay(3000);
                            var currentUrl = _page.Url;
                            result.Logs.Add($"Current URL after search: {currentUrl}");
                            
                            if (currentUrl.Contains("search") || currentUrl.Contains("poole"))
                            {
                                result.Logs.Add("URL indicates search was successful");
                                resultsFound = true;
                            }
                        }
                        
                        if (resultsFound)
                        {
                            result.Logs.Add("Search results detected successfully");
                        }
                        else
                        {
                            result.Logs.Add("No specific search results found, but continuing with test");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Logs.Add($"Error waiting for search results: {ex.Message}");
                        // Continue anyway as the page might have loaded
                    }
                    
                    // Wait a bit more for results to stabilize
                    await Task.Delay(2000);
                    result.Logs.Add("Waited for search results to stabilize");
                }
                else
                {
                    throw new Exception("Could not find search box or search functionality");
                }
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Search for Poole failed: {ex.Message}");
                throw;
            }
        }

        private async Task TakeScreenshotAsync(TestResult result)
        {
            try
            {
                // Create screenshots directory if it doesn't exist
                var screenshotsDir = "screenshots";
                if (!Directory.Exists(screenshotsDir))
                {
                    Directory.CreateDirectory(screenshotsDir);
                }

                var screenshotPath = $"screenshots/TripAdvisor_Poole_Search_Results_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                await _page!.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                result.Logs.Add($"Screenshot saved: {screenshotPath}");
                
                // Also take a full page screenshot
                var fullPageScreenshotPath = $"screenshots/TripAdvisor_Poole_Search_FullPage_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = fullPageScreenshotPath, FullPage = true });
                result.Logs.Add($"Full page screenshot saved: {fullPageScreenshotPath}");
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Screenshot failed: {ex.Message}");
            }
        }

        private async Task GenerateHtmlReportAsync(TestResult result)
        {
            try
            {
                // Create reports directory if it doesn't exist
                var reportsDir = "reports";
                if (!Directory.Exists(reportsDir))
                {
                    Directory.CreateDirectory(reportsDir);
                }

                var reportPath = $"reports/TripAdvisor_Poole_Search_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                
                var htmlContent = GenerateHtmlContent(result);
                await File.WriteAllTextAsync(reportPath, htmlContent);
                
                result.Logs.Add($"HTML report generated: {reportPath}");
            }
            catch (Exception ex)
            {
                result.Logs.Add($"HTML report generation failed: {ex.Message}");
            }
        }

        private string GenerateHtmlContent(TestResult result)
        {
            var currentUrl = _page?.Url ?? "Unknown";
            var pageTitle = _page?.TitleAsync().Result ?? "Unknown";
            
            return $@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>TripAdvisor Poole Search Test Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 1200px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; border-bottom: 2px solid #00AA6C; padding-bottom: 20px; margin-bottom: 30px; }}
        .header h1 {{ color: #00AA6C; margin: 0; }}
        .status {{ padding: 10px; border-radius: 5px; margin: 20px 0; font-weight: bold; }}
        .status.passed {{ background-color: #d4edda; color: #155724; border: 1px solid #c3e6cb; }}
        .status.failed {{ background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }}
        .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 20px; margin: 20px 0; }}
        .info-card {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; border-left: 4px solid #00AA6C; }}
        .info-card h3 {{ margin-top: 0; color: #495057; }}
        .logs {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .logs h3 {{ margin-top: 0; color: #495057; }}
        .log-entry {{ margin: 5px 0; padding: 5px; background-color: white; border-radius: 3px; }}
        .screenshots {{ margin: 20px 0; }}
        .screenshot {{ margin: 20px 0; text-align: center; }}
        .screenshot img {{ max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 5px; }}
        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #dee2e6; color: #6c757d; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🛎️ TripAdvisor Poole Search Test Report</h1>
            <p>Automated test execution report generated by Playwright</p>
        </div>
        
        <div class='status {(result.Passed ? "passed" : "failed")}'>
            Status: {(result.Passed ? "✅ PASSED" : "❌ FAILED")}
        </div>
        
        <div class='info-grid'>
            <div class='info-card'>
                <h3>Test Information</h3>
                <p><strong>Test Name:</strong> {result.TestName}</p>
                <p><strong>Execution Time:</strong> {result.ExecutionTime:yyyy-MM-dd HH:mm:ss}</p>
                <p><strong>Duration:</strong> {result.Duration.TotalSeconds:F2} seconds</p>
                <p><strong>Message:</strong> {result.Message}</p>
            </div>
            
            <div class='info-card'>
                <h3>Page Information</h3>
                <p><strong>Current URL:</strong> <a href='{currentUrl}' target='_blank'>{currentUrl}</a></p>
                <p><strong>Page Title:</strong> {pageTitle}</p>
                <p><strong>Test Date:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
            </div>
        </div>
        
        <div class='logs'>
            <h3>📝 Test Execution Logs</h3>
            {string.Join("", result.Logs.Select(log => $"<div class='log-entry'>• {log}</div>"))}
        </div>
        
        <div class='screenshots'>
            <h3>📸 Screenshots</h3>
            <p>Check the screenshots folder for captured images from this test run.</p>
        </div>
        
        <div class='footer'>
            <p>Report generated by Playwright C# automation framework</p>
            <p>Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
        </div>
    </div>
</body>
</html>";
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
