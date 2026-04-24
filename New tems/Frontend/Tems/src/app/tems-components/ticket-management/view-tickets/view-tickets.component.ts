import { Component, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TicketService } from 'src/app/services/ticket.service';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { ThemeService } from 'src/app/services/theme.service';
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
    AgGridAngular,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './view-tickets.component.html',
  styleUrls: ['./view-tickets.component.scss']
})
export class ViewTicketsComponent implements OnInit {
  activeStatusTab: 'new' | 'in-progress' | 'done' = 'new';
  allTickets: Ticket[] = [];
  rowData: Ticket[] = [];
  ticketTypes: TicketType[] = [];
  selectedTicketType: TicketType | null = null;
  dynamicAttributeValues: { [key: string]: any } = {};
  ticketTypeSearch: FormControl<string | null> = new FormControl('');
  gridApi!: GridApi;
  showCreateModal = false;
  showPreviewModal = false;
  selectedTicket: Ticket | null = null;
  previewTicketType: TicketType | null = null;
  createForm!: FormGroup;
  isSubmitting = false;
  gridReady = false;

  priorities = [
    { label: 'Low', value: 'LOW', dotClass: 'bg-yellow-400' },
    { label: 'Medium', value: 'MEDIUM', dotClass: 'bg-orange-500' },
    { label: 'High', value: 'HIGH', dotClass: 'bg-red-500' },
    { label: 'Critical', value: 'CRITICAL', dotClass: 'bg-black' }
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
      headerName: 'Title',
      field: 'title',
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
        const label = this.getPriorityLabel(priority);
        const badgeClass = this.getPriorityBadgeClass(priority);
        const dotClass = this.getPriorityDotClass(priority);
        return `<span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold ${badgeClass}"><span class="w-2 h-2 rounded-full ${dotClass}"></span>${label}</span>`;
      }
    },
    {
      headerName: 'Channel',
      field: 'reporter.channelSource',
      flex: 1,
      minWidth: 100
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
      field: 'ticketId',
      flex: 0.7,
      minWidth: 100,
      sortable: false,
      filter: false,
      cellRenderer: () => {
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

  get gridThemeClass(): string {
    return this.themeService.isDarkMode ? 'ag-theme-quartz-dark' : 'ag-theme-quartz';
  }

  getPriorityLabel(priority: string): string {
    return (priority || '').toLowerCase().replace(/\b\w/g, (match) => match.toUpperCase());
  }

  getPriorityBadgeClass(priority: string): string {
    const normalized = (priority || '').toUpperCase();
    switch (normalized) {
      case 'CRITICAL':
        return 'bg-gray-100 text-black dark:bg-white/10 dark:text-white';
      case 'HIGH':
        return 'bg-red-100 text-red-800 dark:bg-red-500/15 dark:text-red-300';
      case 'MEDIUM':
        return 'bg-orange-100 text-orange-800 dark:bg-orange-500/15 dark:text-orange-300';
      case 'LOW':
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-500/15 dark:text-yellow-300';
      default:
        return 'bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-200';
    }
  }

  getPriorityDotClass(priority: string): string {
    const normalized = (priority || '').toUpperCase();
    switch (normalized) {
      case 'CRITICAL':
        return 'bg-black';
      case 'HIGH':
        return 'bg-red-500';
      case 'MEDIUM':
        return 'bg-orange-500';
      case 'LOW':
        return 'bg-yellow-400';
      default:
        return 'bg-gray-400';
    }
  }

  getTicketStatusLabel(stateId: string): string {
    const managed = this.getManagedStatusGroup(stateId);
    if (managed === 'new') return 'New';
    if (managed === 'in-progress') return 'In progress';
    if (managed === 'closed') return 'Closed';

    const normalized = this.normalizeStateId(stateId);
    if (!normalized) return '—';
    return normalized
      .split('-')
      .filter(Boolean)
      .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
      .join(' ');
  }

  private normalizeStateId(value: string): string {
    return (value || '')
      .trim()
      .toLowerCase()
      .replace(/\s+/g, '-')
      .replace(/_+/g, '-');
  }

  private getManagedStatusGroup(value: string): string | null {
    const normalized = this.normalizeStateId(value);
    if (!normalized) return null;
    if (['new', 'open', 'state-new'].includes(normalized)) return 'new';
    if (['in-progress', 'state-in-progress', 'state-wip', 'state-wip', 'wip', 'progress'].includes(normalized)) return 'in-progress';
    if (['closed', 'state-closed'].includes(normalized)) return 'closed';
    return null;
  }

  getTicketStatusTab(stateId: string): 'new' | 'in-progress' | 'done' | null {
    const managed = this.getManagedStatusGroup(stateId);
    if (managed === 'new') return 'new';
    if (managed === 'in-progress') return 'in-progress';
    if (managed === 'closed') return 'done';
    return null;
  }

  get statusTabCounts(): Record<'new' | 'in-progress' | 'done', number> {
    return this.allTickets.reduce((acc, ticket) => {
      const tab = this.getTicketStatusTab(ticket.currentStateId);
      if (tab) {
        acc[tab] += 1;
      }
      return acc;
    }, { new: 0, 'in-progress': 0, done: 0 });
  }

  setStatusTab(tab: 'new' | 'in-progress' | 'done'): void {
    this.activeStatusTab = tab;
    this.applyActiveStatusFilter();
    if (this.gridApi) {
      this.gridApi.deselectAll();
      this.gridApi.sizeColumnsToFit();
      if (this.rowData.length === 0) {
        this.gridApi.showNoRowsOverlay();
      } else {
        this.gridApi.hideOverlay();
      }
    }
  }

  private applyActiveStatusFilter(): void {
    this.rowData = this.allTickets.filter((ticket) => this.getTicketStatusTab(ticket.currentStateId) === this.activeStatusTab);
  }

  constructor(
    private ticketService: TicketService,
    private ticketTypeService: TicketTypeService,
    private stateService: TicketManagementStateService,
    private fb: FormBuilder,
    private router: Router,
    private themeService: ThemeService
  ) {
    this.initializeForms();
    
    // React to tickets state changes
    effect(() => {
      const tickets = this.stateService.tickets();
      this.allTickets = tickets || [];
      this.applyActiveStatusFilter();
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
      title: ['', [Validators.required, Validators.maxLength(50)]],
      summary: ['', [Validators.required, Validators.maxLength(2000)]],
      priority: ['MEDIUM', Validators.required]
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
    if (!target?.closest('.action-view-btn')) {
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
      ticketTypeId: '',
      title: '',
      summary: ''
    });
    this.ticketTypeSearch.setValue('');
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
    this.ticketTypeSearch.setValue('');
    this.selectedTicketType = null;
    this.dynamicAttributeValues = {};
  }

  get hasAdditionalFields(): boolean {
    return this.editableAttributeDefinitions.length > 0;
  }

  get editableAttributeDefinitions() {
    return this.selectedTicketType?.attributeDefinitions?.filter(attr => !attr.isPredefined) ?? [];
  }

  get filteredTicketTypes(): TicketType[] {
    const search = (this.ticketTypeSearch.value || '').toString().trim().toLowerCase();
    if (!search) {
      return this.ticketTypes;
    }

    return this.ticketTypes.filter(type => {
      const name = (type.name || '').toLowerCase();
      const description = (type.description || '').toLowerCase();
      const category = (type.itilCategory || '').toLowerCase();
      return name.includes(search) || description.includes(search) || category.includes(search);
    });
  }

  get isSubmitDisabled(): boolean {
    return this.createForm.invalid || this.isSubmitting || (this.hasAdditionalFields && !this.areAllAttributesValid());
  }

  onTicketTypeChange(ticketTypeId: string | null): void {
    this.selectedTicketType = this.ticketTypes.find(tt => tt.ticketTypeId === ticketTypeId) || null;
    this.dynamicAttributeValues = {};
    
    if (this.editableAttributeDefinitions.length > 0) {
      this.editableAttributeDefinitions.forEach(attr => {
        if (attr.dataType === 'BOOL') {
          this.dynamicAttributeValues[attr.key] = false;
        } else {
          this.dynamicAttributeValues[attr.key] = '';
        }
      });
    }
  }

  onTicketTypeSearchInput(value: string): void {
    const normalized = value?.trim().toLowerCase() || '';
    const matchedType = this.ticketTypes.find(type =>
      type.name.trim().toLowerCase() === normalized ||
      type.ticketTypeId === value
    );

    if (!matchedType) {
      this.selectedTicketType = null;
      this.createForm.get('ticketTypeId')?.setValue('');
      this.dynamicAttributeValues = {};
      return;
    }

    this.onTicketTypeChange(matchedType.ticketTypeId);
  }

  onTicketTypeSelected(type: TicketType): void {
    this.selectedTicketType = type;
    this.createForm.get('ticketTypeId')?.setValue(type.ticketTypeId);
    this.ticketTypeSearch.setValue(type.name, { emitEvent: false });
    this.dynamicAttributeValues = {};

    const editableAttributes = type.attributeDefinitions?.filter(attr => !attr.isPredefined) ?? [];
    if (editableAttributes.length > 0) {
      editableAttributes.forEach(attr => {
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
    if (this.editableAttributeDefinitions.length === 0) {
      return true;
    }
    return this.editableAttributeDefinitions
      .filter(attr => attr.isRequired)
      .every(attr => this.isAttributeValid(attr.key));
  }



  onSubmit(): void {
    if (this.isSubmitDisabled) {
      return;
    }

    this.isSubmitting = true;
    const formValue = this.createForm.value;

    const request: CreateTicketRequest = {
      ticketTypeId: formValue.ticketTypeId,
      title: (formValue.title || '').trim(),
      summary: (formValue.summary || '').trim(),
      priority: formValue.priority,
      reporter: {
        userId: 'current-user-id', // TODO: Get from auth service
        channelSource: 'WEB'
      },
      attributes: this.hasAdditionalFields ? this.dynamicAttributeValues : {}
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
