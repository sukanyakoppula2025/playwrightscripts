using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using System.IO; // Added for Path.GetFullPath

namespace GoogleSearchTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 Google Search Playwright Tests - Direct Execution");
            Console.WriteLine("==================================================");
            Console.WriteLine();

            try
            {
                // Create and run the test runner
                var testRunner = new GoogleSearchTestRunner();
                
                Console.WriteLine("Available Tests:");
                Console.WriteLine("1. Navigation Test - Navigate to Google UK");
                Console.WriteLine("2. Search Test - Search for 'Playwright C#'");
                Console.WriteLine("3. Suggestions Test - Check search suggestions");
                Console.WriteLine("4. Images Tab Test - Navigate to Images tab");
                Console.WriteLine("5. Empty Search Test - Test empty search behavior");
                Console.WriteLine();

                Console.Write("Enter test number (1-5) or 'all' to run all tests: ");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("❌ No input provided. Exiting.");
                    return;
                }

                if (input.ToLower() == "all")
                {
                    Console.WriteLine("🎬 Running all tests...");
                    await RunAllTests(testRunner);
                }
                else if (int.TryParse(input, out int testNumber) && testNumber >= 1 && testNumber <= 5)
                {
                    var testName = GetTestName(testNumber);
                    Console.WriteLine($"🎬 Running test: {testName}");
                    await RunSpecificTest(testRunner, testName);
                }
                else
                {
                    Console.WriteLine("❌ Invalid input. Please enter 1-5 or 'all'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static string GetTestName(int testNumber)
        {
            return testNumber switch
            {
                1 => "Navigation Test",
                2 => "Search Test", 
                3 => "Suggestions Test",
                4 => "Images Tab Test",
                5 => "Empty Search Test",
                _ => "Unknown Test"
            };
        }

        static async Task RunSpecificTest(GoogleSearchTestRunner testRunner, string testName)
        {
            try
            {
                var result = await testRunner.RunSpecificTestAsync(testName);
                
                Console.WriteLine();
                Console.WriteLine($"✅ Test '{testName}' completed!");
                Console.WriteLine($"Result: {(result.Passed ? "PASSED" : "FAILED")}");
                Console.WriteLine($"Message: {result.Message}");
                Console.WriteLine($"Duration: {result.Duration.TotalSeconds:F2} seconds");
                
                if (!string.IsNullOrEmpty(result.ScreenshotPath))
                {
                    Console.WriteLine($"Screenshot saved: {result.ScreenshotPath}");
                }
                
                if (!string.IsNullOrEmpty(result.VideoPath))
                {
                    Console.WriteLine($"Video saved: {result.VideoPath}");
                }

                Console.WriteLine();
                Console.WriteLine("Test Logs:");
                foreach (var log in result.Logs)
                {
                    Console.WriteLine($"  {log}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test '{testName}' failed with error: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }
        }

        static async Task RunAllTests(GoogleSearchTestRunner testRunner)
        {
            try
            {
                Console.WriteLine("🎬 Running all tests in a single browser session...");
                Console.WriteLine("This will be much faster than running tests individually!");
                Console.WriteLine();
                
                var allResults = await testRunner.RunAllTestsAsync();
                
                Console.WriteLine();
                Console.WriteLine("🎯 Test Summary:");
                Console.WriteLine("=================");
                
                var passed = 0;
                var total = allResults.Count;
                var totalDuration = TimeSpan.Zero;

                foreach (var result in allResults)
                {
                    Console.WriteLine();
                    Console.WriteLine($"🎬 {result.TestName}");
                    Console.WriteLine(new string('-', 50));
                    Console.WriteLine($"Result: {(result.Passed ? "✅ PASSED" : "❌ FAILED")}");
                    Console.WriteLine($"Duration: {result.Duration.TotalSeconds:F2} seconds");
                    Console.WriteLine($"Message: {result.Message}");
                    
                    if (result.Passed) passed++;
                    totalDuration += result.Duration;
                    
                    // Show screenshot info if available
                    if (!string.IsNullOrEmpty(result.ScreenshotPath))
                    {
                        Console.WriteLine($"Screenshot: {result.ScreenshotPath}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("🎯 Final Summary:");
                Console.WriteLine($"Total Tests: {total}");
                Console.WriteLine($"Passed: {passed}");
                Console.WriteLine($"Failed: {total - passed}");
                Console.WriteLine($"Success Rate: {(double)passed / total * 100:F1}%");
                Console.WriteLine($"Total Execution Time: {totalDuration.TotalSeconds:F2} seconds");
                Console.WriteLine($"Average Time per Test: {totalDuration.TotalSeconds / total:F2} seconds");
                
                // Generate HTML Report
                Console.WriteLine();
                Console.WriteLine("📄 Generating HTML Test Report...");
                TestReportGenerator.SaveHtmlReport(allResults, "test-report.html");
                
                // Open the report in default browser
                try
                {
                    var reportPath = Path.GetFullPath("test-report.html");
                    Console.WriteLine($"🌐 Opening HTML report in browser: {reportPath}");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = reportPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Could not open report automatically: {ex.Message}");
                    Console.WriteLine($"📄 Please open the file manually: test-report.html");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error running all tests: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }
        }
    }
}
