import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DialogService } from 'src/app/services/dialog.service';
import { TicketService } from 'src/app/services/ticket.service';
import { TicketTypeService } from 'src/app/services/ticket-type.service';
import { AuthService } from 'src/app/services/auth.service';
import { TokenService } from 'src/app/services/token.service';
import { UserService } from 'src/app/services/user.service';
import { Ticket, TicketMessage, AddMessageRequest, UpdateTicketRequest } from 'src/app/models/ticket/ticket.model';
import { TicketType, WorkflowState } from 'src/app/models/ticket/ticket-type.model';
import { UserDto } from 'src/app/models/user/user-management.model';
import { ViewUserModalComponent } from '../../admin/user-management/view-user-modal/view-user-modal.component';

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
  creatorUser: UserDto | null = null;
  creatorUserLoading = false;
  canManageTickets = false;
  statusDraft = '';
  isSavingStatus = false;
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
  private aiSummaryPollHandle: ReturnType<typeof setInterval> | null = null;
  private aiSummaryPollTicketId: string | null = null;
  private aiSummaryPollExpiresAt = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    private dialogService: DialogService,
    private ticketService: TicketService,
    private ticketTypeService: TicketTypeService,
    private authService: AuthService,
    private userService: UserService,
    private tokenService: TokenService
  ) {}

  ngOnInit(): void {
    this.initializeCurrentUserContext();
    this.canManageTickets = this.tokenService.canManageTickets();
    const ticketId = this.route.snapshot.paramMap.get('id');
    if (ticketId) {
      this.loadTicket(ticketId);
      this.loadMessages(ticketId);
    }
  }

  ngOnDestroy(): void {
    this.clearAiSummaryPolling();
  }

  loadTicket(ticketId: string): void {
    this.isLoadingTicket = true;
    this.ticketService.getById(ticketId).subscribe({
      next: (ticket) => {
        this.ticket = ticket;
        this.creatorUser = null;
        this.creatorUserLoading = false;
        this.statusDraft = this.getTicketStatusValue(ticket.currentStateId);
        this.isLoadingTicket = false;
        if (ticket.reporter?.userId) {
          this.loadCreatorUser(ticket.reporter.userId);
        }
        if (ticket.ticketTypeId) {
          this.loadTicketType(ticket.ticketTypeId);
        }
        this.maybeStartAiSummaryPolling();
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
        if (this.ticket) {
          this.statusDraft = this.getTicketStatusValue(this.ticket.currentStateId);
        }
        this.maybeStartAiSummaryPolling();
      },
      error: (error) => {
        console.error('Error loading ticket type:', error);
      }
    });
  }

  loadCreatorUser(userId: string): void {
    this.creatorUserLoading = true;
    this.userService.getUserPreviewById(userId).subscribe({
      next: (user) => {
        this.creatorUser = user;
        this.creatorUserLoading = false;
      },
      error: (error) => {
        console.error('Error loading creator user:', error);
        this.creatorUser = null;
        this.creatorUserLoading = false;
      }
    });
  }

  getTicketStatusOptions(): { value: string; label: string }[] {
    const states = this.ticketType?.workflowConfig?.states || [];
    const options: { value: string; label: string; order: number }[] = [];

    const pushOption = (state: WorkflowState, label: string, order: number) => {
      options.push({ value: state.id, label, order });
    };

    const managedStates = this.ticketType?.workflowConfig?.states || [];

    const findState = (aliases: string[]) =>
      managedStates.find((state) => this.isMatchingManagedState(state, aliases)) || null;

    const newState = findState(['new', 'open', 'state_new']);
    if (newState) pushOption(newState, 'New', 1);

    const progressState = findState(['in-progress', 'in_progress', 'state_in_progress', 'state_wip', 'wip', 'progress']);
    if (progressState) pushOption(progressState, 'In progress', 2);

    const closedState = findState(['closed', 'state_closed']);
    if (closedState) pushOption(closedState, 'Closed', 3);

    if (options.length === 0) {
      states.forEach((state, index) => pushOption(state, this.getHumanizedStateLabel(state.id || state.label), index + 1));
    }

    return options
      .sort((a, b) => a.order - b.order)
      .map(({ order, ...option }) => option);
  }

  getTicketStatusLabel(stateId: string): string {
    return this.getHumanizedStateLabel(stateId);
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

  getCurrentStatusLabel(): string {
    return this.getTicketStatusLabel(this.ticket?.currentStateId || '');
  }

  getCurrentStatusValue(): string {
    return this.getTicketStatusValue(this.ticket?.currentStateId || '');
  }

  onStatusDraftChanged(nextValue: string): void {
    this.statusDraft = nextValue;
    this.saveTicketStatus(nextValue);
  }

  saveTicketStatus(nextValue?: string): void {
    if (!this.ticket || !this.canManageTickets) {
      return;
    }

    const currentValue = this.getCurrentStatusValue();
    const targetValue = nextValue || this.statusDraft;
    if (!targetValue || currentValue === targetValue || this.isSavingStatus) {
      return;
    }

    this.isSavingStatus = true;
    const request: UpdateTicketRequest = {
      summary: this.ticket.summary,
      currentStateId: targetValue,
      priority: this.ticket.priority,
      assigneeId: this.ticket.assigneeId,
      attributes: this.ticket.attributes
    };

    this.ticketService.update(this.ticket.ticketId, request).subscribe({
      next: () => {
        this.ticket = {
          ...this.ticket!,
          currentStateId: targetValue,
          updatedAt: new Date().toISOString()
        };
        this.statusDraft = targetValue;
        this.isSavingStatus = false;
      },
      error: (error) => {
        console.error('Error updating ticket status:', error);
        this.statusDraft = currentValue;
        this.isSavingStatus = false;
      }
    });
  }

  private getTicketStatusValue(stateId: string): string {
    const options = this.getTicketStatusOptions();
    const targetGroup = this.getManagedStatusGroup(stateId);
    if (targetGroup) {
      const managed = options.find((option) => this.getManagedStatusGroup(option.value) === targetGroup);
      if (managed) {
        return managed.value;
      }
    }

    const normalized = this.normalizeStateId(stateId);
    const exact = options.find((option) => this.normalizeStateId(option.value) === normalized);
    return exact?.value || options.find((option) => this.normalizeStateId(option.label) === normalized)?.value || stateId;
  }

  private isMatchingManagedState(state: WorkflowState, aliases: string[]): boolean {
    const normalizedId = this.getManagedStatusGroup(state.id) || this.normalizeStateId(state.id);
    const normalizedLabel = this.getManagedStatusGroup(state.label) || this.normalizeStateId(state.label);
    return aliases.some((alias) => {
      const normalizedAlias = this.getManagedStatusGroup(alias) || this.normalizeStateId(alias);
      return normalizedAlias === normalizedId || normalizedAlias === normalizedLabel;
    });
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
    if (!normalized) {
      return null;
    }

    if (['new', 'open', 'state-new'].includes(normalized)) {
      return 'new';
    }
    if (['in-progress', 'state-in-progress', 'state-wip', 'wip', 'progress'].includes(normalized)) {
      return 'in-progress';
    }
    if (['closed', 'state-closed'].includes(normalized)) {
      return 'closed';
    }

    return null;
  }

  private getHumanizedStateLabel(value: string): string {
    const managed = this.getManagedStatusGroup(value);
    if (managed === 'new') {
      return 'New';
    }
    if (managed === 'in-progress') {
      return 'In progress';
    }
    if (managed === 'closed') {
      return 'Closed';
    }

    const normalized = this.normalizeStateId(value);
    if (!normalized) {
      return '';
    }

    return normalized
      .split('-')
      .filter((part) => !!part)
      .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
      .join(' ');
  }

  getCreatorDisplayName(): string {
    if (!this.creatorUser) {
      return '';
    }

    const parts = [this.creatorUser.firstName, this.creatorUser.lastName].filter((part) => !!part && part.trim().length > 0);
    return parts.length > 0 ? parts.join(' ') : this.creatorUser.username;
  }

  openCreatorPreview(): void {
    if (!this.creatorUser) {
      return;
    }

    this.dialogService.openDialog(
      ViewUserModalComponent,
      [{ label: 'user', value: this.creatorUser }],
      () => {}
    );
  }

  private maybeStartAiSummaryPolling(): void {
    if (!this.ticket || this.ticket.aiSummary?.trim()) {
      this.clearAiSummaryPolling();
      return;
    }

    if (!this.shouldPollForAiSummary()) {
      this.clearAiSummaryPolling();
      return;
    }

    if (this.aiSummaryPollTicketId === this.ticket.ticketId) {
      return;
    }

    this.clearAiSummaryPolling();
    this.aiSummaryPollTicketId = this.ticket.ticketId;
    this.aiSummaryPollExpiresAt = Date.now() + 30 * 60 * 1000;

    this.aiSummaryPollHandle = setInterval(() => {
      if (!this.ticket || !this.aiSummaryPollTicketId || Date.now() > this.aiSummaryPollExpiresAt) {
        this.clearAiSummaryPolling();
        return;
      }

      this.ticketService.getById(this.aiSummaryPollTicketId).subscribe({
        next: (updatedTicket) => {
          this.ticket = updatedTicket;
          if (updatedTicket.aiSummary?.trim()) {
            this.clearAiSummaryPolling();
          }
        },
        error: (error) => {
          console.error('Error polling ticket AI summary:', error);
        }
      });
    }, 5000);
  }

  private shouldPollForAiSummary(): boolean {
    if (!this.ticketType) {
      return false;
    }

    const name = `${this.ticketType.name || ''} ${this.ticketType.description || ''} ${this.ticketType.ticketTypeId || ''}`.toLowerCase();
    const isHardwareIssue = name.includes('hardware issue') || name.includes('hardware_issue') || name.includes('hardware');
    const isNetworkIssue = name.includes('network issue') || name.includes('network_issue') || name.includes('network');
    const isIncident = (this.ticketType.itilCategory || '').toLowerCase() === 'incident';

    return isIncident && (isHardwareIssue || isNetworkIssue);
  }

  private clearAiSummaryPolling(): void {
    if (this.aiSummaryPollHandle) {
      clearInterval(this.aiSummaryPollHandle);
      this.aiSummaryPollHandle = null;
    }
    this.aiSummaryPollTicketId = null;
    this.aiSummaryPollExpiresAt = 0;
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
    if (window.history.length > 1) {
      this.location.back();
      return;
    }

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

  renderMarkdown(text: string): string {
    if (!text) {
      return '';
    }

    const lines = this.escapeHtml(text).split(/\r?\n/);
    const blocks: string[] = [];
    const paragraphLines: string[] = [];
    const listItems: string[] = [];
    const codeLines: string[] = [];
    let inCodeBlock = false;

    const flushParagraph = () => {
      if (!paragraphLines.length) return;
      blocks.push(`<p>${paragraphLines.map((line) => this.applyInlineFormatting(line)).join('<br>')}</p>`);
      paragraphLines.length = 0;
    };

    const flushList = () => {
      if (!listItems.length) return;
      blocks.push(`<ul>${listItems.map((item) => `<li>${this.applyInlineFormatting(item)}</li>`).join('')}</ul>`);
      listItems.length = 0;
    };

    const flushCode = () => {
      if (!codeLines.length) return;
      blocks.push(`<pre><code>${codeLines.join('\n')}</code></pre>`);
      codeLines.length = 0;
    };

    for (const line of lines) {
      const trimmed = line.trim();
      if (trimmed.startsWith('```')) {
        if (inCodeBlock) {
          flushCode();
        } else {
          flushParagraph();
          flushList();
        }
        inCodeBlock = !inCodeBlock;
        continue;
      }

      if (inCodeBlock) {
        codeLines.push(line);
        continue;
      }

      const listMatch = trimmed.match(/^[-*]\s+(.*)$/);
      if (listMatch) {
        flushParagraph();
        listItems.push(listMatch[1]);
        continue;
      }

      if (!trimmed) {
        flushParagraph();
        flushList();
        continue;
      }

      flushList();
      paragraphLines.push(trimmed);
    }

    flushParagraph();
    flushList();
    flushCode();

    return blocks.join('');
  }

  private escapeHtml(value: string): string {
    const map: Record<string, string> = {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '"': '&quot;',
      "'": '&#39;'
    };

    return value.replace(/[&<>"']/g, (char) => map[char]);
  }

  private applyInlineFormatting(value: string): string {
    return value
      .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
      .replace(/\*(.+?)\*/g, '<em>$1</em>')
      .replace(/`(.+?)`/g, '<code>$1</code>')
      .replace(/\[(.+?)\]\((.+?)\)/g, '<a href="$2" target="_blank" rel="noopener noreferrer">$1</a>');
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
