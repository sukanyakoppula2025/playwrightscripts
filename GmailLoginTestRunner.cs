using System;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    class GmailLoginTestRunner
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 Gmail Login Test Runner");
            Console.WriteLine("==========================");
            
            // Get credentials from user input (in production, use secure methods)
            Console.Write("Enter Gmail address: ");
            var email = Console.ReadLine();
            
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("❌ Email address is required!");
                return;
            }
            
            Console.Write("Enter password: ");
            var password = ReadPassword();
            
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("❌ Password is required!");
                return;
            }
            
            Console.WriteLine();
            Console.WriteLine("Starting Gmail Login Test...");
            Console.WriteLine("Note: This test will open a browser window for you to see the automation in action.");
            Console.WriteLine();
            
            // Create and run the test
            using var gmailTest = new GmailLoginTest();
            var result = await gmailTest.RunGmailLoginTestAsync(email, password, keepBrowserOpen: true);
            
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
            await gmailTest.DisposeAsync();
        }
        
        private static string ReadPassword()
        {
            var password = "";
            ConsoleKeyInfo key;
            
            do
            {
                key = Console.ReadKey(true);
                
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);
            
            Console.WriteLine();
            return password;
        }
    }
}
