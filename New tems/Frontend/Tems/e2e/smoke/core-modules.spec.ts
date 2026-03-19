import { test, expect } from '../fixtures/test-fixtures';
import { Page } from '@playwright/test';
import { API_CONFIG } from '../helpers/test-config';

const PERMITTED_STATUSES = [200, 401, 403];

async function expectApiStatus(
  page: Page,
  endpoint: string
) {
  const response = await page.request.get(`${API_CONFIG.baseURL}${endpoint}`, {
    timeout: API_CONFIG.timeout,
    headers: {
      'X-Tenant-Id': 'default',
    },
  });

  expect(PERMITTED_STATUSES).toContain(response.status());
  return response.status();
}

test.describe('Core Modules Smoke Tests', () => {
  test('should load assets module and render base UI', async ({ page }) => {
    await page.goto('/assets/view');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('h1')).toContainText('Assets');
    await expect(page.locator('button:has-text("Add New")')).toBeVisible();
    await expect(page.locator('.ag-theme-quartz, .ag-theme-quartz-dark')).toBeVisible();

    const status = await expectApiStatus(page, API_CONFIG.endpoints.assets);
    console.log(`Assets API status: ${status}`);
  });

  test('should load locations module and render base UI', async ({ page }) => {
    await page.goto('/locations/view');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('h1')).toContainText('Locations');
    await expect(page.locator('button:has-text("Add Room")')).toBeVisible();
    await expect(page.locator('.ag-theme-quartz, .ag-theme-quartz-dark')).toBeVisible();

    const status = await expectApiStatus(page, API_CONFIG.endpoints.rooms);
    console.log(`Rooms API status: ${status}`);
  });

  test('should load user management module and validate users/roles endpoints', async ({ page }) => {
    await page.goto('/administration/users');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('h1')).toContainText('User Management');
    await expect(page.locator('button:has-text("Add User")')).toBeVisible();
    await expect(page.locator('.ag-theme-quartz, .ag-theme-quartz-dark')).toBeVisible();

    const usersStatus = await expectApiStatus(page, API_CONFIG.endpoints.users);
    const rolesStatus = await expectApiStatus(page, API_CONFIG.endpoints.roles);
    console.log(`Users API status: ${usersStatus}, Roles API status: ${rolesStatus}`);
  });

  test('should verify ticketing routes remain available', async ({ page, testHelper }) => {
    await page.goto('/technical-support/ticket-types');
    await page.waitForLoadState('networkidle');
    await expect(page.locator('h1')).toContainText('Ticket Types');

    const ticketTypesResponse = await testHelper.api.request('GET', API_CONFIG.endpoints.ticketTypes);
    expect(PERMITTED_STATUSES).toContain(ticketTypesResponse.status());

    await page.goto('/technical-support/tickets');
    await page.waitForLoadState('networkidle');
    await expect(page.locator('h1')).toContainText('Tickets');

    const ticketsResponse = await testHelper.api.request('GET', API_CONFIG.endpoints.tickets);
    expect(PERMITTED_STATUSES).toContain(ticketsResponse.status());
  });
});
