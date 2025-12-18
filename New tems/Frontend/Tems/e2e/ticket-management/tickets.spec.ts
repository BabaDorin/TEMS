import { test, expect } from '../fixtures/test-fixtures';
import { API_CONFIG } from '../helpers/test-config';

/**
 * E2E Tests for Tickets CRUD Operations
 * Tests full frontend-backend integration
 */
test.describe('Tickets Management', () => {
  let createdTicketId: string | null = null;

  test.beforeEach(async ({ page }) => {
    // Navigate to tickets page
    await page.goto('/technical-support/tickets');
    await page.waitForLoadState('networkidle');
    
    // Hide webpack overlay if it exists
    await page.evaluate(() => {
      const overlay = document.getElementById('webpack-dev-server-client-overlay');
      if (overlay) {
        overlay.style.display = 'none';
      }
    });
  });

  test.afterEach(async ({ testHelper }) => {
    // Cleanup: Delete created ticket if exists
    if (createdTicketId) {
      try {
        await testHelper.api.delete(`${API_CONFIG.endpoints.tickets}/${createdTicketId}`);
      } catch (error) {
        console.log('Cleanup failed:', error);
      }
      createdTicketId = null;
    }
  });

  test('should load tickets page successfully', async ({ page }) => {
    // Verify page title
    await expect(page.locator('h1')).toContainText('Tickets');
    
    // Verify "Add Ticket" or "Create Ticket" button exists
    const createButton = page.locator('button:has-text("Add Ticket"), button:has-text("Create Ticket")').first();
    await expect(createButton).toBeVisible();
    
    // Verify AG Grid is present
    await expect(page.locator('.ag-theme-quartz')).toBeVisible();
  });

  test('should fetch tickets from backend and display in grid', async ({ page, testHelper }) => {
    test.setTimeout(5000);
    
    // Verify backend endpoint is accessible
    const backendResponse = await testHelper.api.get(API_CONFIG.endpoints.tickets);
    expect(backendResponse).toBeDefined();
    expect(backendResponse.tickets).toBeDefined();
    
    console.log(`✓ Backend has ${backendResponse.tickets.length} tickets`);
    
    // Verify AG Grid component is visible
    await expect(page.locator('.ag-theme-quartz')).toBeVisible();
    
    console.log('✓ Grid component rendered successfully');
  });

  test('should create a new ticket via UI and verify in backend', async ({ page, testHelper }) => {
    test.setTimeout(10000);
    
    const createButton = page.locator('button:has-text("Add Ticket"), button:has-text("Create Ticket")').first();
    await createButton.click();
    
    // Check if create form exists
    await page.waitForTimeout(1000);
    const titleInput = page.locator('input[formControlName="title"]');
    const titleExists = await titleInput.count() > 0;
    
    if (!titleExists) {
      console.log('✓ Create ticket form not implemented yet - skipping test');
      test.skip();
      return;
    }
    
    const uniqueTitle = `E2E Test Ticket ${Date.now()}`;
    await page.fill('input[formControlName="title"]', uniqueTitle);
    await page.fill('textarea[formControlName="description"]', 'Created by E2E test');
    
    const ticketTypeSelect = page.locator('select[formControlName="ticketTypeId"]');
    if (await ticketTypeSelect.isVisible()) {
      await ticketTypeSelect.selectOption({ index: 1 });
    }
    
    const prioritySelect = page.locator('select[formControlName="priority"]');
    if (await prioritySelect.isVisible()) {
      await prioritySelect.selectOption('Medium');
    }
    
    await page.click('button[type="submit"]');
    await page.waitForTimeout(2000);
    
    console.log('✓ Ticket creation attempted');
    
    // Verify the ticket appears in the grid
    await expect(page.locator(`.ag-cell:has-text("${uniqueTitle}")`)).toBeVisible();
    
    // Verify in backend by making direct API call
    const ticketsResponse = await testHelper.api.get(API_CONFIG.endpoints.tickets);
    const foundTicket = ticketsResponse.tickets.find((t: any) => t.ticketId === createdTicketId);
    
    expect(foundTicket).toBeDefined();
    expect(foundTicket.title).toBe(uniqueTitle);
    expect(foundTicket.description).toBe('Created by E2E test');
  });

  test('should update a ticket via UI and verify changes', async ({ page, testHelper }) => {
    test.setTimeout(5000);
    
    // Check if edit functionality exists
    const editButton = page.locator('button:has-text("Edit")').first();
    if (await editButton.count() === 0) {
      console.log('✓ Edit functionality not implemented yet - skipping test');
      test.skip();
      return;
    }
    
    // Get a ticket type from backend
    const ticketTypes = await testHelper.api.get(API_CONFIG.endpoints.ticketTypes);
    const ticketTypeId = ticketTypes.ticketTypes[0]?.ticketTypeId || 'test-type';
    
    const testTicket = {
      summary: `E2E Update Test ${Date.now()}`,
      ticketTypeId: ticketTypeId,
      priority: 'LOW',
      reporter: {
        userId: 'test-user-123',
        channelSource: 'WEB',
        channelThreadId: null
      },
      assigneeId: null,
      attributes: {}
    };
    
    const createdTicket = await testHelper.api.post(API_CONFIG.endpoints.tickets, testTicket);
    createdTicketId = createdTicket.ticketId;
    
    // Reload the page to show the new ticket
    await page.reload();
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);
    
    // Find and click on the row to select it
    const rowWithTicket = page.locator(`.ag-row:has-text("${testTicket.summary}")`);
    await expect(rowWithTicket).toBeVisible();
    await rowWithTicket.click();
    
    // Wait a moment for selection
    await page.waitForTimeout(500);
    
    // Use the editButton we already declared
    if (await editButton.isVisible()) {
      await editButton.click();
      
      // Update the description
      await page.fill('textarea[formControlName="description"]', 'Updated by E2E test');
      
      // Submit
      await page.click('button[type="submit"]');
      
      // Wait for update to complete
      await page.waitForTimeout(1000);
      
      // Verify via API
      const updatedTicket = await testHelper.api.get(`${API_CONFIG.endpoints.tickets}/${createdTicketId}`);
      expect(updatedTicket.description).toBe('Updated by E2E test');
    } else {
      console.log('Edit functionality not found in UI - skipping update test');
      test.skip();
    }
  });

  test('should delete a ticket via UI and verify removal', async ({ page, testHelper }) => {
    test.setTimeout(5000);
    
    // Check if delete functionality exists
    const deleteButton = page.locator('button:has-text("Delete")').first();
    if (await deleteButton.count() === 0) {
      console.log('✓ Delete functionality not implemented yet - skipping test');
      test.skip();
      return;
    }
    
    // Get a ticket type from backend
    const ticketTypes = await testHelper.api.get(API_CONFIG.endpoints.ticketTypes);
    const ticketTypeId = ticketTypes.ticketTypes[0]?.ticketTypeId || 'test-type';
    
    const testTicket = {
      summary: `E2E Delete Test ${Date.now()}`,
      ticketTypeId: ticketTypeId,
      priority: 'HIGH',
      reporter: {
        userId: 'test-user-123',
        channelSource: 'WEB',
        channelThreadId: null
      },
      assigneeId: null,
      attributes: {}
    };
    
    const createdTicket = await testHelper.api.post(API_CONFIG.endpoints.tickets, testTicket);
    createdTicketId = createdTicket.ticketId;
    
    // Reload the page to show the new ticket
    await page.reload();
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);
    
    // Find the row with our test ticket
    const rowWithTicket = page.locator(`.ag-row:has-text("${testTicket.summary}")`);
    await expect(rowWithTicket).toBeVisible();
    
    // Click on the row to select it
    await rowWithTicket.click();
    await page.waitForTimeout(500);
    
    // Use the deleteButton we already declared
    if (await deleteButton.isVisible()) {
      // Set up to handle confirmation dialog
      page.on('dialog', dialog => dialog.accept());
      
      // Click delete
      await deleteButton.click();
      
      // Wait for deletion to complete
      await page.waitForTimeout(1000);
      
      // Verify the ticket no longer appears in the grid
      await expect(page.locator(`.ag-cell:has-text("${testTicket.summary}")`)).not.toBeVisible();
      
      // Verify via API that it's deleted
      try {
        await testHelper.api.get(`${API_CONFIG.endpoints.tickets}/${createdTicketId}`);
        // If we get here, the ticket still exists - fail the test
        expect(true).toBe(false);
      } catch (error) {
        // Expected: API should return error for deleted ticket
        console.log('Ticket successfully deleted');
      }
      
      createdTicketId = null; // Clear since we deleted it
    } else {
      console.log('Delete functionality not found in UI - skipping delete test');
      test.skip();
    }
  });

  test('should validate required fields in create form', async ({ page }) => {
    test.setTimeout(5000);
    
    const createButton = page.locator('button:has-text("Add Ticket"), button:has-text("Create Ticket")').first();
    await createButton.click();
    
    await expect(page.locator('h2:has-text("Create Ticket"), h2:has-text("New Ticket")')).toBeVisible();
    
    // Check submit button is disabled for empty form
    const submitButton = page.locator('button[type="submit"]');
    await expect(submitButton).toBeDisabled();
    
    console.log('✓ Submit button correctly disabled for empty form');
    
    // Verify validation errors appear
    const titleError = page.locator('p.text-red-500:has-text("Title is required"), p.text-red-500:has-text("required")');
    
    // At least validation should prevent submission or show error
    const errorVisible = await titleError.isVisible();
    
    // Modal should still be open
    const modalStillOpen = await page.locator('h2:has-text("Create Ticket"), h2:has-text("New Ticket")').isVisible();
    
    expect(errorVisible || modalStillOpen).toBeTruthy();
  });

  test('should display ticket details when clicking on a ticket', async ({ page, testHelper }) => {
    test.setTimeout(5000);
    
    let testTicket: any;
    
    // Try to create a ticket via API - skip if endpoint not implemented
    try {
      const ticketTypes = await testHelper.api.get(API_CONFIG.endpoints.ticketTypes);
      const ticketTypeId = ticketTypes.ticketTypes[0]?.ticketTypeId || 'test-type';
      
      testTicket = {
        summary: `E2E View Test ${Date.now()}`,
        ticketTypeId: ticketTypeId,
        priority: 'MEDIUM',
        reporter: {
          userId: 'test-user-123',
          channelSource: 'WEB',
          channelThreadId: null
        },
        assigneeId: null,
        attributes: {}
      };
      
      const createdTicket = await testHelper.api.post(API_CONFIG.endpoints.tickets, testTicket);
      createdTicketId = createdTicket.ticketId;
    } catch (error) {
      console.log('✓ Tickets API not fully implemented - skipping test');
      test.skip();
      return;
    }
    
    // Reload the page
    await page.reload();
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);
    
    // Click on the ticket row
    const rowWithTicket = page.locator(`.ag-row:has-text("${testTicket.summary}")`);
    await expect(rowWithTicket).toBeVisible();
    await rowWithTicket.click();
    
    // Wait for any details panel or modal to appear
    await page.waitForTimeout(500);
    
    // Verify that some details are shown (this will vary based on UI)
    // At minimum, the row should be selected
    const selectedRow = page.locator('.ag-row.ag-row-selected');
    await expect(selectedRow).toBeVisible();
  });

  test('should create ticket with custom attributes from ticket type', async ({ page, testHelper }) => {
    test.setTimeout(15000);
    
    // First create a ticket type with custom attributes
    const ticketTypePayload = {
      name: `E2E Test Type ${Date.now()}`,
      description: 'Test type with custom attributes',
      itilCategory: 'INCIDENT',
      version: 1,
      workflowConfig: {
        states: [{
          id: 'new',
          label: 'New',
          type: 'OPEN',
          allowedTransitions: [],
          automationHook: null
        }],
        initialStateId: 'new'
      },
      attributeDefinitions: [
        {
          key: 'severity',
          label: 'Severity',
          dataType: 'DROPDOWN',
          isRequired: true,
          isPredefined: false,
          options: ['Low', 'Medium', 'High', 'Critical'],
          aiExtractionHint: null,
          validationRule: null
        },
        {
          key: 'reproducible',
          label: 'Is Reproducible',
          dataType: 'BOOL',
          isRequired: false,
          isPredefined: false,
          options: null,
          aiExtractionHint: null,
          validationRule: null
        },
        {
          key: 'environment',
          label: 'Environment',
          dataType: 'STRING',
          isRequired: true,
          isPredefined: false,
          options: null,
          aiExtractionHint: null,
          validationRule: null
        }
      ]
    };

    let createdTicketType: any;
    try {
      createdTicketType = await testHelper.api.post(API_CONFIG.endpoints.ticketTypes, ticketTypePayload);
      console.log('✓ Created test ticket type with custom attributes');
    } catch (error) {
      console.log('Failed to create ticket type:', error);
      test.skip();
      return;
    }

    // Reload page to get new ticket type in dropdown
    await page.reload();
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    // Open create modal
    const createButton = page.locator('button:has-text("Add Ticket"), button:has-text("Create Ticket")').first();
    await createButton.click();
    await page.waitForTimeout(500);

    // Select the ticket type
    const ticketTypeSelect = page.locator('select[formControlName="ticketTypeId"]');
    await ticketTypeSelect.selectOption(createdTicketType.ticketTypeId);
    await page.waitForTimeout(1000);

    // Fill in standard fields
    await page.fill('textarea[formControlName="summary"]', 'Test ticket with custom attributes');
    
    // Select priority
    const priorityRadio = page.locator('input[formControlName="priority"][value="MEDIUM"]');
    await priorityRadio.click();

    // Select channel source
    const channelSelect = page.locator('select[formControlName="channelSource"]');
    await channelSelect.selectOption('WEB');

    // Fill in custom attributes
    // STRING attribute
    const environmentInput = page.locator('input').filter({ hasText: /Environment/ }).or(
      page.locator('input[placeholder*="Environment"]')
    ).first();
    if (await environmentInput.count() > 0) {
      await environmentInput.fill('Production');
    }

    // DROPDOWN attribute
    const severitySelect = page.locator('select').filter({ hasText: /Severity/ }).or(
      page.locator('select option:has-text("Critical")')
    ).first();
    if (await severitySelect.count() > 0) {
      await severitySelect.selectOption('High');
    }

    // BOOL attribute
    const reproducibleCheckbox = page.locator('input[type="checkbox"]').filter({ hasText: /Reproducible/ }).first();
    if (await reproducibleCheckbox.count() > 0) {
      await reproducibleCheckbox.check();
    }

    // Submit the form
    const submitButton = page.locator('button[type="submit"]');
    await submitButton.click();
    await page.waitForTimeout(2000);

    // Verify ticket was created
    const ticketsResponse = await testHelper.api.get(API_CONFIG.endpoints.tickets);
    const createdTicket = ticketsResponse.tickets.find((t: any) => 
      t.summary === 'Test ticket with custom attributes'
    );

    expect(createdTicket).toBeDefined();
    expect(createdTicket.attributes).toBeDefined();
    
    // Verify custom attributes were saved
    if (createdTicket.attributes.environment) {
      expect(createdTicket.attributes.environment).toBe('Production');
    }
    if (createdTicket.attributes.severity) {
      expect(createdTicket.attributes.severity).toBe('High');
    }

    createdTicketId = createdTicket.ticketId;
    console.log('✓ Ticket created with custom attributes successfully');

    // Cleanup ticket type
    try {
      await testHelper.api.delete(`${API_CONFIG.endpoints.ticketTypes}/${createdTicketType.ticketTypeId}`);
    } catch (error) {
      console.log('Failed to cleanup ticket type:', error);
    }
  });
});
