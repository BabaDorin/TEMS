import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TicketService } from 'src/app/services/ticket.service';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { AuthService } from 'src/app/services/auth.service';
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
  activeMainTab: 'details' | 'chat' = 'details';
  activeChatTab: 'messages' | 'notes' = 'messages';
  isLoadingTicket = true;
  isLoadingMessages = false;
  isSendingMessage = false;
  editingMessageId: string | null = null;
  editDraftContent = '';
  isSavingEdit = false;
  deletingMessageId: string | null = null;
  currentUserId = 'current-user';
  currentUserLabel = 'You';
  private currentUserIdentifiers = new Set<string>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private ticketService: TicketService,
    private ticketTypeService: TicketTypeService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.initializeCurrentUserContext();
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
        this.messages = allMessages
          .filter(m => !m.isInternalNote)
          .map((m) => this.normalizeMessage(m))
          .sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
        this.internalNotes = allMessages
          .filter(m => m.isInternalNote)
          .map((m) => this.normalizeMessage(m))
          .sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
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

    const isInternalNote = this.activeChatTab === 'notes';
    const content = this.newMessageContent.trim();
    this.isSendingMessage = true;
    this.newMessageContent = '';

    const optimisticMessage: TicketMessage = this.normalizeMessage({
      senderType: 'AGENT',
      senderId: this.currentUserId,
      timestamp: new Date().toISOString(),
      content,
      isInternalNote
    } as TicketMessage);

    this.addMessageToActiveCollection(optimisticMessage);

    const request: AddMessageRequest = {
      senderType: 'AGENT',
      senderId: this.currentUserId,
      content,
      isInternalNote: isInternalNote
    };

    this.ticketService.addMessage(this.ticket.ticketId, request).subscribe({
      next: (response) => {
        const createdMessage = response?.message ? this.normalizeMessage(response.message) : optimisticMessage;
        this.replaceMessageInCollection(optimisticMessage, createdMessage);
        this.isSendingMessage = false;
      },
      error: (error) => {
        console.error('Error sending message:', error);
        this.removeMessageFromCollection(optimisticMessage);
        this.newMessageContent = content;
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
    return this.activeChatTab === 'messages' ? this.messages : this.internalNotes;
  }

  isOwnMessage(message: TicketMessage): boolean {
    const senderId = this.normalizeIdentity(message.senderId);
    return !!senderId && this.currentUserIdentifiers.has(senderId);
  }

  getSenderLabel(message: TicketMessage): string {
    if (this.isOwnMessage(message)) return this.currentUserLabel;
    if (message.senderType === 'AI_SYSTEM') return 'System';
    return message.senderId || 'Unknown';
  }

  getSenderInitial(message: TicketMessage): string {
    if (this.isOwnMessage(message)) return 'Y';
    const label = this.getSenderLabel(message).trim();
    return label.length > 0 ? label.charAt(0).toUpperCase() : '?';
  }

  trackByMessageId(_: number, message: TicketMessage): string {
    return message.messageId || this.buildMessageId(message);
  }

  startEditingMessage(message: TicketMessage): void {
    if (!message.messageId || !this.isOwnMessage(message)) return;
    this.editingMessageId = message.messageId;
    this.editDraftContent = message.content;
  }

  cancelEditingMessage(): void {
    this.editingMessageId = null;
    this.editDraftContent = '';
  }

  saveEditedMessage(message: TicketMessage): void {
    if (!this.ticket || !message.messageId || !this.isOwnMessage(message)) return;

    const newContent = this.editDraftContent.trim();
    if (!newContent) return;
    if (newContent === message.content) {
      this.cancelEditingMessage();
      return;
    }

    this.isSavingEdit = true;
    const previousContent = message.content;

    this.applyLocalMessagePatch(message.messageId, {
      content: newContent,
      editedAt: new Date().toISOString()
    });

    this.ticketService.editMessage(this.ticket.ticketId, message.messageId, newContent).subscribe({
      next: (response) => {
        const updated = response.message ? this.normalizeMessage(response.message) : null;
        if (updated) {
          this.applyLocalMessageReplace(message.messageId!, updated);
        }
        this.isSavingEdit = false;
        this.cancelEditingMessage();
      },
      error: (error) => {
        console.error('Error editing message:', error);
        this.applyLocalMessagePatch(message.messageId!, {
          content: previousContent
        });
        this.isSavingEdit = false;
      }
    });
  }

  deleteMessage(message: TicketMessage): void {
    if (!this.ticket || !message.messageId || !this.isOwnMessage(message)) return;

    this.deletingMessageId = message.messageId;
    const removed = this.removeMessageById(message.messageId, message.isInternalNote);

    this.ticketService.deleteMessage(this.ticket.ticketId, message.messageId).subscribe({
      next: () => {
        this.deletingMessageId = null;
        if (this.editingMessageId === message.messageId) {
          this.cancelEditingMessage();
        }
      },
      error: (error) => {
        console.error('Error deleting message:', error);
        this.deletingMessageId = null;
        if (removed) {
          this.addMessageToActiveCollection(removed);
        }
      }
    });
  }

  private initializeCurrentUserContext(): void {
    const claims = this.authService.getIdentityClaims() as any;
    const candidates = [
      claims?.sub,
      claims?.preferred_username,
      claims?.email,
      claims?.name
    ]
      .map((value: unknown) => this.normalizeIdentity(value))
      .filter((value): value is string => !!value);

    this.currentUserIdentifiers = new Set(candidates);
    this.currentUserId = candidates[0] ?? 'current-user';
    this.currentUserLabel = claims?.preferred_username || claims?.name || claims?.email || 'You';
  }

  private normalizeIdentity(value: unknown): string | null {
    if (typeof value !== 'string') return null;
    const normalized = value.trim().toLowerCase();
    return normalized.length > 0 ? normalized : null;
  }

  private normalizeMessage(message: TicketMessage): TicketMessage {
    return {
      ...message,
      messageId: message.messageId || this.buildMessageId(message),
      editedAt: message.editedAt ?? null
    };
  }

  private buildMessageId(message: Pick<TicketMessage, 'senderId' | 'timestamp' | 'content' | 'isInternalNote'>): string {
    const timestamp = new Date(message.timestamp).getTime() || Date.now();
    return `${message.senderId}|${timestamp}|${message.isInternalNote ? 'note' : 'msg'}|${message.content}`;
  }

  private addMessageToActiveCollection(message: TicketMessage): void {
    if (message.isInternalNote) {
      this.internalNotes = [...this.internalNotes, message];
      return;
    }
    this.messages = [...this.messages, message];
  }

  private replaceMessageInCollection(original: TicketMessage, replacement: TicketMessage): void {
    const replace = (items: TicketMessage[]) =>
      items.map((item) => (item.messageId === original.messageId ? replacement : item));

    if (original.isInternalNote) {
      this.internalNotes = replace(this.internalNotes);
      return;
    }
    this.messages = replace(this.messages);
  }

  private removeMessageFromCollection(message: TicketMessage): void {
    const remove = (items: TicketMessage[]) =>
      items.filter((item) => item.messageId !== message.messageId);

    if (message.isInternalNote) {
      this.internalNotes = remove(this.internalNotes);
      return;
    }
    this.messages = remove(this.messages);
  }

  private applyLocalMessagePatch(messageId: string, patch: Partial<TicketMessage>): void {
    const apply = (items: TicketMessage[]) =>
      items.map((item) => (item.messageId === messageId ? { ...item, ...patch } : item));

    this.messages = apply(this.messages);
    this.internalNotes = apply(this.internalNotes);
  }

  private applyLocalMessageReplace(messageId: string, updated: TicketMessage): void {
    const replace = (items: TicketMessage[]) =>
      items.map((item) => (item.messageId === messageId ? updated : item));

    this.messages = replace(this.messages);
    this.internalNotes = replace(this.internalNotes);
  }

  private removeMessageById(messageId: string, isInternalNote: boolean): TicketMessage | null {
    const source = isInternalNote ? this.internalNotes : this.messages;
    const existing = source.find((m) => m.messageId === messageId) || null;
    if (!existing) return null;

    if (isInternalNote) {
      this.internalNotes = this.internalNotes.filter((m) => m.messageId !== messageId);
    } else {
      this.messages = this.messages.filter((m) => m.messageId !== messageId);
    }

    return existing;
  }
}
