using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleSearchTests
{
    public class SimpleHtmlReportGenerator
    {
        public static async Task GenerateReportAsync(TestResult result, string outputPath = null)
        {
            try
            {
                // Create reports directory if it doesn't exist
                var reportsDir = "reports";
                if (!Directory.Exists(reportsDir))
                {
                    Directory.CreateDirectory(reportsDir);
                }

                // Generate default filename if none provided
                if (string.IsNullOrEmpty(outputPath))
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var testName = result.TestName.Replace(" ", "_").Replace("&", "and");
                    outputPath = $"reports/{testName}_Report_{timestamp}.html";
                }

                var htmlContent = GenerateHtmlContent(result);
                await File.WriteAllTextAsync(outputPath, htmlContent);
                
                Console.WriteLine($"✅ HTML report generated successfully: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to generate HTML report: {ex.Message}");
            }
        }

        private static string GenerateHtmlContent(TestResult result)
        {
            var currentTime = DateTime.Now;
            var testDuration = result.Duration.TotalSeconds;
            var statusClass = result.Passed ? "passed" : "failed";
            var statusIcon = result.Passed ? "✅" : "❌";
            var statusText = result.Passed ? "PASSED" : "FAILED";
            
            return $@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{result.TestName} - Test Report</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 20px;
            background-color: #f5f5f5;
            line-height: 1.6;
        }}
        .container {{
            max-width: 1000px;
            margin: 0 auto;
            background-color: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            border-bottom: 3px solid #007bff;
            padding-bottom: 20px;
            margin-bottom: 30px;
        }}
        .header h1 {{
            color: #007bff;
            margin: 0;
            font-size: 2.5em;
        }}
        .status-banner {{
            padding: 15px;
            border-radius: 8px;
            margin: 20px 0;
            text-align: center;
            font-size: 1.3em;
            font-weight: bold;
            color: white;
        }}
        .status-banner.passed {{
            background: linear-gradient(135deg, #28a745, #20c997);
        }}
        .status-banner.failed {{
            background: linear-gradient(135deg, #dc3545, #fd7e14);
        }}
        .info-grid {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 25px;
            margin: 25px 0;
        }}
        .info-card {{
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 8px;
            border-left: 4px solid #007bff;
        }}
        .info-card h3 {{
            margin-top: 0;
            color: #495057;
            border-bottom: 2px solid #dee2e6;
            padding-bottom: 10px;
        }}
        .info-item {{
            margin: 10px 0;
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
        }}
        .info-item:last-child {{
            border-bottom: none;
        }}
        .info-label {{
            font-weight: bold;
            color: #495057;
            display: inline-block;
            width: 140px;
        }}
        .info-value {{
            color: #6c757d;
        }}
        .logs-section {{
            background-color: #f8f9fa;
            padding: 25px;
            border-radius: 8px;
            margin: 25px 0;
        }}
        .logs-section h3 {{
            margin-top: 0;
            color: #495057;
            border-bottom: 2px solid #dee2e6;
            padding-bottom: 10px;
        }}
        .log-entry {{
            background-color: white;
            margin: 8px 0;
            padding: 12px 15px;
            border-radius: 6px;
            border-left: 4px solid #007bff;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
            box-shadow: 0 2px 5px rgba(0,0,0,0.05);
        }}
        .summary {{
            background: linear-gradient(135deg, #f8f9fa, #e9ecef);
            padding: 25px;
            border-radius: 8px;
            text-align: center;
            margin: 25px 0;
        }}
        .summary h3 {{
            color: #495057;
            margin-bottom: 20px;
            font-size: 1.4em;
        }}
        .summary-stats {{
            display: flex;
            justify-content: space-around;
            flex-wrap: wrap;
            gap: 20px;
        }}
        .stat-item {{
            text-align: center;
        }}
        .stat-number {{
            font-size: 2em;
            font-weight: bold;
            color: #007bff;
        }}
        .stat-label {{
            color: #6c757d;
            font-size: 0.9em;
            text-transform: uppercase;
            letter-spacing: 1px;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #dee2e6;
            color: #6c757d;
        }}
        @media (max-width: 768px) {{
            .info-grid {{
                grid-template-columns: 1fr;
            }}
            .summary-stats {{
                flex-direction: column;
            }}
            .container {{
                padding: 20px;
                margin: 10px;
            }}
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🧪 {result.TestName}</h1>
            <p>Automated Test Execution Report</p>
        </div>
        
        <div class='status-banner {statusClass}'>
            {statusIcon} Test Status: {statusText}
        </div>
        
        <div class='summary'>
            <h3>📊 Test Summary</h3>
            <div class='summary-stats'>
                <div class='stat-item'>
                    <div class='stat-number'>{statusIcon}</div>
                    <div class='stat-label'>Status</div>
                </div>
                <div class='stat-item'>
                    <div class='stat-number'>{testDuration:F1}s</div>
                    <div class='stat-label'>Duration</div>
                </div>
                <div class='stat-item'>
                    <div class='stat-number'>{result.Logs.Count}</div>
                    <div class='stat-label'>Log Entries</div>
                </div>
                <div class='stat-item'>
                    <div class='stat-number'>{result.ExecutionTime:HH:mm}</div>
                    <div class='stat-label'>Start Time</div>
                </div>
            </div>
        </div>
        
        <div class='info-grid'>
            <div class='info-card'>
                <h3>📋 Test Information</h3>
                <div class='info-item'>
                    <span class='info-label'>Test Name:</span>
                    <span class='info-value'>{result.TestName}</span>
                </div>
                <div class='info-item'>
                    <span class='info-label'>Execution Time:</span>
                    <span class='info-value'>{result.ExecutionTime:yyyy-MM-dd HH:mm:ss}</span>
                </div>
                <div class='info-item'>
                    <span class='info-label'>Duration:</span>
                    <span class='info-value'>{result.Duration.TotalSeconds:F2} seconds</span>
                </div>
                <div class='info-item'>
                    <span class='info-label'>Status:</span>
                    <span class='info-value'><strong>{statusText}</strong></span>
                </div>
            </div>
            
            <div class='info-card'>
                <h3>📝 Test Details</h3>
                <div class='info-item'>
                    <span class='info-label'>Message:</span>
                    <span class='info-value'>{result.Message}</span>
                </div>
                <div class='info-item'>
                    <span class='info-label'>Log Count:</span>
                    <span class='info-value'>{result.Logs.Count} entries</span>
                </div>
                <div class='info-item'>
                    <span class='info-label'>Report Generated:</span>
                    <span class='info-value'>{currentTime:yyyy-MM-dd HH:mm:ss}</span>
                </div>
            </div>
        </div>
        
        <div class='logs-section'>
            <h3>📝 Test Execution Logs</h3>
            {string.Join("", result.Logs.Select((log, index) => 
                $"<div class='log-entry'><strong>[{index + 1:D3}]</strong> {log}</div>"))}
        </div>
        
        <div class='footer'>
            <p><strong>Report generated by Playwright C# automation framework</strong></p>
            <p>Generated on {currentTime:yyyy-MM-dd HH:mm:ss}</p>
            <p>This is an automated test report. For questions, please refer to your test automation team.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
