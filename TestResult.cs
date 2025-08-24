using System;
using System.Collections.Generic;

namespace GoogleSearchTests
{
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
