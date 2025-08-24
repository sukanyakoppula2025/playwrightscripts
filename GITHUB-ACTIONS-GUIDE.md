# 🚀 GitHub Actions with Playwright Tests

This guide will help you set up and run your Playwright tests using GitHub Actions CI/CD.

## 📁 **Workflow Files Created**

### 1. **Basic Workflow** (`.github/workflows/playwright-tests.yml`)
- Simple single-browser testing
- Perfect for getting started
- Fast execution

### 2. **Matrix Workflow** (`.github/workflows/playwright-matrix.yml`)
- Tests across multiple browsers (Chrome, Firefox, Safari)
- Comprehensive cross-browser testing
- Advanced reporting and PR comments

## 🚀 **Quick Setup (3 Steps)**

### **Step 1: Push Workflow Files**
The workflow files are already created and ready to use. Just push them to GitHub:

```bash
git add .github/workflows/
git commit -m "Add GitHub Actions workflows for Playwright tests"
git push origin main
```

### **Step 2: View Actions Tab**
1. Go to your GitHub repository
2. Click on the **Actions** tab
3. You'll see your workflows listed

### **Step 3: Run Tests**
- **Automatic**: Tests run on every push/PR
- **Manual**: Click "Run workflow" button

## 🔧 **Workflow Features**

### **Triggers**
```yaml
on:
  push:
    branches: [ main, develop ]          # Runs on code pushes
  pull_request:
    branches: [ main, develop ]          # Runs on PRs
  workflow_dispatch:                     # Manual trigger
```

### **Environment**
- **Runner**: `windows-latest` (Windows build agent)
- **.NET**: 8.0.x SDK
- **Playwright**: Latest version with all browsers

### **Test Execution**
- **Headless mode** for CI/CD
- **Screenshot capture** on success/failure
- **Video recording** of test runs
- **HTML report generation**

## 📊 **What You Get**

### **Artifacts Published**
- 📄 **HTML Test Reports** - Beautiful reports with screenshots
- 📸 **Screenshots** - Visual evidence of test execution
- 🎥 **Test Videos** - Recordings for debugging
- 📋 **Test Logs** - Detailed execution logs

### **Pull Request Integration**
- ✅ **Automatic comments** with test results
- 📊 **Status checks** for quality gates
- 🔍 **Easy access** to test artifacts

### **Cross-Browser Testing** (Matrix Workflow)
- 🌐 **Chrome** - Chromium-based browser
- 🦊 **Firefox** - Mozilla browser
- 🍎 **Safari** - WebKit browser (macOS)

## 🎯 **Workflow Comparison**

| Feature | Basic Workflow | Matrix Workflow |
|---------|----------------|-----------------|
| **Speed** | ⚡ Fast | 🐌 Slower (3x browsers) |
| **Coverage** | 🎯 Single browser | 🌐 All browsers |
| **Complexity** | 🟢 Simple | 🟡 Advanced |
| **Use Case** | 🚀 Quick feedback | 🧪 Production quality |

## 🚀 **Running Tests**

### **Automatic Execution**
Tests run automatically when:
- ✅ **Code pushed** to main/develop branches
- ✅ **Pull requests** created/updated
- ✅ **Manual trigger** via Actions tab

### **Manual Execution**
1. Go to **Actions** tab
2. Select your workflow
3. Click **Run workflow**
4. Choose options (for matrix workflow)
5. Click **Run workflow**

### **Test Results**
- 🟢 **Green checkmark** = All tests passed
- 🔴 **Red X** = Tests failed
- 🟡 **Yellow dot** = Tests in progress

## 📱 **Local Testing**

### **Test CI/CD Mode Locally**
```bash
# Use the batch file
.\run-tests-ci.bat

# Or PowerShell
powershell -ExecutionPolicy Bypass -File "run-tests-ci.ps1"
```

### **Environment Variables**
```bash
# Set headless mode
set PLAYWRIGHT_HEADLESS=true

# Run tests
dotnet run --configuration Release
```

## 🔍 **Monitoring & Debugging**

### **Viewing Results**
1. **Actions Tab** - Overall workflow status
2. **Workflow Runs** - Individual execution details
3. **Job Logs** - Step-by-step execution
4. **Artifacts** - Download test reports and screenshots

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
- Review test logs in Actions output
- Check screenshots for visual issues
- Verify test environment variables
- Download artifacts for analysis

### **Debugging Tips**
- **Check logs** for error messages
- **Review screenshots** for visual issues
- **Download artifacts** for local analysis
- **Use manual triggers** for testing

## 🚀 **Advanced Features**

### **Matrix Testing**
```yaml
strategy:
  fail-fast: false
  matrix:
    browser: [chrome, firefox, webkit]
```

### **Conditional Steps**
```yaml
- name: Comment PR
  if: github.event_name == 'pull_request'
  # Only runs on PRs
```

### **Artifact Management**
```yaml
- name: Upload results
  uses: actions/upload-artifact@v4
  if: always()  # Upload even if tests fail
```

### **Environment-Specific Settings**
```yaml
env:
  PLAYWRIGHT_HEADLESS: true
  PLAYWRIGHT_BROWSER: ${{ matrix.browser }}
```

## 📈 **Performance Optimization**

### **Caching Strategies**
- **NuGet packages** - Restore dependencies faster
- **Playwright browsers** - Avoid re-downloading
- **Build artifacts** - Reuse previous builds

### **Parallel Execution**
- **Matrix testing** - Run browsers in parallel
- **Job dependencies** - Optimize workflow order
- **Resource allocation** - Use appropriate runners

## 🔐 **Security Considerations**

### **Secrets Management**
- Use **GitHub Secrets** for sensitive data
- Store **API keys** as repository secrets
- Never commit **secrets** to source code

### **Access Control**
- **Branch protection** rules
- **Required status checks** for PRs
- **Code review** requirements

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

### **GitHub Actions Documentation**
- [GitHub Actions](https://docs.github.com/en/actions)
- [Workflow Syntax](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)

### **Playwright Documentation**
- [Playwright C#](https://playwright.dev/dotnet/)
- [CI/CD Integration](https://playwright.dev/dotnet/docs/ci)

### **Community Support**
- [GitHub Community](https://github.com/orgs/community/discussions)
- [Playwright GitHub](https://github.com/microsoft/playwright)

---

## 🎉 **Next Steps**

1. **Push workflow files** to GitHub
2. **Check Actions tab** for workflow status
3. **Make a test commit** to trigger execution
4. **Review results** and artifacts
5. **Customize** based on your needs

## 🎯 **Success Metrics**

Your Playwright tests are now ready for:
- ✅ **Automated testing** on every code change
- ✅ **Quality gates** for pull requests
- ✅ **Cross-browser testing** across Chrome, Firefox, Safari
- ✅ **Professional reporting** with artifacts
- ✅ **Visual verification** with screenshots
- ✅ **Team collaboration** with PR integration

**Welcome to GitHub Actions automation! 🚀✨**
