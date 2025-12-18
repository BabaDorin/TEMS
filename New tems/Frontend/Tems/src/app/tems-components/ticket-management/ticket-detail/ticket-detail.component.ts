import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TicketService } from 'src/app/services/ticket.service';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { Ticket, TicketMessage, AddMessageRequest } from 'src/app/models/ticket/ticket.model';
import { TicketType } from 'src/app/models/ticket/ticket-type.model';

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket-detail.component.html',
  styleUrls: ['./ticket-detail.component.scss']
})
export class TicketDetailComponent implements OnInit {
  ticket: Ticket | null = null;
  ticketType: TicketType | null = null;
  messages: TicketMessage[] = [];
  internalNotes: TicketMessage[] = [];
  newMessageContent = '';
  activeTab: 'messages' | 'notes' = 'messages';
  isLoadingTicket = true;
  isLoadingMessages = false;
  isSendingMessage = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private ticketService: TicketService,
    private ticketTypeService: TicketTypeService
  ) {}

  ngOnInit(): void {
    const ticketId = this.route.snapshot.paramMap.get('id');
    if (ticketId) {
      this.loadTicket(ticketId);
      this.loadMessages(ticketId);
    }
  }

  loadTicket(ticketId: string): void {
    this.isLoadingTicket = true;
    this.ticketService.getById(ticketId).subscribe({
      next: (ticket) => {
        this.ticket = ticket;
        this.isLoadingTicket = false;
        if (ticket.ticketTypeId) {
          this.loadTicketType(ticket.ticketTypeId);
        }
      },
      error: (error) => {
        console.error('Error loading ticket:', error);
        this.isLoadingTicket = false;
      }
    });
  }

  loadTicketType(ticketTypeId: string): void {
    this.ticketTypeService.getById(ticketTypeId).subscribe({
      next: (ticketType) => {
        this.ticketType = ticketType;
      },
      error: (error) => {
        console.error('Error loading ticket type:', error);
      }
    });
  }

  loadMessages(ticketId: string): void {
    this.isLoadingMessages = true;
    this.ticketService.getMessages(ticketId).subscribe({
      next: (conversation) => {
        const allMessages = conversation.messages || [];
        this.messages = allMessages.filter(m => !m.isInternalNote);
        this.internalNotes = allMessages.filter(m => m.isInternalNote);
        this.isLoadingMessages = false;
      },
      error: (error) => {
        console.error('Error loading messages:', error);
        this.isLoadingMessages = false;
      }
    });
  }

  sendMessage(): void {
    if (!this.ticket || !this.newMessageContent.trim()) {
      return;
    }

    const isInternalNote = this.activeTab === 'notes';
    this.isSendingMessage = true;

    const request: AddMessageRequest = {
      senderType: 'AGENT',
      senderId: 'current-user',
      content: this.newMessageContent.trim(),
      isInternalNote: isInternalNote
    };

    this.ticketService.addMessage(this.ticket.ticketId, request).subscribe({
      next: () => {
        this.newMessageContent = '';
        this.loadMessages(this.ticket!.ticketId);
        this.isSendingMessage = false;
      },
      error: (error) => {
        console.error('Error sending message:', error);
        this.isSendingMessage = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/technical-support/tickets']);
  }

  getAttributeKeys(): string[] {
    return Object.keys(this.ticket?.attributes || {});
  }

  getAttributeLabel(key: string): string {
    const attr = this.ticketType?.attributeDefinitions?.find(a => a.key === key);
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

  getDisplayMessages(): TicketMessage[] {
    return this.activeTab === 'messages' ? this.messages : this.internalNotes;
  }

  isUserMessage(message: TicketMessage): boolean {
    // For testing, we'll consider messages from the reporter as user messages
    return message.senderId === this.ticket?.reporter?.userId;
  }
}
