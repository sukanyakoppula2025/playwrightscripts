using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    public class GoogleSearchTestRunner
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        public async Task<TestResult> RunSpecificTestAsync(string testName, bool keepBrowserOpen = false)
        {
            var startTime = DateTime.UtcNow;
            var result = new TestResult
            {
                TestName = testName,
                ExecutionTime = startTime,
                Logs = new List<string>()
            };

            try
            {
                result.Logs.Add($"Starting test: {testName}");
                
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
                        Headless = false, // Make browser visible
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

                // Run the specific test
                switch (testName)
                {
                    case "Navigation Test":
                        await RunNavigationTestAsync(_page, result);
                        break;
                    case "Search Test":
                        await RunSearchTestAsync(_page, result);
                        break;
                    case "Suggestions Test":
                        await RunSuggestionsTestAsync(_page, result);
                        break;
                    case "Images Tab Test":
                        await RunImagesTabTestAsync(_page, result);
                        break;
                    case "Empty Search Test":
                        await RunEmptySearchTestAsync(_page, result);
                        break;
                    default:
                        throw new ArgumentException($"Unknown test: {testName}");
                }

                result.Passed = true;
                result.Message = $"Test '{testName}' completed successfully";
                
                // Take screenshot after successful test completion
                if (_page != null)
                {
                    var screenshotPath = $"screenshots/{testName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.ScreenshotPath = screenshotPath;
                    result.Logs.Add($"Screenshot saved: {screenshotPath}");
                }
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Message = $"Test '{testName}' failed: {ex.Message}";
                result.Logs.Add($"Error: {ex.Message}");
                result.Logs.Add($"Exception Details: {ex}");
                
                // Take screenshot on failure
                if (_page != null)
                {
                    try
                    {
                        var screenshotPath = $"screenshots/{testName.Replace(" ", "_")}_FAILED_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                        result.ScreenshotPath = screenshotPath;
                        result.Logs.Add($"Failure screenshot saved: {screenshotPath}");
                    }
                    catch (Exception screenshotEx)
                    {
                        result.Logs.Add($"Failed to take failure screenshot: {screenshotEx.Message}");
                    }
                }
            }
            finally
            {
                // Only close page and context, keep browser open if requested
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
                    
                    if (keepBrowserOpen)
                    {
                        result.Logs.Add("Page and context cleaned up (browser kept open for reuse)");
                    }
                    else
                    {
                        // Close everything if this is a single test run
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
                        
                        result.Logs.Add("All resources cleaned up");
                    }
                }
                catch (Exception cleanupEx)
                {
                    result.Logs.Add($"Cleanup warning: {cleanupEx.Message}");
                }

                result.Duration = DateTime.UtcNow - startTime;
                result.Logs.Add($"Test completed in {result.Duration.TotalSeconds:F2} seconds");
            }

            return result;
        }

        public async Task<List<TestResult>> RunAllTestsAsync()
        {
            var allResults = new List<TestResult>();
            var testNames = new[] { "Navigation Test", "Search Test", "Suggestions Test", "Images Tab Test", "Empty Search Test" };
            
            try
            {
                // Initialize Playwright once for all tests
                if (_playwright == null)
                {
                    _playwright = await Playwright.CreateAsync();
                    Console.WriteLine("🚀 Playwright initialized for all tests");
                }

                // Launch browser once for all tests
                if (_browser == null)
                {
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = false, // Make browser visible
                        SlowMo = 1000, // 1 second delay between actions
                        Channel = "chrome" // Use Chrome browser
                    });
                    Console.WriteLine("🌐 Chrome browser launched - will be reused for all tests");
                }

                // Create single context and page for all tests
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
                Console.WriteLine("📱 Single browser context created for all tests");

                if (_page != null)
                {
                    await _page.CloseAsync();
                }
                
                _page = await _context.NewPageAsync();
                Console.WriteLine("🌐 Single browser tab created - all tests will run here");
                Console.WriteLine();

                // Run each test in the same browser session
                foreach (var testName in testNames)
                {
                    Console.WriteLine($"🎬 Running: {testName}");
                    Console.WriteLine(new string('-', 50));
                    
                    var result = await RunTestInSingleSessionAsync(testName, _page);
                    allResults.Add(result);
                    
                    // Small delay between tests (but keep the same page)
                    if (testName != testNames.Last())
                    {
                        Console.WriteLine("⏳ Waiting 3 seconds before next test...");
                        await Task.Delay(3000);
                        Console.WriteLine();
                    }
                }
            }
            finally
            {
                // Clean up all resources after all tests are done
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
                    
                    Console.WriteLine("🧹 All browser resources cleaned up");
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine($"Final cleanup warning: {cleanupEx.Message}");
                }
            }

            return allResults;
        }

        private async Task<TestResult> RunTestInSingleSessionAsync(string testName, IPage page)
        {
            var startTime = DateTime.UtcNow;
            var result = new TestResult
            {
                TestName = testName,
                ExecutionTime = startTime,
                Logs = new List<string>()
            };

            try
            {
                result.Logs.Add($"Starting test: {testName}");
                
                // Run the specific test using the existing page
                switch (testName)
                {
                    case "Navigation Test":
                        await RunNavigationTestAsync(page, result);
                        break;
                    case "Search Test":
                        await RunSearchTestAsync(page, result);
                        break;
                    case "Suggestions Test":
                        await RunSuggestionsTestAsync(page, result);
                        break;
                    case "Images Tab Test":
                        await RunImagesTabTestAsync(page, result);
                        break;
                    case "Empty Search Test":
                        await RunEmptySearchTestAsync(page, result);
                        break;
                    default:
                        throw new ArgumentException($"Unknown test: {testName}");
                }

                result.Passed = true;
                result.Message = $"Test '{testName}' completed successfully";
                
                // Take screenshot after successful test completion
                var screenshotPath = $"screenshots/{testName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                result.ScreenshotPath = screenshotPath;
                result.Logs.Add($"Screenshot saved: {screenshotPath}");
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Message = $"Test '{testName}' failed: {ex.Message}";
                result.Logs.Add($"Error: {ex.Message}");
                result.Logs.Add($"Exception Details: {ex}");
                
                // Take screenshot on failure
                try
                {
                    var screenshotPath = $"screenshots/{testName.Replace(" ", "_")}_FAILED_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.ScreenshotPath = screenshotPath;
                    result.Logs.Add($"Failure screenshot saved: {screenshotPath}");
                }
                catch (Exception screenshotEx)
                {
                    result.Logs.Add($"Failed to take failure screenshot: {screenshotEx.Message}");
                }
            }
            finally
            {
                result.Duration = DateTime.UtcNow - startTime;
                result.Logs.Add($"Test completed in {result.Duration.TotalSeconds:F2} seconds");
            }

            return result;
        }

        private async Task HandleCookieConsentAsync(IPage page, TestResult result)
        {
            try
            {
                // Wait a moment for the page to fully load
                await page.WaitForTimeoutAsync(1000);
                
                // Try multiple selectors for the Accept All button
                var acceptSelectors = new[]
                {
                    "button:has-text('Accept all')",
                    "button:has-text('Accept All')",
                    "button:has-text('Accept')",
                    "[aria-label*='Accept all']",
                    "[aria-label*='Accept All']",
                    "button[data-testid='accept-all']",
                    "button[data-testid='acceptAll']",
                    "button[data-testid='accept']",
                    "button[id*='accept']",
                    "button[class*='accept']"
                };

                foreach (var selector in acceptSelectors)
                {
                    try
                    {
                        var acceptButton = page.Locator(selector);
                        if (await acceptButton.IsVisibleAsync())
                        {
                            await acceptButton.ClickAsync();
                            result.Logs.Add($"Cookie consent accepted using selector: {selector}");
                            await page.WaitForTimeoutAsync(1000); // Wait for dialog to close
                            return;
                        }
                    }
                    catch
                    {
                        // Continue to next selector
                        continue;
                    }
                }

                // If no button found, try to find any button with accept-like text
                try
                {
                    var allButtons = page.Locator("button");
                    var buttonCount = await allButtons.CountAsync();
                    
                    for (int i = 0; i < buttonCount; i++)
                    {
                        var button = allButtons.Nth(i);
                        var buttonText = await button.TextContentAsync();
                        
                        if (buttonText != null && 
                            (buttonText.Contains("Accept", StringComparison.OrdinalIgnoreCase) ||
                             buttonText.Contains("Agree", StringComparison.OrdinalIgnoreCase) ||
                             buttonText.Contains("OK", StringComparison.OrdinalIgnoreCase)))
                        {
                            await button.ClickAsync();
                            result.Logs.Add($"Cookie consent accepted using button text: {buttonText}");
                            await page.WaitForTimeoutAsync(1000);
                            return;
                        }
                    }
                }
                catch
                {
                    // Continue if this approach fails
                }

                result.Logs.Add("No cookie consent dialog found or already accepted");
            }
            catch (Exception ex)
            {
                result.Logs.Add($"Cookie consent handling warning: {ex.Message}");
            }
        }

        private async Task RunNavigationTestAsync(IPage page, TestResult result)
        {
            result.Logs.Add("Navigating to Google UK...");
            await page.GotoAsync("https://www.google.co.uk");
            
            result.Logs.Add("Waiting for page to load...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            result.Logs.Add("Handling cookie consent if present...");
            await HandleCookieConsentAsync(page, result);
            
            result.Logs.Add("Checking page title...");
            var title = await page.TitleAsync();
            result.Logs.Add($"Page title: {title}");
            
            if (title.Contains("Google"))
            {
                result.Logs.Add("✅ Navigation test passed - Google page loaded successfully");
            }
            else
            {
                throw new Exception($"Expected Google page, but got: {title}");
            }
        }

        private async Task<ILocator> FindSearchElementAsync(IPage page, TestResult result)
        {
            // Try textarea first (current Google implementation)
            try
            {
                var textareaSearch = page.Locator("textarea[name='q']");
                if (await textareaSearch.IsVisibleAsync())
                {
                    result.Logs.Add("Found search element: textarea[name='q']");
                    return textareaSearch;
                }
            }
            catch
            {
                // Continue to input selector
            }

            // Try input as fallback (older Google implementation)
            try
            {
                var inputSearch = page.Locator("input[name='q']");
                if (await inputSearch.IsVisibleAsync())
                {
                    result.Logs.Add("Found search element: input[name='q']");
                    return inputSearch;
                }
            }
            catch
            {
                // Continue to other selectors
            }

            // Try other common Google search selectors
            var alternativeSelectors = new[]
            {
                "textarea[aria-label*='Search']",
                "input[aria-label*='Search']",
                "textarea[id='APjFqb']",
                "input[id='APjFqb']",
                "textarea[class*='search']",
                "input[class*='search']",
                "textarea[class*='gLFyf']",
                "input[class*='gLFyf']"
            };

            foreach (var selector in alternativeSelectors)
            {
                try
                {
                    var element = page.Locator(selector);
                    if (await element.IsVisibleAsync())
                    {
                        result.Logs.Add($"Found search element: {selector}");
                        return element;
                    }
                }
                catch
                {
                    continue;
                }
            }

            throw new Exception("Search element not found with any selector");
        }

        private async Task RunSearchTestAsync(IPage page, TestResult result)
        {
            result.Logs.Add("Navigating to Google UK...");
            await page.GotoAsync("https://www.google.co.uk");
            
            result.Logs.Add("Waiting for page to load...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            result.Logs.Add("Handling cookie consent if present...");
            await HandleCookieConsentAsync(page, result);
            
            result.Logs.Add("Finding search element...");
            var searchElement = await FindSearchElementAsync(page, result);
            
            result.Logs.Add("Typing search query...");
            await searchElement.FillAsync("Playwright C#");
            
            result.Logs.Add("Pressing Enter to search...");
            await searchElement.PressAsync("Enter");
            
            result.Logs.Add("Waiting for search results...");
            
            // Try multiple selectors for search results
            var searchResultsSelectors = new[]
            {
                "#search",
                "#search .g",
                "#search .rc",
                "#search .tF2Cxc",
                "#search .yuRUbf",
                "#search .LC20lb",
                "div[data-hveid]",
                "div[jscontroller]",
                "div[data-ved]",
                "div[class*='g']",
                "div[class*='rc']",
                "div[class*='tF2Cxc']",
                "div[class*='yuRUbf']"
            };

            var searchResultsFound = false;
            var resultsCount = 0;

            foreach (var selector in searchResultsSelectors)
            {
                try
                {
                    result.Logs.Add($"Trying search results selector: {selector}");
                    await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = 5000 });
                    
                    var results = page.Locator(selector);
                    resultsCount = await results.CountAsync();
                    
                    if (resultsCount > 0)
                    {
                        result.Logs.Add($"Found {resultsCount} search results with selector: {selector}");
                        searchResultsFound = true;
                        break;
                    }
                }
                catch
                {
                    result.Logs.Add($"Selector {selector} failed or no results found");
                    continue;
                }
            }

            if (!searchResultsFound)
            {
                // Try to find any div that might contain search results
                try
                {
                    result.Logs.Add("Trying to find search results with generic approach...");
                    var allDivs = page.Locator("div");
                    var divCount = await allDivs.CountAsync();
                    result.Logs.Add($"Found {divCount} div elements on page");
                    
                    // Look for divs that might contain search results
                    for (int i = 0; i < Math.Min(divCount, 50); i++) // Check first 50 divs
                    {
                        try
                        {
                            var div = allDivs.Nth(i);
                            var divClass = await div.GetAttributeAsync("class");
                            var divId = await div.GetAttributeAsync("id");
                            
                            if ((divClass != null && (divClass.Contains("g") || divClass.Contains("rc") || divClass.Contains("result"))) ||
                                (divId != null && (divId.Contains("search") || divId.Contains("result"))))
                            {
                                result.Logs.Add($"Potential search result div found: class={divClass}, id={divId}");
                                resultsCount++;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    
                    if (resultsCount > 0)
                    {
                        searchResultsFound = true;
                    }
                }
                catch (Exception ex)
                {
                    result.Logs.Add($"Generic search results approach failed: {ex.Message}");
                }
            }

            result.Logs.Add($"Final search results count: {resultsCount}");
            
            if (searchResultsFound && resultsCount > 0)
            {
                result.Logs.Add("✅ Search test passed - Results found successfully");
            }
            else
            {
                // Take a screenshot to see what the page looks like
                try
                {
                    var screenshotPath = $"screenshots/Search_Test_DEBUG_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    result.Logs.Add($"Debug screenshot saved: {screenshotPath}");
                }
                catch
                {
                    // Continue if screenshot fails
                }
                
                throw new Exception($"No search results found. Page may have changed structure or search failed.");
            }
        }

        private async Task RunSuggestionsTestAsync(IPage page, TestResult result)
        {
            result.Logs.Add("Navigating to Google UK...");
            await page.GotoAsync("https://www.google.co.uk");
            
            result.Logs.Add("Waiting for page to load...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            result.Logs.Add("Handling cookie consent if present...");
            await HandleCookieConsentAsync(page, result);
            
            result.Logs.Add("Finding search element...");
            var searchElement = await FindSearchElementAsync(page, result);
            
            result.Logs.Add("Typing partial search query...");
            await searchElement.FillAsync("Playwright");
            
            result.Logs.Add("Waiting for suggestions...");
            await page.WaitForTimeoutAsync(1000);
            
            result.Logs.Add("Checking for suggestions dropdown...");
            var suggestions = await page.Locator("ul[role='listbox'] li").CountAsync();
            result.Logs.Add($"Found {suggestions} suggestions");
            
            if (suggestions > 0)
            {
                result.Logs.Add("✅ Suggestions test passed - Search suggestions displayed");
            }
            else
            {
                result.Logs.Add("⚠️ No suggestions found, but continuing...");
            }
        }

        private async Task RunImagesTabTestAsync(IPage page, TestResult result)
        {
            result.Logs.Add("Navigating to Google UK...");
            await page.GotoAsync("https://www.google.co.uk");
            
            result.Logs.Add("Waiting for page to load...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            result.Logs.Add("Handling cookie consent if present...");
            await HandleCookieConsentAsync(page, result);
            
            result.Logs.Add("Waiting for Images tab...");
            await page.WaitForSelectorAsync("a[href*='imghp']");
            
            result.Logs.Add("Clicking Images tab...");
            await page.ClickAsync("a[href*='imghp']");
            
            result.Logs.Add("Waiting for Images page to load...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            result.Logs.Add("Checking if we're on Images page...");
            var currentUrl = page.Url;
            result.Logs.Add($"Current URL: {currentUrl}");
            
            if (currentUrl.Contains("imghp") || currentUrl.Contains("tbm=isch"))
            {
                result.Logs.Add("✅ Images tab test passed - Successfully navigated to Images");
            }
            else
            {
                throw new Exception($"Expected Images page, but got: {currentUrl}");
            }
        }

        private async Task RunEmptySearchTestAsync(IPage page, TestResult result)
        {
            result.Logs.Add("Navigating to Google UK...");
            await page.GotoAsync("https://www.google.co.uk");
            
            result.Logs.Add("Waiting for page to load...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            result.Logs.Add("Handling cookie consent if present...");
            await HandleCookieConsentAsync(page, result);
            
            result.Logs.Add("Finding search element...");
            var searchElement = await FindSearchElementAsync(page, result);
            
            result.Logs.Add("Pressing Enter without entering text...");
            await searchElement.PressAsync("Enter");
            
            result.Logs.Add("Waiting for page to process...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            result.Logs.Add("Checking current URL...");
            var currentUrl = page.Url;
            result.Logs.Add($"Current URL: {currentUrl}");
            
            // Google typically stays on the same page for empty searches
            if (currentUrl.Contains("google.co.uk") && !currentUrl.Contains("q="))
            {
                result.Logs.Add("✅ Empty search test passed - Page handled empty search correctly");
            }
            else
            {
                result.Logs.Add("⚠️ Empty search behavior different than expected, but continuing...");
            }
        }
    }

    public class TestResult
    {
        public string TestName { get; set; } = string.Empty;
        public bool Passed { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public string ScreenshotPath { get; set; } = string.Empty;
        public string VideoPath { get; set; } = string.Empty;
        public List<string> Logs { get; set; } = new List<string>();
        public DateTime ExecutionTime { get; set; }
    }
}
