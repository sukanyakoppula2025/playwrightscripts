using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    public class HtmlReportDemo
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("📊 HTML Report Generator Demo");
            Console.WriteLine("=============================");
            
            try
            {
                Console.WriteLine("Creating a sample test result and generating HTML report...");
                Console.WriteLine();
                
                // Create a sample test result (you can replace this with actual test data)
                var testResult = new TestResult
                {
                    TestName = "Bed Bath & Beyond Login Test",
                    ExecutionTime = DateTime.UtcNow.AddMinutes(-10),
                    Logs = new List<string>
                    {
                        "Starting Bed Bath & Beyond Login Test",
                        "Playwright initialized",
                        "Chrome browser launched",
                        "Creating incognito/private browser context",
                        "Browser context created",
                        "New page created",
                        "Navigated to Bed Bath & Beyond account page",
                        "Page loaded successfully (NetworkIdle)",
                        "Waited for page to stabilize",
                        "Looking for cookie consent dialog...",
                        "Cookie consent dialog found, accepting cookies",
                        "Clicked on cookie consent button",
                        "Looking for sign-in button...",
                        "Sign-in button found, clicking to open login form",
                        "Found login form, filling email field",
                        "Entered email: sukanyakoppula2025@gmail.com",
                        "Found password field, filling password",
                        "Entered password successfully",
                        "Form submitted successfully",
                        "Already on account page - form may have auto-submitted",
                        "Test completed successfully"
                    },
                    Passed = true,
                    Message = "Successfully logged into Bed Bath & Beyond account"
                };
                
                // Calculate duration
                testResult.Duration = DateTime.UtcNow - testResult.ExecutionTime;
                
                // Generate HTML report
                Console.WriteLine("Generating HTML report...");
                await SimpleHtmlReportGenerator.GenerateReportAsync(testResult);
                
                Console.WriteLine();
                Console.WriteLine("🎉 HTML report generated successfully!");
                Console.WriteLine("📁 Check the 'reports' folder for the generated HTML file.");
                Console.WriteLine();
                Console.WriteLine("You can open the HTML file in any web browser to view the report.");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in demo: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }
        }
    }
}
