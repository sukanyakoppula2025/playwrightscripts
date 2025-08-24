# 🚀 Azure DevOps CI/CD Integration Guide

This guide will help you integrate your Playwright tests with Azure DevOps CI/CD pipeline.

## 📋 **Prerequisites**

- ✅ Azure DevOps organization and project
- ✅ Repository with your Playwright tests
- ✅ .NET 8.0 SDK installed on build agents
- ✅ Access to create and manage pipelines

## 🔧 **Setup Steps**

### 1. **Repository Structure**
Ensure your repository has this structure:
```
your-repo/
├── Tests/
│   ├── GoogleSearchTestRunner.cs
│   └── TestReportGenerator.cs
├── Program.cs
├── GoogleSearchTests.csproj
├── azure-pipelines.yml (or azure-pipelines-simple.yml)
├── run-tests-ci.ps1
└── README.md
```

### 2. **Choose Your Pipeline**

#### **Option A: Full Pipeline** (`azure-pipelines.yml`)
- Includes deployment stage
- More comprehensive artifact publishing
- Better for production environments

#### **Option B: Simple Pipeline** (`azure-pipelines-simple.yml`)
- Focused only on testing
- Faster execution
- Better for development/testing environments

### 3. **Create Pipeline in Azure DevOps**

1. **Navigate to Pipelines**
   - Go to your Azure DevOps project
   - Click on "Pipelines" in the left sidebar
   - Click "New Pipeline"

2. **Choose Repository**
   - Select "Azure Repos Git" (or your source)
   - Choose your repository

3. **Configure Pipeline**
   - Select "Existing Azure Pipelines YAML file"
   - Choose `azure-pipelines.yml` or `azure-pipelines-simple.yml`
   - Click "Continue"

4. **Review and Run**
   - Review the pipeline configuration
   - Click "Run" to test the pipeline

### 4. **Pipeline Configuration Options**

#### **Triggers**
```yaml
trigger:
  - main          # Runs on main branch commits
  - develop       # Runs on develop branch commits
  - feature/*     # Runs on any feature branch
```

#### **Pull Request Validation**
```yaml
pr:
  - main          # Validates PRs to main
  - develop       # Validates PRs to develop
```

#### **Build Agent**
```yaml
pool:
  vmImage: 'windows-latest'  # Uses Windows build agent
```

### 5. **Environment Variables**

The pipeline automatically sets these environment variables:
- `PLAYWRIGHT_HEADLESS=true` - Runs tests in headless mode
- `PLAYWRIGHT_BROWSERS_PATH` - Specifies browser installation path

## 🧪 **Running Tests in CI/CD**

### **Automatic Execution**
Tests run automatically when:
- Code is pushed to configured branches
- Pull requests are created/updated
- Pipeline is manually triggered

### **Test Execution Flow**
1. **Setup Environment**
   - Install .NET 8.0
   - Install Playwright CLI
   - Install browser binaries

2. **Build & Test**
   - Restore NuGet packages
   - Build solution
   - Run Playwright tests
   - Generate HTML report

3. **Publish Artifacts**
   - Test results (TRX format)
   - HTML test report
   - Screenshots
   - Test logs

## 📊 **Pipeline Artifacts**

### **Test Results**
- **Format**: TRX (Visual Studio Test Results)
- **Location**: `TestResults` artifact
- **Usage**: View in Azure DevOps Test tab

### **HTML Report**
- **Format**: HTML with embedded screenshots
- **Location**: `TestReport` artifact
- **Usage**: Download and view in browser

### **Screenshots**
- **Format**: PNG files
- **Location**: `Screenshots` artifact
- **Usage**: Visual verification of test execution

## 🔍 **Monitoring & Troubleshooting**

### **Pipeline Status**
- **Green**: All tests passed
- **Red**: Tests failed
- **Yellow**: Tests partially passed

### **Common Issues**

#### **Build Failures**
```bash
# Check .NET version
dotnet --version

# Verify Playwright installation
playwright --version

# Check browser installation
playwright install --help
```

#### **Test Failures**
- Review test logs in pipeline output
- Check screenshots for visual issues
- Verify test environment variables

### **Logs & Debugging**
- Pipeline logs show detailed execution steps
- Test output includes console logs
- Screenshots capture test state

## 🚀 **Advanced Configuration**

### **Custom Build Agents**
```yaml
pool:
  name: 'CustomAgentPool'  # Use custom build agents
  demands:
    - agent.name -equals MyCustomAgent
```

### **Parallel Test Execution**
```yaml
strategy:
  parallel: 2  # Run tests in parallel
```

### **Conditional Execution**
```yaml
condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
```

### **Environment-Specific Settings**
```yaml
variables:
  ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
    environment: 'Production'
  ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/develop') }}:
    environment: 'Staging'
```

## 📱 **Local CI/CD Testing**

### **PowerShell Script**
Use the included `run-tests-ci.ps1` script:
```powershell
# Run tests in CI/CD mode
.\run-tests-ci.ps1

# Run with custom options
.\run-tests-ci.ps1 -Headless -GenerateReport -OutputPath "custom-report.html"
```

### **Environment Variables**
```powershell
# Set headless mode
$env:PLAYWRIGHT_HEADLESS = "true"

# Run tests
dotnet run --configuration Release
```

## 🔐 **Security Considerations**

### **Secrets Management**
- Use Azure Key Vault for sensitive data
- Store API keys as pipeline variables
- Never commit secrets to source code

### **Access Control**
- Limit pipeline access to authorized users
- Use branch policies for main branches
- Require PR reviews before merging

## 📈 **Performance Optimization**

### **Caching**
- NuGet package caching
- Playwright browser caching
- Build artifact caching

### **Parallel Execution**
- Run tests in parallel
- Use multiple build agents
- Optimize test execution order

## 🎯 **Best Practices**

1. **Keep Tests Fast**
   - Minimize test dependencies
   - Use efficient selectors
   - Avoid unnecessary waits

2. **Reliable Tests**
   - Handle dynamic elements
   - Implement retry logic
   - Use stable selectors

3. **Clear Reporting**
   - Generate detailed logs
   - Include screenshots
   - Provide actionable error messages

4. **Regular Maintenance**
   - Update Playwright version
   - Review test stability
   - Monitor execution times

## 🆘 **Support & Resources**

### **Azure DevOps Documentation**
- [Azure Pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/)
- [YAML Schema](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/)

### **Playwright Documentation**
- [Playwright C#](https://playwright.dev/dotnet/)
- [CI/CD Integration](https://playwright.dev/dotnet/docs/ci)

### **Community Support**
- [Azure DevOps Community](https://developercommunity.visualstudio.com/spaces/21/index.html)
- [Playwright GitHub](https://github.com/microsoft/playwright)

---

## 🎉 **Next Steps**

1. **Choose your pipeline file** (full or simple)
2. **Create the pipeline** in Azure DevOps
3. **Test the integration** with a small change
4. **Monitor execution** and review results
5. **Customize** based on your needs

Your Playwright tests are now ready for professional CI/CD integration! 🚀✨
