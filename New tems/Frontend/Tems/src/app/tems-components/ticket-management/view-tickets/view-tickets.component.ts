import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { TicketService } from 'src/app/services/ticket.service';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { Ticket, CreateTicketRequest, TicketMessage, AddMessageRequest } from 'src/app/models/ticket/ticket.model';
import { TicketType } from 'src/app/models/ticket/ticket-type.model';

@Component({
  selector: 'app-view-tickets',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AgGridAngular
  ],
  templateUrl: './view-tickets.component.html',
  styleUrls: ['./view-tickets.component.scss']
})
export class ViewTicketsComponent implements OnInit {
  rowData: Ticket[] = [];
  ticketTypes: TicketType[] = [];
  gridApi!: GridApi;
  showCreateModal = false;
  showDetailsModal = false;
  selectedTicket: Ticket | null = null;
  messages: TicketMessage[] = [];
  createForm!: FormGroup;
  messageForm!: FormGroup;
  isSubmitting = false;
  isSubmittingMessage = false;
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
      flex: 0.7,
      minWidth: 100,
      cellRenderer: (params: any) => {
        return `
          <button class="action-delete-btn px-2 py-1 text-red-600 hover:text-red-800 text-sm">
            Delete
          </button>
        `;
      },
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target.classList.contains('action-delete-btn')) {
          this.deleteTicket(params.data.ticketId);
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
    private fb: FormBuilder
  ) {
    this.initializeForms();
  }

  ngOnInit(): void {
    // Initialize with empty arrays to ensure grid displays
    this.rowData = [];
    this.ticketTypes = [];
    // Try to load data, but grid will show even if this fails
    setTimeout(() => {
      this.loadTickets();
      this.loadTicketTypes();
    }, 100);
  }

  initializeForms(): void {
    this.createForm = this.fb.group({
      ticketTypeId: ['', Validators.required],
      summary: ['', Validators.required],
      priority: ['MEDIUM', Validators.required],
      channelSource: ['WEB', Validators.required]
    });

    this.messageForm = this.fb.group({
      content: ['', Validators.required],
      isInternalNote: [false]
    });
  }

  loadTickets(): void {
    this.ticketService.getAll().subscribe({
      next: (data) => {
        this.rowData = data || [];
        if (this.gridApi) {
          this.gridApi.sizeColumnsToFit();
        }
      },
      error: (error) => {
        console.error('Error loading tickets:', error);
        this.rowData = [];
        if (this.gridApi) {
          this.gridApi.sizeColumnsToFit();
        }
      }
    });
  }

  loadTicketTypes(): void {
    this.ticketTypeService.getAll().subscribe({
      next: (data) => {
        this.ticketTypes = data?.filter(t => t.isActive) || [];
      },
      error: (error) => {
        console.error('Error loading ticket types:', error);
        this.ticketTypes = [];
      }
    });
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
    if (!target.classList.contains('action-delete-btn')) {
      this.selectedTicket = event.data;
      this.loadMessages(event.data.ticketId);
      this.showDetailsModal = true;
    }
  }

  loadMessages(ticketId: string): void {
    this.ticketService.getMessages(ticketId).subscribe({
      next: (conversation) => {
        this.messages = conversation.messages || [];
      },
      error: (error) => {
        console.error('Error loading messages:', error);
        this.messages = [];
      }
    });
  }

  openCreateModal(): void {
    this.showCreateModal = true;
    this.createForm.reset({
      priority: 'MEDIUM',
      channelSource: 'WEB'
    });
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
    this.createForm.reset();
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedTicket = null;
    this.messages = [];
    this.messageForm.reset({ isInternalNote: false });
  }

  onSubmit(): void {
    if (this.createForm.invalid || this.isSubmitting) {
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
      attributes: {}
    };

    this.ticketService.create(request).subscribe({
      next: (response) => {
        console.log('Ticket created:', response);
        this.loadTickets();
        this.closeCreateModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error creating ticket:', error);
        this.isSubmitting = false;
      }
    });
  }

  onSubmitMessage(): void {
    if (this.messageForm.invalid || this.isSubmittingMessage || !this.selectedTicket) {
      return;
    }

    this.isSubmittingMessage = true;
    const formValue = this.messageForm.value;

    const request: AddMessageRequest = {
      senderType: 'USER',
      senderId: 'current-user-id', // TODO: Get from auth service
      content: formValue.content,
      isInternalNote: formValue.isInternalNote
    };

    this.ticketService.addMessage(this.selectedTicket.ticketId, request).subscribe({
      next: () => {
        console.log('Message added');
        this.loadMessages(this.selectedTicket!.ticketId);
        this.messageForm.reset({ isInternalNote: false });
        this.isSubmittingMessage = false;
      },
      error: (error) => {
        console.error('Error adding message:', error);
        this.isSubmittingMessage = false;
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
        this.loadTickets();
      },
      error: (error) => {
        console.error('Error deleting ticket:', error);
      }
    });
  }
}
