import { Component, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { TicketService } from 'src/app/services/ticket.service';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { TokenService } from 'src/app/services/token.service';
import { ThemeService } from 'src/app/services/theme.service';
import { UserService } from 'src/app/services/user.service';
import { Ticket, CreateTicketRequest, TicketMessage, AddMessageRequest, ApprovalGateApprover } from 'src/app/models/ticket/ticket.model';
import { AttributeDefinition, TicketType } from 'src/app/models/ticket/ticket-type.model';
import { UserDto } from 'src/app/models/user/user-management.model';
import { TicketManagementStateService } from 'src/app/state/ticket-management.state';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-view-tickets',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridAngular,
    CustomSelectComponent
  ],
  templateUrl: './view-tickets.component.html',
  styleUrls: ['./view-tickets.component.scss']
})
export class ViewTicketsComponent implements OnInit {
  activeStatusTab: 'approval' | 'new' | 'in-progress' | 'closed' = 'approval';
  allTickets: Ticket[] = [];
  approvalTickets: Ticket[] = [];
  rowData: Ticket[] = [];
  ticketTypes: TicketType[] = [];
  selectedTicketType: TicketType | null = null;
  dynamicAttributeValues: { [key: string]: any } = {};
  gridApi!: GridApi;
  showCreateModal = false;
  showPreviewModal = false;
  selectedTicket: Ticket | null = null;
  previewCreatorUser: UserDto | null = null;
  previewCreatorUserLoading = false;
  previewTicketType: TicketType | null = null;
  createForm!: FormGroup;
  isSubmitting = false;
  gridReady = false;
  canManageTickets = false;
  canOpenTickets = false;
  currentUserId = '';
  private currentUserIdentifiers = new Set<string>();

  priorities = [
    { label: 'Low', value: 'LOW', dotClass: 'bg-yellow-400' },
    { label: 'Medium', value: 'MEDIUM', dotClass: 'bg-orange-500' },
    { label: 'High', value: 'HIGH', dotClass: 'bg-red-500' },
    { label: 'Critical', value: 'CRITICAL', dotClass: 'bg-black' }
  ];

  columnDefs: ColDef[] = [];

  get ticketTypeOptions(): SelectOption[] {
    return this.ticketTypes.map((type) => ({
      value: type.ticketTypeId,
      label: type.name
    }));
  }

  private buildColumnDefs(): ColDef[] {
    const baseColumns: ColDef[] = [
    {
      headerName: 'ID',
      field: 'humanReadableId',
      flex: 1,
      minWidth: 80,
      cellClass: 'font-medium'
    },
    {
      headerName: 'Priority',
      field: 'priority',
      flex: 1,
      minWidth: 120,
      cellRenderer: (params: any) => {
        const priority = params.value;
        const label = this.getPriorityLabel(priority);
        const badgeClass = this.getPriorityBadgeClass(priority);
        const dotClass = this.getPriorityDotClass(priority);
        return `<span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold ${badgeClass}"><span class="w-2 h-2 rounded-full ${dotClass}"></span>${label}</span>`;
      }
    },
    {
      headerName: 'Type',
      field: 'ticketTypeId',
      flex: 1.4,
      minWidth: 170,
      valueGetter: (params) => this.getTicketTypeName(params.data)
    },
    {
      headerName: 'Title',
      field: 'title',
      flex: 3,
      minWidth: 250
    },
    ...(this.activeStatusTab === 'approval' ? [{
      headerName: 'Your status',
      flex: 1.3,
      minWidth: 160,
      sortable: false,
      filter: false,
      cellRenderer: (params: any) => {
        const status = this.getCurrentUserApprovalStatus(params.data);
        const label = this.getCurrentUserApprovalStatusLabel(params.data);
        const badgeClass = this.getApprovalStatusBadgeClass(status);
        return `<span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold ${badgeClass}">${label}</span>`;
      }
    }] : []),
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

    return baseColumns;
  }

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

  getTicketTypeName(ticket: Ticket | null | undefined): string {
    if (!ticket?.ticketTypeId) {
      return '—';
    }

    const ticketType = this.ticketTypes.find((type) => type.ticketTypeId === ticket.ticketTypeId);
    return ticketType?.name || '—';
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

  getTicketStatusTab(stateId: string): 'new' | 'in-progress' | 'closed' | null {
    const managed = this.getManagedStatusGroup(stateId);
    if (managed === 'new') return 'new';
    if (managed === 'in-progress') return 'in-progress';
    if (managed === 'closed') return 'closed';
    return null;
  }

  isApprovalTicket(ticket: Ticket): boolean {
    if (!ticket || this.isClosedTicket(ticket.currentStateId)) {
      return false;
    }

    return (ticket.approvalGates || []).some((gate) =>
      (gate.approvers || []).some((approver) => this.matchesCurrentUser(approver.userId)));
  }

  isClosedTicket(stateId: string): boolean {
    return this.getManagedStatusGroup(stateId) === 'closed';
  }

  getCurrentUserApprovalStatus(ticket: Ticket): 'approved' | 'rejected' | 'pending' | '' {
    const approvers = (ticket.approvalGates || []).reduce((acc, gate) => {
      acc.push(...(gate.approvers || []));
      return acc;
    }, [] as ApprovalGateApprover[]);

    const statuses = approvers
      .filter((approver) => this.matchesCurrentUser(approver.userId))
      .map((approver) => (approver.status || '').toLowerCase());

    if (statuses.some((status) => status === 'rejected')) {
      return 'rejected';
    }

    if (statuses.some((status) => status === 'approved')) {
      return 'approved';
    }

    if (statuses.length > 0) {
      return 'pending';
    }

    return '';
  }

  getCurrentUserApprovalStatusLabel(ticket: Ticket): string {
    const status = this.getCurrentUserApprovalStatus(ticket);
    if (status === 'approved') return 'Approved';
    if (status === 'rejected') return 'Rejected';
    if (status === 'pending') return 'Not provided yet';
    return 'Not provided yet';
  }

  getApprovalStatusBadgeClass(status: 'approved' | 'rejected' | 'pending' | ''): string {
    switch (status) {
      case 'approved':
        return 'bg-green-100 text-green-800 dark:bg-green-500/15 dark:text-green-300';
      case 'rejected':
        return 'bg-red-100 text-red-800 dark:bg-red-500/15 dark:text-red-300';
      case 'pending':
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-500/15 dark:text-yellow-300';
      default:
        return 'bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-200';
    }
  }

  private normalizeIdentity(value: unknown): string | null {
    if (typeof value !== 'string') return null;
    const normalized = value.trim().toLowerCase();
    return normalized.length > 0 ? normalized : null;
  }

  private matchesCurrentUser(userId: string): boolean {
    const normalized = this.normalizeIdentity(userId);
    return !!normalized && this.currentUserIdentifiers.has(normalized);
  }

  get statusTabCounts(): Record<'approval' | 'new' | 'in-progress' | 'closed', number> {
    return this.ticketSource.reduce(
      (acc, ticket) => {
        const tab = this.getTicketStatusTab(ticket.currentStateId);
        if (tab) {
          acc[tab] += 1;
        }
        return acc;
      },
      { approval: this.approvalTickets.length, new: 0, 'in-progress': 0, closed: 0 }
    );
  }

  get ticketSource(): Ticket[] {
    return (this.canManageTickets || this.canOpenTickets) ? this.allTickets : [];
  }

  setStatusTab(tab: 'approval' | 'new' | 'in-progress' | 'closed'): void {
    this.activeStatusTab = tab;
    this.refreshColumnDefs();
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
    const source = this.activeStatusTab === 'approval'
      ? this.approvalTickets
      : this.ticketSource;
    this.rowData = source.filter((ticket) => {
      if (this.activeStatusTab === 'approval') {
        return this.isApprovalTicket(ticket);
      }
      return this.getTicketStatusTab(ticket.currentStateId) === this.activeStatusTab;
    });
  }

  private refreshColumnDefs(): void {
    this.columnDefs = this.buildColumnDefs();
  }

  constructor(
    private ticketService: TicketService,
    private ticketTypeService: TicketTypeService,
    private stateService: TicketManagementStateService,
    private fb: FormBuilder,
    private router: Router,
    private themeService: ThemeService,
    private tokenService: TokenService,
    private userService: UserService
  ) {
    this.canManageTickets = this.tokenService.canManageTickets();
    this.canOpenTickets = this.tokenService.canOpenTickets();
    this.currentUserId = this.tokenService.getUserId() || '';
    this.initializeForms();
    this.refreshColumnDefs();
    
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
      this.refreshColumnDefs();
      this.gridApi?.refreshCells({ force: true });
    });
  }

  ngOnInit(): void {
    this.loadCurrentUserContext(() => {
      this.loadTickets();
      this.loadApprovalTickets();
    });
    this.loadTicketTypes();
  }

  private loadCurrentUserContext(onReady?: () => void): void {
    const claims = this.tokenService.getTokenObject() as any;
    const candidates = [
      claims?.sub,
      claims?.oid,
      claims?.preferred_username,
      claims?.upn,
      claims?.unique_name,
      claims?.email,
      claims?.name
    ]
      .map((value: unknown) => this.normalizeIdentity(value))
      .filter((value): value is string => !!value);

    this.currentUserIdentifiers = new Set(candidates);
    this.currentUserId = candidates[0] || this.currentUserId;

    this.userService.getMyUser().subscribe({
      next: (user) => {
        const profileCandidates = [
          user.id,
          user.keycloakId,
          user.username,
          user.email
        ]
          .map((value: unknown) => this.normalizeIdentity(value))
          .filter((value): value is string => !!value);

        profileCandidates.forEach((value) => this.currentUserIdentifiers.add(value));
        this.currentUserId = user.id || this.currentUserId;
        onReady?.();
        this.applyActiveStatusFilter();
      },
      error: () => {
        onReady?.();
        this.applyActiveStatusFilter();
      }
    });
  }

  initializeForms(): void {
    this.createForm = this.fb.group({
      ticketTypeId: ['', Validators.required],
      title: ['', [Validators.required, Validators.maxLength(50)]],
      summary: ['', [Validators.required, Validators.maxLength(2000)]],
      priority: ['MEDIUM', Validators.required]
    });

    this.createForm.get('ticketTypeId')?.valueChanges.subscribe((ticketTypeId) => {
      this.onTicketTypeChange(ticketTypeId);
    });
  }

  loadTickets(): void {
    if (!this.canManageTickets && !this.canOpenTickets) {
      this.allTickets = [];
      this.applyActiveStatusFilter();
      return;
    }

    this.ticketService.getAll(true).subscribe({
      next: (tickets) => {
        this.allTickets = tickets || [];
        this.applyActiveStatusFilter();
      }
    });
  }

  loadApprovalTickets(): void {
    if (!this.canManageTickets && !this.canOpenTickets) {
      this.approvalTickets = [];
      this.applyActiveStatusFilter();
      return;
    }

    this.ticketService.getForApproval(true).subscribe({
      next: (tickets) => {
        this.approvalTickets = tickets || [];
        if (this.activeStatusTab === 'approval' && this.approvalTickets.length === 0) {
          this.activeStatusTab = 'new';
        }
        this.applyActiveStatusFilter();
        if (this.gridApi) {
          this.gridApi.sizeColumnsToFit();
        }
      },
      error: () => {
        this.approvalTickets = [];
        if (this.activeStatusTab === 'approval') {
          this.activeStatusTab = 'new';
        }
        this.applyActiveStatusFilter();
      }
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
      // Navigate to ticket detail page when clicking on the row
      this.router.navigate(['/technical-support/tickets', event.data.ticketId]);
    }
  }

  openPreviewModal(ticket: Ticket): void {
    this.selectedTicket = ticket;
    this.previewCreatorUser = null;
    this.previewCreatorUserLoading = false;
    this.loadPreviewTicketType(ticket.ticketTypeId);
    if (ticket.reporter?.userId) {
      this.loadPreviewCreatorUser(ticket.reporter.userId);
    }
    this.showPreviewModal = true;
  }

  closePreviewModal(): void {
    this.showPreviewModal = false;
    this.selectedTicket = null;
    this.previewCreatorUser = null;
    this.previewCreatorUserLoading = false;
    this.previewTicketType = null;
  }

  private loadPreviewCreatorUser(userId: string): void {
    this.previewCreatorUserLoading = true;
    this.userService.getUserPreviewById(userId).subscribe({
      next: (user) => {
        this.previewCreatorUser = user;
        this.previewCreatorUserLoading = false;
      },
      error: () => {
        this.previewCreatorUser = null;
        this.previewCreatorUserLoading = false;
      }
    });
  }

  getPreviewCreatorDisplayName(): string {
    if (this.previewCreatorUser) {
      return this.previewCreatorUser.firstName && this.previewCreatorUser.lastName
        ? `${this.previewCreatorUser.firstName} ${this.previewCreatorUser.lastName}`
        : this.previewCreatorUser.username || this.previewCreatorUser.email || this.previewCreatorUser.id;
    }

    if (!this.selectedTicket) {
      return 'Loading...';
    }

    if (this.selectedTicket.reporter.displayName) {
      return this.selectedTicket.reporter.displayName;
    }

    return this.previewCreatorUserLoading ? 'Loading creator...' : this.selectedTicket.reporter.userId;
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

  get hasAdditionalFields(): boolean {
    return this.editableAttributeDefinitions.length > 0;
  }

  get editableAttributeDefinitions() {
    return this.selectedTicketType?.attributeDefinitions?.filter(attr => !attr.isPredefined) ?? [];
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

  getAttributeDropdownOptions(attribute: AttributeDefinition): SelectOption[] {
    return (attribute.options ?? []).map((option) => ({
      value: option,
      label: option
    }));
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
        userId: this.currentUserId || 'current-user-id',
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
