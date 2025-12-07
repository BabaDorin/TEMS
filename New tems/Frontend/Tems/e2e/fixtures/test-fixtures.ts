import { test as base, Page } from '@playwright/test';
import { TestHelper } from '../helpers/test-helpers';

/**
 * Extended test fixture with common helpers
 */
type TestFixtures = {
  testHelper: TestHelper;
};

/**
 * Extend base test with custom fixtures
 */
export const test = base.extend<TestFixtures>({
  page: async ({ page }, use) => {
    // Remove webpack dev server overlay if it exists
    await page.addInitScript(() => {
      const style = document.createElement('style');
      style.textContent = '#webpack-dev-server-client-overlay { display: none !important; }';
      document.head.appendChild(style);
    });
    
    await use(page);
  },
  
  testHelper: async ({ page }, use) => {
    const helper = new TestHelper(page);
    
    // Check if backend is running before each test
    const isBackendRunning = await helper.api.checkBackendHealth();
    if (!isBackendRunning) {
      throw new Error(
        'Backend is not running! Please start the backend on port 5158 before running E2E tests.'
      );
    }
    
    await use(helper);
  },
});

export { expect } from '@playwright/test';
