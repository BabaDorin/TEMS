import { Component, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { TicketService } from 'src/app/services/ticket.service';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { Ticket, CreateTicketRequest, TicketMessage, AddMessageRequest } from 'src/app/models/ticket/ticket.model';
import { TicketType } from 'src/app/models/ticket/ticket-type.model';
import { TicketManagementStateService } from 'src/app/state/ticket-management.state';

@Component({
  selector: 'app-view-tickets',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridAngular
  ],
  templateUrl: './view-tickets.component.html',
  styleUrls: ['./view-tickets.component.scss']
})
export class ViewTicketsComponent implements OnInit {
  rowData: Ticket[] = [];
  ticketTypes: TicketType[] = [];
  selectedTicketType: TicketType | null = null;
  dynamicAttributeValues: { [key: string]: any } = {};
  gridApi!: GridApi;
  showCreateModal = false;
  showPreviewModal = false;
  selectedTicket: Ticket | null = null;
  previewTicketType: TicketType | null = null;
  createForm!: FormGroup;
  isSubmitting = false;
  gridReady = false;

  priorities = [
    { label: 'Low', value: 'LOW' },
    { label: 'Medium', value: 'MEDIUM' },
    { label: 'High', value: 'HIGH' },
    { label: 'Critical', value: 'CRITICAL' }
  ];

  columnDefs: ColDef[] = [
    {
      headerName: 'ID',
      field: 'humanReadableId',
      flex: 1,
      minWidth: 120,
      cellClass: 'font-medium'
    },
    {
      headerName: 'Summary',
      field: 'summary',
      flex: 3,
      minWidth: 250
    },
    {
      headerName: 'Priority',
      field: 'priority',
      flex: 1,
      minWidth: 100,
      cellRenderer: (params: any) => {
        const priority = params.value;
        let className = 'text-gray-600';
        if (priority === 'CRITICAL') className = 'text-red-600 font-semibold';
        else if (priority === 'HIGH') className = 'text-orange-600 font-semibold';
        else if (priority === 'MEDIUM') className = 'text-yellow-600';
        return `<span class="${className}">${priority}</span>`;
      }
    },
    {
      headerName: 'State',
      field: 'currentStateId',
      flex: 1,
      minWidth: 120
    },
    {
      headerName: 'Channel',
      field: 'reporter.channelSource',
      flex: 1,
      minWidth: 100
    },
    {
      headerName: 'Created',
      field: 'auditMetadata.createdAt',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => {
        if (!params.value) return '';
        return new Date(params.value).toLocaleDateString();
      }
    },
    {
      headerName: 'Actions',
      flex: 1,
      minWidth: 150,
      cellRenderer: (params: any) => {
        return `
          <div class="flex gap-2">
            <button class="action-preview-btn px-2 py-1 text-blue-600 hover:text-blue-800 text-sm">
              Preview
            </button>
            <button class="action-delete-btn px-2 py-1 text-red-600 hover:text-red-800 text-sm">
              Delete
            </button>
          </div>
        `;
      },
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target.classList.contains('action-delete-btn')) {
          this.deleteTicket(params.data.ticketId);
        } else if (target.classList.contains('action-preview-btn')) {
          this.openPreviewModal(params.data);
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

  constructor(
    private ticketService: TicketService,
    private ticketTypeService: TicketTypeService,
    private stateService: TicketManagementStateService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.initializeForms();
    
    // React to tickets state changes
    effect(() => {
      const tickets = this.stateService.tickets();
      this.rowData = tickets || [];
      if (this.gridApi) {
        this.gridApi.sizeColumnsToFit();
        if (this.rowData.length === 0) {
          this.gridApi.showNoRowsOverlay();
        }
      }
    });
    
    // React to ticket types state changes
    effect(() => {
      const ticketTypes = this.stateService.ticketTypes();
      // Filter by isActive only if the property exists (backend may not return it)
      this.ticketTypes = ticketTypes?.filter(t => t.isActive !== false) || [];
    });
  }

  ngOnInit(): void {
    // Load data only if not already cached
    this.loadTickets();
    this.loadTicketTypes();
  }

  initializeForms(): void {
    this.createForm = this.fb.group({
      ticketTypeId: ['', Validators.required],
      summary: ['', Validators.required],
      priority: ['MEDIUM', Validators.required],
      channelSource: ['WEB', Validators.required]
    });
  }

  loadTickets(): void {
    // The service will return cached data if available
    this.ticketService.getAll().subscribe();
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
    if (!target.classList.contains('action-delete-btn') && !target.classList.contains('action-preview-btn')) {
      // Navigate to ticket detail page when clicking on the row
      this.router.navigate(['/technical-support/tickets', event.data.ticketId]);
    }
  }

  openPreviewModal(ticket: Ticket): void {
    this.selectedTicket = ticket;
    this.loadPreviewTicketType(ticket.ticketTypeId);
    this.showPreviewModal = true;
  }

  closePreviewModal(): void {
    this.showPreviewModal = false;
    this.selectedTicket = null;
    this.previewTicketType = null;
  }

  loadPreviewTicketType(ticketTypeId: string): void {
    const ticketType = this.ticketTypes.find(tt => tt.ticketTypeId === ticketTypeId);
    if (ticketType) {
      this.previewTicketType = ticketType;
    } else {
      // Load from API if not in cache
      this.ticketTypeService.getById(ticketTypeId).subscribe({
        next: (type) => {
          this.previewTicketType = type;
        },
        error: (error) => {
          console.error('Error loading ticket type:', error);
        }
      });
    }
  }

  openFullTicket(): void {
    if (this.selectedTicket) {
      const ticketId = this.selectedTicket.ticketId;
      console.log('Navigating to ticket:', ticketId);
      this.closePreviewModal();
      this.router.navigate(['/technical-support/tickets', ticketId])
        .then(() => console.log('Navigation successful'))
        .catch(err => console.error('Navigation error:', err));
    }
  }

  getAttributeKeys(attributes: { [key: string]: any }): string[] {
    return Object.keys(attributes || {});
  }

  getAttributeLabel(key: string): string {
    const attr = this.previewTicketType?.attributeDefinitions?.find(a => a.key === key);
    return attr?.label || key;
  }

  formatAttributeValue(value: any): string {
    if (typeof value === 'boolean') {
      return value ? 'Yes' : 'No';
    }
    if (value === null || value === undefined) {
      return 'N/A';
    }
    return String(value);
  }



  openCreateModal(): void {
    this.showCreateModal = true;
    this.createForm.reset({
      priority: 'MEDIUM',
      channelSource: 'WEB'
    });
    this.selectedTicketType = null;
    this.dynamicAttributeValues = {};
    
    // Ensure ticket types are loaded
    if (this.ticketTypes.length === 0) {
      this.loadTicketTypes();
    }
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
    this.createForm.reset();
    this.selectedTicketType = null;
    this.dynamicAttributeValues = {};
  }

  onTicketTypeChange(ticketTypeId: string): void {
    this.selectedTicketType = this.ticketTypes.find(tt => tt.ticketTypeId === ticketTypeId) || null;
    this.dynamicAttributeValues = {};
    
    if (this.selectedTicketType?.attributeDefinitions) {
      this.selectedTicketType.attributeDefinitions.forEach(attr => {
        if (attr.dataType === 'BOOL') {
          this.dynamicAttributeValues[attr.key] = false;
        } else {
          this.dynamicAttributeValues[attr.key] = '';
        }
      });
    }
  }

  isAttributeValid(key: string): boolean {
    const attr = this.selectedTicketType?.attributeDefinitions.find(a => a.key === key);
    if (!attr || !attr.isRequired) {
      return true;
    }
    const value = this.dynamicAttributeValues[key];
    if (attr.dataType === 'BOOL') {
      return value !== undefined && value !== null;
    }
    return value !== '' && value !== null && value !== undefined;
  }

  areAllAttributesValid(): boolean {
    if (!this.selectedTicketType?.attributeDefinitions) {
      return true;
    }
    return this.selectedTicketType.attributeDefinitions
      .filter(attr => attr.isRequired)
      .every(attr => this.isAttributeValid(attr.key));
  }



  onSubmit(): void {
    if (this.createForm.invalid || this.isSubmitting || !this.areAllAttributesValid()) {
      return;
    }

    this.isSubmitting = true;
    const formValue = this.createForm.value;

    const request: CreateTicketRequest = {
      ticketTypeId: formValue.ticketTypeId,
      summary: formValue.summary,
      priority: formValue.priority,
      reporter: {
        userId: 'current-user-id', // TODO: Get from auth service
        channelSource: formValue.channelSource
      },
      attributes: this.dynamicAttributeValues
    };

    this.ticketService.create(request).subscribe({
      next: (response) => {
        console.log('Ticket created:', response);
        // Reload data from API (will update state)
        this.ticketService.getAll(true).subscribe();
        this.closeCreateModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error creating ticket:', error);
        this.isSubmitting = false;
      }
    });
  }



  deleteTicket(id: string): void {
    if (!confirm('Are you sure you want to delete this ticket?')) {
      return;
    }

    this.ticketService.delete(id).subscribe({
      next: () => {
        console.log('Ticket deleted');
        // State is automatically updated by the service
      },
      error: (error) => {
        console.error('Error deleting ticket:', error);
      }
    });
  }
}
