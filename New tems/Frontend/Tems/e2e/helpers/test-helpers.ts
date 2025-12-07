import { Page, expect } from '@playwright/test';
import { API_CONFIG } from './test-config';

/**
 * API Helper for making direct API calls in tests
 * Useful for setup/teardown and verification
 */
export class ApiHelper {
  constructor(private page: Page) {}

  /**
   * Make a GET request to the API
   */
  async get(endpoint: string): Promise<any> {
    const response = await this.page.request.get(`${API_CONFIG.baseURL}${endpoint}`, {
      timeout: API_CONFIG.timeout,
      headers: {
        'X-Tenant-Id': 'default',
      },
    });
    
    expect(response.ok()).toBeTruthy();
    return await response.json();
  }

  /**
   * Make a POST request to the API
   */
  async post(endpoint: string, data: any): Promise<any> {
    const response = await this.page.request.post(`${API_CONFIG.baseURL}${endpoint}`, {
      data,
      timeout: API_CONFIG.timeout,
      headers: {
        'Content-Type': 'application/json',
        'X-Tenant-Id': 'default',
      },
    });
    
    expect(response.ok()).toBeTruthy();
    return await response.json();
  }

  /**
   * Make a PUT request to the API
   */
  async put(endpoint: string, data: any): Promise<any> {
    const response = await this.page.request.put(`${API_CONFIG.baseURL}${endpoint}`, {
      data,
      timeout: API_CONFIG.timeout,
      headers: {
        'Content-Type': 'application/json',
        'X-Tenant-Id': 'default',
      },
    });
    
    expect(response.ok()).toBeTruthy();
    return await response.json();
  }

  /**
   * Make a DELETE request to the API
   */
  async delete(endpoint: string): Promise<void> {
    const response = await this.page.request.delete(`${API_CONFIG.baseURL}${endpoint}`, {
      timeout: API_CONFIG.timeout,
      headers: {
        'X-Tenant-Id': 'default',
      },
    });
    
    expect(response.ok()).toBeTruthy();
  }

  /**
   * Wait for a network request to complete
   */
  async waitForRequest(urlPattern: string | RegExp): Promise<any> {
    const response = await this.page.waitForResponse(
      (response) => {
        const url = response.url();
        if (typeof urlPattern === 'string') {
          return url.includes(urlPattern);
        }
        return urlPattern.test(url);
      },
      { timeout: API_CONFIG.timeout }
    );
    
    return await response.json();
  }

  /**
   * Check if backend is running
   */
  async checkBackendHealth(): Promise<boolean> {
    try {
      // Try to call a real endpoint instead of /health
      const response = await this.page.request.get(`${API_CONFIG.baseURL}/ticket-types`, {
        timeout: 5000,
      });
      return response.ok();
    } catch (error) {
      return false;
    }
  }
}

/**
 * UI Helper for common UI interactions
 */
export class UiHelper {
  constructor(private page: Page) {}

  /**
   * Fill a form field
   */
  async fillField(selector: string, value: string): Promise<void> {
    await this.page.fill(selector, value);
  }

  /**
   * Click a button and wait for navigation if needed
   */
  async clickButton(selector: string, waitForNavigation = false): Promise<void> {
    if (waitForNavigation) {
      await Promise.all([
        this.page.waitForNavigation(),
        this.page.click(selector),
      ]);
    } else {
      await this.page.click(selector);
    }
  }

  /**
   * Wait for element to be visible
   */
  async waitForElement(selector: string, timeout = 5000): Promise<void> {
    await this.page.waitForSelector(selector, { state: 'visible', timeout });
  }

  /**
   * Wait for loading to complete
   */
  async waitForLoading(): Promise<void> {
    // Wait for any loading spinners to disappear
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Get text content from element
   */
  async getTextContent(selector: string): Promise<string | null> {
    return await this.page.textContent(selector);
  }

  /**
   * Check if element exists
   */
  async elementExists(selector: string): Promise<boolean> {
    return (await this.page.$(selector)) !== null;
  }

  /**
   * Count elements matching selector
   */
  async countElements(selector: string): Promise<number> {
    return await this.page.locator(selector).count();
  }

  /**
   * Navigate to a route
   */
  async navigateTo(path: string): Promise<void> {
    await this.page.goto(`${API_CONFIG.frontendURL}${path}`);
    await this.waitForLoading();
  }

  /**
   * Take a screenshot with a descriptive name
   */
  async takeScreenshot(name: string): Promise<void> {
    await this.page.screenshot({ path: `test-results/screenshots/${name}.png`, fullPage: true });
  }
}

/**
 * Combined helper that includes both API and UI helpers
 */
export class TestHelper {
  public api: ApiHelper;
  public ui: UiHelper;

  constructor(page: Page) {
    this.api = new ApiHelper(page);
    this.ui = new UiHelper(page);
  }
}
