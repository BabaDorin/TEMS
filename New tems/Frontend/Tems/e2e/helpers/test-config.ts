/**
 * API Configuration for E2E Tests
 */
export const API_CONFIG = {
  baseURL: 'http://localhost:5158',
  identityServerURL: 'http://localhost:5001',
  frontendURL: 'http://localhost:4200',
  
  endpoints: {
    // Ticket Management
    ticketTypes: '/ticket-types',
    tickets: '/tickets',
    ticketMessages: (ticketId: string) => `/tickets/${ticketId}/messages`,
    
    // Equipment Management
    equipment: '/equipment',
    equipmentTypes: '/equipment-types',
  },
  
  // Default timeout for API calls
  timeout: 10000,
};

/**
 * Test Data Constants
 */
export const TEST_DATA = {
  // Ticket Type Test Data
  ticketType: {
    name: 'E2E Test Ticket Type',
    description: 'Created by E2E tests',
    isActive: true,
  },
  
  // Ticket Test Data
  ticket: {
    title: 'E2E Test Ticket',
    description: 'Created by E2E tests',
    priority: 'Medium',
    status: 'Open',
  },
};

/**
 * Selectors for common UI elements
 */
export const SELECTORS = {
  // Common
  loadingSpinner: '.loading-spinner',
  errorMessage: '.error-message',
  successMessage: '.success-message',
  confirmDialog: '.confirm-dialog',
  
  // Buttons
  createButton: 'button:has-text("Create")',
  saveButton: 'button:has-text("Save")',
  deleteButton: 'button:has-text("Delete")',
  cancelButton: 'button:has-text("Cancel")',
  submitButton: 'button[type="submit"]',
  
  // Forms
  formInput: (name: string) => `input[name="${name}"]`,
  formTextarea: (name: string) => `textarea[name="${name}"]`,
  formSelect: (name: string) => `select[name="${name}"]`,
  formCheckbox: (name: string) => `input[type="checkbox"][name="${name}"]`,
  
  // AG Grid
  agGridRow: '.ag-row',
  agGridCell: '.ag-cell',
  agGridNoRows: '.ag-overlay-no-rows-wrapper',
};
