import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Ticket } from 'src/app/models/ticket/ticket.model';
import { TicketType } from 'src/app/models/ticket/ticket-type.model';
import { TicketService } from 'src/app/services/ticket.service';
import { TicketTypeService } from 'src/app/services/ticket-type.service';

export interface TicketPreviewModalData {
  ticketId: string;
}

@Component({
  selector: 'app-ticket-preview-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './ticket-preview-modal.component.html'
})
export class TicketPreviewModalComponent implements OnInit {
  ticket: Ticket | null = null;
  ticketType: TicketType | null = null;
  loading = true;
  error: string | null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: TicketPreviewModalData,
    private dialogRef: MatDialogRef<TicketPreviewModalComponent>,
    private ticketService: TicketService,
    private ticketTypeService: TicketTypeService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.ticketService.getById(this.data.ticketId).subscribe({
      next: (ticket) => {
        this.ticket = ticket;
        this.loading = false;

        if (ticket.ticketTypeId) {
          this.ticketTypeService.getById(ticket.ticketTypeId).subscribe({
            next: (ticketType) => {
              this.ticketType = ticketType;
            }
          });
        }
      },
      error: () => {
        this.error = 'Failed to load ticket preview';
        this.loading = false;
      }
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  openFullTicket(): void {
    if (!this.ticket) {
      return;
    }

    this.dialogRef.close();
    this.router.navigate(['/technical-support/tickets', this.ticket.ticketId]);
  }

  getTitle(): string {
    return this.ticket?.title || this.ticket?.summary || 'Ticket';
  }
}
