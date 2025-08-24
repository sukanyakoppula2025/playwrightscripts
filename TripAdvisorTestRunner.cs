using System;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    class TripAdvisorTestRunner
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🛎️  TripAdvisor Poole Search Test Runner");
            Console.WriteLine("=========================================");
            
            try
            {
                Console.WriteLine("Starting TripAdvisor Poole Search Test...");
                Console.WriteLine("This test will:");
                Console.WriteLine("1. Navigate to TripAdvisor UK");
                Console.WriteLine("2. Accept cookies");
                Console.WriteLine("3. Search for 'Poole'");
                Console.WriteLine("4. Take screenshots");
                Console.WriteLine("5. Generate HTML report");
                Console.WriteLine();
                
                // Create and run the TripAdvisor test
                using var tripAdvisorTest = new TripAdvisorPooleSearchTest();
                var result = await tripAdvisorTest.RunTripAdvisorPooleSearchTestAsync();
                
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
                    Console.WriteLine("🎉 Test completed successfully!");
                    Console.WriteLine("📸 Screenshots saved in the 'screenshots' folder");
                    Console.WriteLine("📄 HTML report generated in the 'reports' folder");
                }
                else
                {
                    Console.WriteLine("⚠️  Test failed. Check the screenshots folder for error details.");
                }
                
                Console.WriteLine();
                Console.WriteLine("Press any key to close the browser and exit...");
                Console.ReadKey();
                
                // Clean up
                await tripAdvisorTest.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error running test: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }
        }
    }
}
