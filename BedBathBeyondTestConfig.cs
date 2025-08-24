using System;
using System.Collections.Generic;

namespace GoogleSearchTests
{
    public class BedBathBeyondTestConfig
    {
        // Bed Bath & Beyond credentials (in production, use secure configuration or environment variables)
        public string Email { get; set; } = "sukanyakoppula2025@gmail.com";
        public string Password { get; set; } = "Sukanya2020#";
        
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
        public bool HandleCookieConsent { get; set; } = true;
        public bool IncognitoMode { get; set; } = true; // Use incognito/private mode for clean sessions
        
        // Bed Bath & Beyond specific settings
        public string AccountUrl { get; set; } = "https://www.bedbathandbeyond.com/myaccount";
        public int PageLoadWaitTime { get; set; } = 2000; // Wait time after page load in milliseconds
        
        // Load configuration from environment variables (for CI/CD)
        public static BedBathBeyondTestConfig LoadFromEnvironment()
        {
            return new BedBathBeyondTestConfig
            {
                Email = Environment.GetEnvironmentVariable("BEDBATH_EMAIL") ?? "sukanyakoppula2025@gmail.com",
                Password = Environment.GetEnvironmentVariable("BEDBATH_PASSWORD") ?? "Sukanya2020#",
                Headless = bool.Parse(Environment.GetEnvironmentVariable("BEDBATH_HEADLESS") ?? "false"),
                SlowMo = int.Parse(Environment.GetEnvironmentVariable("BEDBATH_SLOWMO") ?? "1000"),
                BrowserChannel = Environment.GetEnvironmentVariable("BEDBATH_BROWSER_CHANNEL") ?? "chrome",
                Timeout = int.Parse(Environment.GetEnvironmentVariable("BEDBATH_TIMEOUT") ?? "30000"),
                ViewportWidth = int.Parse(Environment.GetEnvironmentVariable("BEDBATH_VIEWPORT_WIDTH") ?? "1920"),
                ViewportHeight = int.Parse(Environment.GetEnvironmentVariable("BEDBATH_VIEWPORT_HEIGHT") ?? "1080"),
                RecordVideo = bool.Parse(Environment.GetEnvironmentVariable("BEDBATH_RECORD_VIDEO") ?? "true"),
                VideoDirectory = Environment.GetEnvironmentVariable("BEDBATH_VIDEO_DIR") ?? "videos/",
                ScreenshotDirectory = Environment.GetEnvironmentVariable("BEDBATH_SCREENSHOT_DIR") ?? "screenshots/",
                KeepBrowserOpen = bool.Parse(Environment.GetEnvironmentVariable("BEDBATH_KEEP_BROWSER_OPEN") ?? "false"),
                TakeScreenshots = bool.Parse(Environment.GetEnvironmentVariable("BEDBATH_TAKE_SCREENSHOTS") ?? "true"),
                HandleCookieConsent = bool.Parse(Environment.GetEnvironmentVariable("BEDBATH_HANDLE_COOKIES") ?? "true"),
                AccountUrl = Environment.GetEnvironmentVariable("BEDBATH_ACCOUNT_URL") ?? "https://www.bedbathandbeyond.com/myaccount",
                PageLoadWaitTime = int.Parse(Environment.GetEnvironmentVariable("BEDBATH_PAGE_LOAD_WAIT") ?? "2000")
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
        
        // Create a default configuration for testing
        public static BedBathBeyondTestConfig CreateDefault()
        {
            return new BedBathBeyondTestConfig
            {
                Email = "sukanyakoppula2025@gmail.com",
                Password = "Sukanya2020#",
                Headless = false, // Run in visible mode to see what's happening
                SlowMo = 1000,
                Timeout = 30000,
                ViewportWidth = 1920,
                ViewportHeight = 1080,
                RecordVideo = true,
                TakeScreenshots = true,
                HandleCookieConsent = true,
                KeepBrowserOpen = true, // Keep browser open for debugging
                IncognitoMode = true // Use incognito/private mode for clean sessions
            };
        }
    }
}
