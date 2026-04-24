import { Component, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { ThemeService } from 'src/app/services/theme.service';
import { TicketType, CreateTicketTypeRequest, AttributeDefinition } from 'src/app/models/ticket/ticket-type.model';
import { TicketManagementStateService } from 'src/app/state/ticket-management.state';
import { AttributeBuilder } from 'src/app/components/ticket-type/attribute-builder/attribute-builder';

@Component({
  selector: 'app-view-ticket-types',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AgGridAngular,
    AttributeBuilder
  ],
  templateUrl: './view-ticket-types.component.html',
  styleUrls: ['./view-ticket-types.component.scss']
})
export class ViewTicketTypesComponent implements OnInit {
  rowData: TicketType[] = [];
  gridApi!: GridApi;
  showCreateModal = false;
  showDetailsModal = false;
  selectedTicketType: TicketType | null = null;
  createForm!: FormGroup;
  isSubmitting = false;
  gridReady = false;
  attributeDefinitions: AttributeDefinition[] = [];

  columnDefs: ColDef[] = [
    {
      headerName: 'Name',
      field: 'name',
      flex: 2,
      minWidth: 200,
      cellClass: 'font-medium'
    },
    {
      headerName: 'Category',
      field: 'itilCategory',
      flex: 1,
      minWidth: 150
    },
    {
      headerName: 'Version',
      field: 'version',
      flex: 0.5,
      minWidth: 80,
      type: 'numericColumn'
    },
    {
      headerName: 'Status',
      field: 'isActive',
      flex: 0.7,
      minWidth: 100,
      cellRenderer: (params: any) => {
        const isActive = params.value;
        const className = isActive ? 'text-green-600' : 'text-gray-600';
        return `<span class="${className}">${isActive ? 'Active' : 'Inactive'}</span>`;
      }
    },
    {
      headerName: 'Created',
      field: 'createdAt',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => {
        if (!params.value) return '';
        return new Date(params.value).toLocaleDateString();
      }
    },
    {
      headerName: 'Actions',
      field: 'ticketTypeId',
      flex: 0.7,
      minWidth: 100,
      sortable: false,
      filter: false,
      cellRenderer: (params: any) => {
        return `
          <div class="flex items-center justify-center h-full">
            <button class="action-view-btn text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 cursor-pointer" title="Quick preview" aria-label="Quick preview">
              <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
                <path d="M1.333 8s2.667-4 6.667-4 6.667 4 6.667 4-2.667 4-6.667 4-6.667-4-6.667-4Z" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"/>
                <circle cx="8" cy="8" r="2" stroke="currentColor" stroke-width="1.4"/>
              </svg>
            </button>
          </div>
        `;
      },
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target?.closest('.action-view-btn')) {
          this.selectedTicketType = params.data;
          this.showDetailsModal = true;
        }
      }
    }
  ];

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true,
    flex: 1
  };

  get gridThemeClass(): string {
    return this.themeService.isDarkMode ? 'ag-theme-quartz-dark' : 'ag-theme-quartz';
  }

  constructor(
    private ticketTypeService: TicketTypeService,
    private stateService: TicketManagementStateService,
    private fb: FormBuilder,
    private themeService: ThemeService
  ) {
    this.initializeForm();
    
    // React to state changes using Angular signals effect
    effect(() => {
      const ticketTypes = this.stateService.ticketTypes();
      this.rowData = ticketTypes || [];
      if (this.gridApi) {
        this.gridApi.sizeColumnsToFit();
        if (this.rowData.length === 0) {
          this.gridApi.showNoRowsOverlay();
        }
      }
    });
  }

  ngOnInit(): void {
    // Load data only if not already cached
    this.loadTicketTypes();
  }

  initializeForm(): void {
    this.createForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      itilCategory: ['', Validators.required],
      initialStateId: ['', Validators.required]
    });
  }

  loadTicketTypes(): void {
    // The service will return cached data if available
    this.ticketTypeService.getAll().subscribe();
  }

  onGridReady(params: GridReadyEvent): void {
    this.gridApi = params.api;
    this.gridReady = true;
    this.gridApi.sizeColumnsToFit();
    
    // Ensure we show empty state if no data
    if (!this.rowData || this.rowData.length === 0) {
      this.gridApi.showNoRowsOverlay();
    }
  }

  onRowClicked(event: any): void {
    const target = event.event?.target as HTMLElement;
    if (!target?.closest('.action-view-btn')) {
      this.selectedTicketType = event.data;
      this.showDetailsModal = true;
    }
  }

  openCreateModal(): void {
    this.showCreateModal = true;
    this.createForm.reset();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
    this.createForm.reset();
    this.attributeDefinitions = [];
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedTicketType = null;
  }

  onSubmit(): void {
    if (this.createForm.invalid || this.isSubmitting) {
      return;
    }

    // Validate dropdown attributes have options
    const invalidDropdown = this.attributeDefinitions.find(
      attr => attr.dataType === 'DROPDOWN' && (!attr.options || attr.options.length === 0)
    );
    
    if (invalidDropdown) {
      alert(`Dropdown attribute "${invalidDropdown.label}" must have at least one option.`);
      return;
    }

    // Validate all attributes have required fields
    const invalidAttribute = this.attributeDefinitions.find(
      attr => !attr.key || !attr.label || !attr.dataType
    );
    
    if (invalidAttribute) {
      alert('All attributes must have a key, label, and data type.');
      return;
    }

    this.isSubmitting = true;
    const formValue = this.createForm.value;

    const request: CreateTicketTypeRequest = {
      name: formValue.name,
      description: formValue.description,
      itilCategory: formValue.itilCategory,
      workflowConfig: {
        states: [
          {
            id: formValue.initialStateId,
            label: this.formatWorkflowStateLabel(formValue.initialStateId),
            type: 'OPEN',
            allowedTransitions: []
          }
        ],
        initialStateId: formValue.initialStateId
      },
      attributeDefinitions: this.attributeDefinitions
    };

    this.ticketTypeService.create(request).subscribe({
      next: (response) => {
        console.log('Ticket type created:', response);
        // Reload data from API (will update state)
        this.ticketTypeService.getAll(true).subscribe();
        this.closeCreateModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error creating ticket type:', error);
        alert('Failed to create ticket type. Please check the form and try again.');
        this.isSubmitting = false;
      }
    });
  }

  deleteTicketType(id: string): void {
    if (!confirm('Are you sure you want to delete this ticket type?')) {
      return;
    }

    this.ticketTypeService.delete(id).subscribe({
      next: () => {
        console.log('Ticket type deleted');
        // State is automatically updated by the service
      },
      error: (error) => {
        console.error('Error deleting ticket type:', error);
      }
    });
  }

  private formatWorkflowStateLabel(stateId: string): string {
    return (stateId || '')
      .trim()
      .replace(/[_-]+/g, ' ')
      .split(' ')
      .filter(Boolean)
      .map((part) => part.charAt(0).toUpperCase() + part.slice(1).toLowerCase())
      .join(' ');
  }
}
