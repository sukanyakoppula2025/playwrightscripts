using System;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    class BedBathBeyondConfigTestRunner
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🛏️  Bed Bath & Beyond Config-Based Test Runner");
            Console.WriteLine("==============================================");
            
            try
            {
                // Load configuration
                var config = BedBathBeyondTestConfig.CreateDefault();
                
                Console.WriteLine($"Using configuration:");
                Console.WriteLine($"  Email: {config.Email}");
                Console.WriteLine($"  URL: {config.AccountUrl}");
                Console.WriteLine($"  Headless: {config.Headless}");
                Console.WriteLine($"  SlowMo: {config.SlowMo}ms");
                Console.WriteLine($"  Timeout: {config.Timeout}ms");
                Console.WriteLine($"  Take Screenshots: {config.TakeScreenshots}");
                Console.WriteLine($"  Record Video: {config.RecordVideo}");
                Console.WriteLine();
                
                // Validate configuration
                if (!config.IsValid())
                {
                    var errors = config.GetValidationErrors();
                    Console.WriteLine("❌ Configuration validation failed:");
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"  - {error}");
                    }
                    return;
                }
                
                Console.WriteLine("✅ Configuration is valid!");
                Console.WriteLine();
                Console.WriteLine("Starting Bed Bath & Beyond Login Test...");
                Console.WriteLine("Note: This test will open a browser window for you to see the automation in action.");
                Console.WriteLine();
                
                // Create and run the enhanced test
                using var bedBathTest = new BedBathBeyondLoginTestEnhanced(config);
                var result = await bedBathTest.RunBedBathBeyondLoginTestAsync();
                
                // Display results
                Console.WriteLine();
                Console.WriteLine("📊 Test Results");
                Console.WriteLine("===============");
                Console.WriteLine($"Test Name: {result.TestName}");
                Console.WriteLine($"Status: {(result.Passed ? "✅ PASSED" : "❌ FAILED")}");
                Console.WriteLine($"Message: {result.Message}");
                Console.WriteLine($"Duration: {result.Duration.TotalSeconds:F2} seconds");
                Console.WriteLine($"Start Time: {result.ExecutionTime:yyyy-MM-dd HH:mm:ss}");
                
                Console.WriteLine();
                Console.WriteLine("📝 Test Logs");
                Console.WriteLine("=============");
                foreach (var log in result.Logs)
                {
                    Console.WriteLine($"  {log}");
                }
                
                Console.WriteLine();
                if (result.Passed)
                {
                    Console.WriteLine("🎉 Test completed successfully! Check the screenshots folder for captured images.");
                }
                else
                {
                    Console.WriteLine("⚠️  Test failed. Check the screenshots folder for error details.");
                }
                
                Console.WriteLine();
                Console.WriteLine("Press any key to close the browser and exit...");
                Console.ReadKey();
                
                // Clean up
                await bedBathTest.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error running test: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }
        }
    }
}
