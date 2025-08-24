using System;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    class BedBathBeyondFurnitureTestRunner
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🛏️  Bed Bath & Beyond Furniture Navigation Test Runner");
            Console.WriteLine("=====================================================");
            
            try
            {
                Console.WriteLine("Starting Bed Bath & Beyond Furniture Navigation Test...");
                Console.WriteLine("This test will navigate to the main page and find the Furniture menu.");
                Console.WriteLine();
                
                // Create and run the furniture navigation test
                using var furnitureTest = new BedBathBeyondFurnitureNavigationTest();
                var result = await furnitureTest.RunFurnitureNavigationTestAsync();
                
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
                await furnitureTest.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error running test: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }
        }
    }
}
