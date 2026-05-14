import { Component, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { ThemeService } from 'src/app/services/theme.service';
import {
  AttributeDefinition,
  CreateTicketTypeRequest,
  TicketType,
  UpdateTicketTypeRequest
} from 'src/app/models/ticket/ticket-type.model';
import { TicketManagementStateService } from 'src/app/state/ticket-management.state';
import { AttributeBuilder } from 'src/app/components/ticket-type/attribute-builder/attribute-builder';
import { TokenService } from 'src/app/services/token.service';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

type TicketTypeModalMode = 'create' | 'edit';

@Component({
  selector: 'app-view-ticket-types',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AgGridAngular,
    AttributeBuilder,
    CustomSelectComponent
  ],
  templateUrl: './view-ticket-types.component.html',
  styleUrls: ['./view-ticket-types.component.scss']
})
export class ViewTicketTypesComponent implements OnInit {
  rowData: TicketType[] = [];
  gridApi!: GridApi;
  showTypeFormModal = false;
  showDetailsModal = false;
  showDeleteModal = false;
  selectedTicketType: TicketType | null = null;
  ticketTypePendingDelete: TicketType | null = null;
  typeForm!: FormGroup;
  isSubmitting = false;
  isDeleteSubmitting = false;
  gridReady = false;
  attributeDefinitions: AttributeDefinition[] = [];
  validationMessage = '';
  formModalMode: TicketTypeModalMode = 'create';
  canManageTickets = false;
  itilCategoryOptions: SelectOption[] = [
    { value: 'INCIDENT', label: 'Incident' },
    { value: 'PROBLEM', label: 'Problem' },
    { value: 'CHANGE', label: 'Change Request' },
    { value: 'REQUEST', label: 'Service Request' },
    { value: 'SECURITY_INCIDENT', label: 'Security Incident' },
    { value: 'ALERT', label: 'Alert / Event' }
  ];

  columnDefs: ColDef[] = [
    {
      headerName: 'Name',
      field: 'name',
      flex: 2,
      minWidth: 150,
      cellClass: 'font-medium'
    },
    {
      headerName: 'Category',
      field: 'itilCategory',
      flex: 1,
      minWidth: 150
    },
    {
      headerName: 'Created',
      field: 'createdAt',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => {
        if (!params.value) return '';
        return new Date(params.value).toLocaleDateString('en-GB', {
          day: '2-digit',
          month: 'short',
          year: 'numeric'
        });
      }
    },
    {
      headerName: 'Actions',
      field: 'ticketTypeId',
      flex: 0.9,
      minWidth: 132,
      sortable: false,
      filter: false,
      cellRenderer: () => this.renderActionsCell(),
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;

        if (target?.closest('.action-view-btn')) {
          this.openDetailsModal(params.data);
          return;
        }

        if (!this.canManageTickets) {
          return;
        }

        if (target?.closest('.action-edit-btn')) {
          this.openEditModal(params.data);
          return;
        }

        if (target?.closest('.action-delete-btn')) {
          this.openDeleteModal(params.data);
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

  get formModalTitle(): string {
    return this.formModalMode === 'create' ? 'Create Ticket Type' : 'Edit Ticket Type';
  }

  get formSubmitLabel(): string {
    if (this.isSubmitting) {
      return this.formModalMode === 'create' ? 'Creating...' : 'Saving...';
    }

    return this.formModalMode === 'create' ? 'Create Ticket Type' : 'Save Changes';
  }

  constructor(
    private ticketTypeService: TicketTypeService,
    private stateService: TicketManagementStateService,
    private fb: FormBuilder,
    private themeService: ThemeService,
    private tokenService: TokenService
  ) {
    this.initializeForm();
    this.canManageTickets = this.tokenService.canManageTickets();

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
    this.loadTicketTypes();
  }

  initializeForm(): void {
    this.typeForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      itilCategory: ['', Validators.required]
    });
  }

  loadTicketTypes(): void {
    this.ticketTypeService.getAll().subscribe();
  }

  onGridReady(params: GridReadyEvent): void {
    this.gridApi = params.api;
    this.gridReady = true;
    this.gridApi.sizeColumnsToFit();

    if (!this.rowData || this.rowData.length === 0) {
      this.gridApi.showNoRowsOverlay();
    }
  }

  onRowClicked(event: any): void {
    const target = event.event?.target as HTMLElement;
    if (!target?.closest('.ticket-type-action-btn')) {
      this.openDetailsModal(event.data);
    }
  }

  openCreateModal(): void {
    if (!this.canManageTickets) {
      return;
    }

    this.formModalMode = 'create';
    this.selectedTicketType = null;
    this.validationMessage = '';
    this.attributeDefinitions = [];
    this.typeForm.reset({
      name: '',
      description: '',
      itilCategory: ''
    });
    this.showTypeFormModal = true;
  }

  openEditModal(ticketType: TicketType): void {
    if (!this.canManageTickets) {
      return;
    }

    this.formModalMode = 'edit';
    this.selectedTicketType = ticketType;
    this.validationMessage = '';
    this.attributeDefinitions = this.cloneAttributes(ticketType.attributeDefinitions || []);
    this.typeForm.reset({
      name: ticketType.name,
      description: ticketType.description,
      itilCategory: ticketType.itilCategory
    });
    this.showTypeFormModal = true;
  }

  closeTypeFormModal(): void {
    this.showTypeFormModal = false;
    this.isSubmitting = false;
    this.validationMessage = '';
    this.attributeDefinitions = [];
    this.typeForm.reset();

    if (this.formModalMode === 'create') {
      this.selectedTicketType = null;
    }
  }

  openDetailsModal(ticketType: TicketType): void {
    this.selectedTicketType = ticketType;
    this.showDetailsModal = true;
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedTicketType = null;
  }

  openDeleteModal(ticketType: TicketType): void {
    if (!this.canManageTickets) {
      return;
    }

    this.ticketTypePendingDelete = ticketType;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.ticketTypePendingDelete = null;
    this.isDeleteSubmitting = false;
  }

  onSubmit(): void {
    if (this.typeForm.invalid || this.isSubmitting) {
      this.typeForm.markAllAsTouched();
      return;
    }

    const validationMessage = this.validateAttributes();
    if (validationMessage) {
      this.validationMessage = validationMessage;
      return;
    }

    this.validationMessage = '';
    this.isSubmitting = true;

    const formValue = this.typeForm.getRawValue();

    if (this.formModalMode === 'create') {
      const request: CreateTicketTypeRequest = {
        name: formValue.name,
        description: formValue.description,
        itilCategory: formValue.itilCategory,
        attributeDefinitions: this.cloneAttributes(this.attributeDefinitions)
      };

      this.ticketTypeService.create(request).subscribe({
        next: () => {
          this.ticketTypeService.getAll(true).subscribe();
          this.closeTypeFormModal();
        },
        error: (error) => {
          console.error('Error creating ticket type:', error);
          this.validationMessage = 'Failed to create ticket type. Please review the form and try again.';
          this.isSubmitting = false;
        }
      });
      return;
    }

    if (!this.selectedTicketType) {
      this.validationMessage = 'No ticket type selected for editing.';
      this.isSubmitting = false;
      return;
    }

    const request: UpdateTicketTypeRequest = {
      name: formValue.name,
      description: formValue.description,
      itilCategory: formValue.itilCategory,
      version: this.selectedTicketType.version,
      attributeDefinitions: this.cloneAttributes(this.attributeDefinitions)
    };

    this.ticketTypeService.update(this.selectedTicketType.ticketTypeId, request).subscribe({
      next: () => {
        this.ticketTypeService.getAll(true).subscribe();
        this.closeTypeFormModal();
      },
      error: (error) => {
        console.error('Error updating ticket type:', error);
        this.validationMessage = 'Failed to save ticket type changes. Please try again.';
        this.isSubmitting = false;
      }
    });
  }

  confirmDelete(): void {
    if (!this.ticketTypePendingDelete || this.isDeleteSubmitting) {
      return;
    }

    this.isDeleteSubmitting = true;
    this.ticketTypeService.delete(this.ticketTypePendingDelete.ticketTypeId).subscribe({
      next: () => {
        this.ticketTypeService.getAll(true).subscribe();
        this.closeDeleteModal();
      },
      error: (error) => {
        console.error('Error deleting ticket type:', error);
        this.isDeleteSubmitting = false;
      }
    });
  }

  private validateAttributes(): string {
    const invalidDropdown = this.attributeDefinitions.find(
      (attr) => attr.dataType === 'DROPDOWN' && (!attr.options || attr.options.every((option) => !option.trim()))
    );

    if (invalidDropdown) {
      return `Dropdown attribute "${invalidDropdown.label || invalidDropdown.key || 'Untitled'}" must have at least one option.`;
    }

    const invalidAttribute = this.attributeDefinitions.find(
      (attr) => !attr.key?.trim() || !attr.label?.trim() || !attr.dataType
    );

    if (invalidAttribute) {
      return 'Each attribute needs a key, label, and data type before saving.';
    }

    return '';
  }

  private renderActionsCell(): string {
    const manageActions = this.canManageTickets ? `
      <button class="ticket-type-action-btn action-edit-btn" title="Edit ticket type" aria-label="Edit ticket type">
        <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
          <path d="M11.333 2.001a1.414 1.414 0 0 1 2 2L6 11.334 3.333 12l.666-2.667 7.334-7.332Z" stroke="currentColor" stroke-width="1.35" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      <button class="ticket-type-action-btn action-delete-btn" title="Delete ticket type" aria-label="Delete ticket type">
        <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
          <path d="M2.667 4h10.666M6.667 6.667v4.666M9.333 6.667v4.666M4 4.667l.333 7A1.333 1.333 0 0 0 5.666 13h4.668a1.333 1.333 0 0 0 1.333-1.333l.333-7M6 4V2.667h4V4" stroke="currentColor" stroke-width="1.35" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
    ` : '';

    return `
      <div class="ticket-type-actions-cell">
        <button class="ticket-type-action-btn action-view-btn" title="Preview ticket type" aria-label="Preview ticket type">
          <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
            <path d="M1.333 8s2.667-4 6.667-4 6.667 4 6.667 4-2.667 4-6.667 4-6.667-4-6.667-4Z" stroke="currentColor" stroke-width="1.35" stroke-linecap="round" stroke-linejoin="round"/>
            <circle cx="8" cy="8" r="2" stroke="currentColor" stroke-width="1.35"/>
          </svg>
        </button>
        ${manageActions}
      </div>
    `;
  }

  private cloneAttributes(attributes: AttributeDefinition[]): AttributeDefinition[] {
    return attributes.map((attribute) => ({
      ...attribute,
      options: attribute.options ? [...attribute.options] : []
    }));
  }
}
