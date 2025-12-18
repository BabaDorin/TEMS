# End-to-End Tests

This directory contains end-to-end (E2E) tests for the TEMS application using [Playwright](https://playwright.dev/).

## Overview

E2E tests verify the complete integration between frontend and backend, including:
- Real API calls to backend services (port 5158)
- Real authentication with Identity Server (port 5001)
- Complete user workflows from UI interaction to data persistence
- Validation of frontend-backend communication

## Structure

```
e2e/
├── fixtures/           # Test fixtures and custom test setup
├── helpers/           # Utility functions and configuration
├── ticket-management/ # Ticket management feature tests
├── equipment-management/ # Equipment management feature tests
└── README.md
```

## Prerequisites

1. **Backend must be running** on `http://localhost:5158`
2. **Identity Server must be running** on `http://localhost:5001`
3. **Frontend dev server** will start automatically at `http://localhost:4200`

## Running Tests

### Install Dependencies
```bash
npm install
npx playwright install chromium
```

### Run All E2E Tests
```bash
npm run e2e
```

### Run Tests in Headed Mode (See Browser)
```bash
npm run e2e:headed
```

### Run Tests in Debug Mode
```bash
npm run e2e:debug
```

### Run Tests with UI Mode (Interactive)
```bash
npm run e2e:ui
```

### Run Specific Test Suites
```bash
# Ticket Types tests only
npm run e2e:ticket-types

# Tickets tests only
npm run e2e:tickets
```

### View Test Report
```bash
npm run e2e:report
```

## Test Structure

### Ticket Management Tests

**ticket-types.spec.ts**
- ✅ Load ticket types page
- ✅ Fetch and display ticket types from backend
- ✅ Create new ticket type via UI
- ✅ Update existing ticket type
- ✅ Delete ticket type
- ✅ Validate form fields
- ✅ Handle backend errors

**tickets.spec.ts**
- ✅ Load tickets page
- ✅ Fetch and display tickets from backend
- ✅ Create new ticket via UI
- ✅ Update existing ticket
- ✅ Delete ticket
- ✅ Validate form fields
- ✅ Display ticket details

## Writing New Tests

### Example Test Structure

```typescript
import { test, expect } from '../fixtures/test-fixtures';
import { API_CONFIG } from '../helpers/test-config';

test.describe('Feature Name', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/your-route');
  });

  test('should do something', async ({ page, testHelper }) => {
    // Use testHelper.ui for UI interactions
    await testHelper.ui.clickButton('button:has-text("Click Me")');
    
    // Use testHelper.api for direct API calls
    const data = await testHelper.api.get('/api/endpoint');
    
    // Make assertions
    expect(data).toBeDefined();
  });
});
```

### Test Helpers

**testHelper.api**
- `get(endpoint)` - GET request
- `post(endpoint, data)` - POST request
- `put(endpoint, data)` - PUT request
- `delete(endpoint)` - DELETE request
- `checkBackendHealth()` - Check if backend is running

**testHelper.ui**
- `fillField(selector, value)` - Fill form field
- `clickButton(selector)` - Click button
- `waitForElement(selector)` - Wait for element
- `navigateTo(path)` - Navigate to route
- `takeScreenshot(name)` - Capture screenshot

## Configuration

Edit `playwright.config.ts` to customize:
- Test timeout
- Number of workers
- Retry attempts
- Browser settings
- Base URL

Edit `e2e/helpers/test-config.ts` to update:
- API endpoints
- Test data
- UI selectors

## Troubleshooting

### Backend Connection Refused
```
Error: Backend is not running!
```
**Solution:** Start the backend on port 5158
```bash
cd Backend/Tems
dotnet run --project Tems.Host
```

### Tests Timeout
**Solution:** Increase timeout in `playwright.config.ts` or specific test:
```typescript
test('slow test', async ({ page }) => {
  test.setTimeout(60000); // 60 seconds
  // ... test code
});
```

### Element Not Found
**Solution:** Add wait conditions:
```typescript
await page.waitForLoadState('networkidle');
await page.waitForSelector('.my-element', { state: 'visible' });
```

### Flaky Tests
**Solution:** 
1. Add explicit waits: `await page.waitForTimeout(1000)`
2. Wait for API responses: `await page.waitForResponse()`
3. Use retry mechanism in config

## Best Practices

1. **Always clean up test data** in `afterEach` hooks
2. **Use unique identifiers** (timestamps) for test data
3. **Verify both UI and API** - don't trust just the UI
4. **Handle async operations** properly with await
5. **Make tests independent** - don't rely on test execution order
6. **Use meaningful test names** - describe what is being tested
7. **Add console.log** for debugging during development
8. **Take screenshots** on failures (automatic)

## CI/CD Integration

Tests are configured for CI/CD with:
- Automatic retries on failure
- JSON and HTML reports
- Screenshots and videos on failure
- Single worker for deterministic execution

## Resources

- [Playwright Documentation](https://playwright.dev/docs/intro)
- [Playwright Best Practices](https://playwright.dev/docs/best-practices)
- [Playwright API Reference](https://playwright.dev/docs/api/class-playwright)
