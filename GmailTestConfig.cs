using System;
using System.Collections.Generic; // Added missing import for List

namespace GoogleSearchTests
{
    public class GmailTestConfig
    {
        // Gmail credentials (in production, use secure configuration or environment variables)
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        
        // Test configuration
        public bool Headless { get; set; } = false; // Set to true for CI/CD environments
        public int SlowMo { get; set; } = 1000; // Delay between actions in milliseconds
        public string BrowserChannel { get; set; } = "chrome"; // chrome, msedge, etc.
        public int Timeout { get; set; } = 30000; // Timeout for waiting for elements in milliseconds
        
        // Browser settings
        public int ViewportWidth { get; set; } = 1920;
        public int ViewportHeight { get; set; } = 1080;
        public bool RecordVideo { get; set; } = true;
        public string VideoDirectory { get; set; } = "videos/";
        public string ScreenshotDirectory { get; set; } = "screenshots/";
        
        // Test behavior
        public bool KeepBrowserOpen { get; set; } = false; // Useful for debugging
        public bool TakeScreenshots { get; set; } = true;
        public bool Handle2FA { get; set; } = false; // Set to true if you want to handle 2FA manually
        
        // Gmail specific settings
        public string GmailUrl { get; set; } = "https://mail.google.com";
        public int LoginWaitTime { get; set; } = 5000; // Wait time after login attempts
        
        // Load configuration from environment variables (for CI/CD)
        public static GmailTestConfig LoadFromEnvironment()
        {
            return new GmailTestConfig
            {
                Email = Environment.GetEnvironmentVariable("GMAIL_EMAIL") ?? "",
                Password = Environment.GetEnvironmentVariable("GMAIL_PASSWORD") ?? "",
                Headless = bool.Parse(Environment.GetEnvironmentVariable("GMAIL_HEADLESS") ?? "false"),
                SlowMo = int.Parse(Environment.GetEnvironmentVariable("GMAIL_SLOWMO") ?? "1000"),
                BrowserChannel = Environment.GetEnvironmentVariable("GMAIL_BROWSER_CHANNEL") ?? "chrome",
                Timeout = int.Parse(Environment.GetEnvironmentVariable("GMAIL_TIMEOUT") ?? "30000"),
                ViewportWidth = int.Parse(Environment.GetEnvironmentVariable("GMAIL_VIEWPORT_WIDTH") ?? "1920"),
                ViewportHeight = int.Parse(Environment.GetEnvironmentVariable("GMAIL_VIEWPORT_HEIGHT") ?? "1080"),
                RecordVideo = bool.Parse(Environment.GetEnvironmentVariable("GMAIL_RECORD_VIDEO") ?? "true"),
                VideoDirectory = Environment.GetEnvironmentVariable("GMAIL_VIDEO_DIR") ?? "videos/",
                ScreenshotDirectory = Environment.GetEnvironmentVariable("GMAIL_SCREENSHOT_DIR") ?? "screenshots/",
                KeepBrowserOpen = bool.Parse(Environment.GetEnvironmentVariable("GMAIL_KEEP_BROWSER_OPEN") ?? "false"),
                TakeScreenshots = bool.Parse(Environment.GetEnvironmentVariable("GMAIL_TAKE_SCREENSHOTS") ?? "true"),
                Handle2FA = bool.Parse(Environment.GetEnvironmentVariable("GMAIL_HANDLE_2FA") ?? "false"),
                GmailUrl = Environment.GetEnvironmentVariable("GMAIL_URL") ?? "https://mail.google.com",
                LoginWaitTime = int.Parse(Environment.GetEnvironmentVariable("GMAIL_LOGIN_WAIT_TIME") ?? "5000")
            };
        }
        
        // Validate configuration
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        }
        
        // Get validation errors
        public string[] GetValidationErrors()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrEmpty(Email))
                errors.Add("Email is required");
                
            if (string.IsNullOrEmpty(Password))
                errors.Add("Password is required");
                
            if (SlowMo < 0)
                errors.Add("SlowMo must be non-negative");
                
            if (Timeout <= 0)
                errors.Add("Timeout must be positive");
                
            if (ViewportWidth <= 0 || ViewportHeight <= 0)
                errors.Add("Viewport dimensions must be positive");
                
            return errors.ToArray();
        }
    }
}
