using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq; // Added for .Count() and .Sum()

namespace GoogleSearchTests
{
    public class TestReportGenerator
    {
        public static string GenerateHtmlReport(List<TestResult> testResults, string reportTitle = "Playwright Test Report")
        {
            var html = new StringBuilder();
            
            // HTML Header
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine($"    <title>{reportTitle}</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        * { margin: 0; padding: 0; box-sizing: border-box; }");
            html.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; background: #f5f5f5; }");
            html.AppendLine("        .container { max-width: 1200px; margin: 0 auto; padding: 20px; }");
            html.AppendLine("        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; border-radius: 10px; margin-bottom: 30px; text-align: center; }");
            html.AppendLine("        .header h1 { font-size: 2.5em; margin-bottom: 10px; }");
            html.AppendLine("        .header p { font-size: 1.2em; opacity: 0.9; }");
            html.AppendLine("        .summary { background: white; padding: 25px; border-radius: 10px; margin-bottom: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }");
            html.AppendLine("        .summary-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-top: 20px; }");
            html.AppendLine("        .summary-item { text-align: center; padding: 20px; border-radius: 8px; }");
            html.AppendLine("        .summary-item.passed { background: #d4edda; color: #155724; border: 1px solid #c3e6cb; }");
            html.AppendLine("        .summary-item.failed { background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }");
            html.AppendLine("        .summary-item.total { background: #d1ecf1; color: #0c5460; border: 1px solid #bee5eb; }");
            html.AppendLine("        .summary-item.time { background: #fff3cd; color: #856404; border: 1px solid #ffeaa7; }");
            html.AppendLine("        .summary-number { font-size: 2em; font-weight: bold; margin-bottom: 5px; }");
            html.AppendLine("        .summary-label { font-size: 0.9em; opacity: 0.8; }");
            html.AppendLine("        .test-results { margin-bottom: 30px; }");
            html.AppendLine("        .test-item { background: white; margin-bottom: 20px; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }");
            html.AppendLine("        .test-header { padding: 20px; border-left: 5px solid; }");
            html.AppendLine("        .test-header.passed { border-left-color: #28a745; background: #f8fff9; }");
            html.AppendLine("        .test-header.failed { border-left-color: #dc3545; background: #fff8f8; }");
            html.AppendLine("        .test-title { font-size: 1.3em; font-weight: bold; margin-bottom: 10px; }");
            html.AppendLine("        .test-status { display: inline-block; padding: 5px 15px; border-radius: 20px; font-size: 0.9em; font-weight: bold; }");
            html.AppendLine("        .test-status.passed { background: #28a745; color: white; }");
            html.AppendLine("        .test-status.failed { background: #dc3545; color: white; }");
            html.AppendLine("        .test-details { padding: 0 20px 20px; }");
            html.AppendLine("        .test-info { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 15px; margin-bottom: 20px; }");
            html.AppendLine("        .info-item { background: #f8f9fa; padding: 15px; border-radius: 5px; }");
            html.AppendLine("        .info-label { font-size: 0.8em; color: #6c757d; margin-bottom: 5px; }");
            html.AppendLine("        .info-value { font-weight: bold; }");
            html.AppendLine("        .test-screenshot { margin: 20px 0; }");
            html.AppendLine("        .test-screenshot img { max-width: 100%; height: auto; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }");
            html.AppendLine("        .test-logs { background: #f8f9fa; padding: 15px; border-radius: 5px; margin-top: 15px; }");
            html.AppendLine("        .test-logs h4 { margin-bottom: 10px; color: #495057; }");
            html.AppendLine("        .log-entry { margin: 5px 0; padding: 5px 0; border-bottom: 1px solid #e9ecef; }");
            html.AppendLine("        .log-entry:last-child { border-bottom: none; }");
            html.AppendLine("        .footer { text-align: center; padding: 20px; color: #6c757d; font-size: 0.9em; }");
            html.AppendLine("        .timestamp { background: #e9ecef; padding: 10px; border-radius: 5px; margin-top: 15px; text-align: center; }");
            html.AppendLine("        @media (max-width: 768px) {");
            html.AppendLine("            .container { padding: 10px; }");
            html.AppendLine("            .header { padding: 20px; }");
            html.AppendLine("            .header h1 { font-size: 2em; }");
            html.AppendLine("            .summary-grid { grid-template-columns: repeat(2, 1fr); }");
            html.AppendLine("        }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            
            // Header
            html.AppendLine("    <div class=\"container\">");
            html.AppendLine("        <div class=\"header\">");
            html.AppendLine($"            <h1>{reportTitle}</h1>");
            html.AppendLine("            <p>Automated Playwright Test Results for Google UK</p>");
            html.AppendLine("        </div>");
            
            // Summary Section
            var totalTests = testResults.Count;
            var passedTests = testResults.Count(t => t.Passed);
            var failedTests = totalTests - passedTests;
            var totalDuration = TimeSpan.FromTicks(testResults.Sum(t => t.Duration.Ticks));
            var successRate = totalTests > 0 ? (double)passedTests / totalTests * 100 : 0;
            
            html.AppendLine("        <div class=\"summary\">");
            html.AppendLine("            <h2>📊 Test Summary</h2>");
            html.AppendLine("            <div class=\"summary-grid\">");
            html.AppendLine($"                <div class=\"summary-item total\">");
            html.AppendLine($"                    <div class=\"summary-number\">{totalTests}</div>");
            html.AppendLine("                    <div class=\"summary-label\">Total Tests</div>");
            html.AppendLine("                </div>");
            html.AppendLine($"                <div class=\"summary-item passed\">");
            html.AppendLine($"                    <div class=\"summary-number\">{passedTests}</div>");
            html.AppendLine("                    <div class=\"summary-label\">Passed</div>");
            html.AppendLine("                </div>");
            html.AppendLine($"                <div class=\"summary-item failed\">");
            html.AppendLine($"                    <div class=\"summary-number\">{failedTests}</div>");
            html.AppendLine("                    <div class=\"summary-label\">Failed</div>");
            html.AppendLine("                </div>");
            html.AppendLine($"                <div class=\"summary-item time\">");
            html.AppendLine($"                    <div class=\"summary-number\">{successRate:F1}%</div>");
            html.AppendLine("                    <div class=\"summary-label\">Success Rate</div>");
            html.AppendLine("                </div>");
            html.AppendLine("            </div>");
            html.AppendLine($"            <div class=\"timestamp\">");
            html.AppendLine($"                <strong>Total Execution Time:</strong> {totalDuration.TotalSeconds:F2} seconds | ");
            html.AppendLine($"                <strong>Average Time per Test:</strong> {totalDuration.TotalSeconds / totalTests:F2} seconds | ");
            html.AppendLine($"                <strong>Report Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            
            // Test Results
            html.AppendLine("        <div class=\"test-results\">");
            html.AppendLine("            <h2>🧪 Test Results</h2>");
            
            foreach (var test in testResults)
            {
                var statusClass = test.Passed ? "passed" : "failed";
                var statusText = test.Passed ? "PASSED" : "FAILED";
                var statusIcon = test.Passed ? "✅" : "❌";
                
                html.AppendLine($"            <div class=\"test-item\">");
                html.AppendLine($"                <div class=\"test-header {statusClass}\">");
                html.AppendLine($"                    <div class=\"test-title\">{statusIcon} {test.TestName}</div>");
                html.AppendLine($"                    <div class=\"test-status {statusClass}\">{statusText}</div>");
                html.AppendLine("                </div>");
                html.AppendLine("                <div class=\"test-details\">");
                
                // Test Information
                html.AppendLine("                    <div class=\"test-info\">");
                html.AppendLine("                        <div class=\"info-item\">");
                html.AppendLine("                            <div class=\"info-label\">Status</div>");
                html.AppendLine($"                            <div class=\"info-value\">{statusText}</div>");
                html.AppendLine("                        </div>");
                html.AppendLine("                        <div class=\"info-item\">");
                html.AppendLine("                            <div class=\"info-label\">Duration</div>");
                html.AppendLine($"                            <div class=\"info-value\">{test.Duration.TotalSeconds:F2}s</div>");
                html.AppendLine("                        </div>");
                html.AppendLine("                        <div class=\"info-item\">");
                html.AppendLine("                            <div class=\"info-label\">Started</div>");
                html.AppendLine($"                            <div class=\"info-value\">{test.ExecutionTime:HH:mm:ss}</div>");
                html.AppendLine("                        </div>");
                html.AppendLine("                        <div class=\"info-item\">");
                html.AppendLine("                            <div class=\"info-label\">Message</div>");
                html.AppendLine($"                            <div class=\"info-value\">{test.Message}</div>");
                html.AppendLine("                        </div>");
                html.AppendLine("                    </div>");
                
                // Screenshot
                if (!string.IsNullOrEmpty(test.ScreenshotPath) && File.Exists(test.ScreenshotPath))
                {
                    html.AppendLine("                    <div class=\"test-screenshot\">");
                    html.AppendLine("                        <h4>📸 Test Screenshot</h4>");
                    html.AppendLine($"                        <img src=\"{test.ScreenshotPath}\" alt=\"{test.TestName} Screenshot\" />");
                    html.AppendLine("                    </div>");
                }
                
                // Test Logs
                if (test.Logs != null && test.Logs.Count > 0)
                {
                    html.AppendLine("                    <div class=\"test-logs\">");
                    html.AppendLine("                        <h4>📝 Test Logs</h4>");
                    foreach (var log in test.Logs)
                    {
                        html.AppendLine($"                        <div class=\"log-entry\">{log}</div>");
                    }
                    html.AppendLine("                    </div>");
                }
                
                html.AppendLine("                </div>");
                html.AppendLine("            </div>");
            }
            
            html.AppendLine("        </div>");
            
            // Footer
            html.AppendLine("        <div class=\"footer\">");
            html.AppendLine("            <p>Generated by Playwright C# Test Runner</p>");
            html.AppendLine("            <p>Test automation for Google UK functionality</p>");
            html.AppendLine("        </div>");
            
            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            
            return html.ToString();
        }
        
        public static void SaveHtmlReport(List<TestResult> testResults, string outputPath = "test-report.html")
        {
            try
            {
                var htmlContent = GenerateHtmlReport(testResults);
                File.WriteAllText(outputPath, htmlContent);
                Console.WriteLine($"📄 HTML test report saved to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving HTML report: {ex.Message}");
            }
        }
    }
}
