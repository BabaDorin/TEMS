import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnDestroy, OnInit, ViewChild, afterNextRender } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, Subscription, takeUntil } from 'rxjs';
import {
  AiSupportConversationDetail,
  AiSupportConversationMessage,
  AiSupportConversationSummary,
  AiSupportService
} from 'src/app/services/ai-support.service';

type ChatRole = 'user' | 'assistant';
type ChatState = 'done' | 'streaming';
type AiSupportTab = 'chat' | 'history';

interface ChatMessage {
  id: string;
  role: ChatRole;
  content: string;
  state: ChatState;
  createdAt?: string;
}

@Component({
  selector: 'app-ai-support',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-support.component.html',
  styleUrls: ['./ai-support.component.scss']
})
export class AiSupportComponent implements OnInit, OnDestroy {
  private static readonly historyPageSize = 10;
  @ViewChild('conversationViewport') private conversationViewport?: ElementRef<HTMLDivElement>;

  public activeTab: AiSupportTab = 'chat';
  public draftMessage = '';
  public isProcessing = false;
  public isLoadingConversation = false;
  public isLoadingConversations = false;
  public isDeletingConversationId: string | null = null;
  public activeConversationId: string | null = null;
  public activeConversationTitle = 'New conversation';
  public activeConversationCreatedAt: string | null = null;
  public historyError: string | null = null;
  public conversationError: string | null = null;
  public conversations: AiSupportConversationSummary[] = [];
  public visibleConversationCount = AiSupportComponent.historyPageSize;
  public messages: ChatMessage[] = [this.createWelcomeMessage()];

  private activeStream?: Subscription;
  private readonly destroy$ = new Subject<void>();

  constructor(
    private aiSupportService: AiSupportService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    afterNextRender(() => this.scrollToBottom());
  }

  ngOnInit(): void {
    this.loadConversationSummaries();

    this.route.queryParamMap
      .pipe(takeUntil(this.destroy$))
      .subscribe((params) => {
        const conversationId = params.get('conversationId');

        if (!conversationId) {
          if (this.activeConversationId) {
            this.resetComposerState();
          }

          return;
        }

        if (conversationId === this.activeConversationId && this.messages.length > 1) {
          return;
        }

        this.openConversation(conversationId, false);
      });
  }

  ngOnDestroy(): void {
    this.activeStream?.unsubscribe();
    this.destroy$.next();
    this.destroy$.complete();
  }

  sendMessage(): void {
    const message = this.draftMessage.trim();
    if (!message || this.isProcessing || this.isLoadingConversation) {
      return;
    }

    this.conversationError = null;
    this.activeTab = 'chat';
    this.messages = [
      ...this.messages,
      {
        id: this.createId(),
        role: 'user',
        content: message,
        state: 'done',
        createdAt: new Date().toISOString()
      },
      {
        id: this.createId(),
        role: 'assistant',
        content: '',
        state: 'streaming'
      }
    ];

    this.draftMessage = '';
    this.isProcessing = true;
    this.scrollToBottom();

    const assistantMessageId = this.messages[this.messages.length - 1].id;
    let accumulatedResponse = '';

    this.activeStream?.unsubscribe();
    this.activeStream = this.aiSupportService
      .streamResponse(message, this.activeConversationId)
      .subscribe({
        next: (event) => {
          if (event.type === 'conversation') {
            this.applyConversationSummary(event.conversation, true);
            return;
          }

          if (event.type === 'delta') {
            accumulatedResponse += event.content;
            this.patchMessage(assistantMessageId, {
              content: accumulatedResponse,
              state: 'streaming'
            });
            this.scrollToBottom();
            return;
          }

          if (event.type === 'done') {
            this.patchMessage(assistantMessageId, {
              content: event.content || accumulatedResponse,
              state: 'done',
              createdAt: new Date().toISOString()
            });

            if (event.conversation) {
              this.applyConversationSummary(event.conversation, true);
            } else {
              this.loadConversationSummaries(false);
            }

            this.isProcessing = false;
            this.scrollToBottom();
            return;
          }

          if (event.type === 'error') {
            this.patchMessage(assistantMessageId, {
              content: event.content,
              state: 'done',
              createdAt: new Date().toISOString()
            });
            this.isProcessing = false;
            this.conversationError = event.content;
            this.loadConversationSummaries(false);
            this.scrollToBottom();
          }
        },
        error: () => {
          const fallbackMessage = 'Sorry, I could not reach the AI support backend right now. Please try again in a moment.';
          this.patchMessage(assistantMessageId, {
            content: fallbackMessage,
            state: 'done',
            createdAt: new Date().toISOString()
          });
          this.isProcessing = false;
          this.conversationError = fallbackMessage;
          this.loadConversationSummaries(false);
          this.scrollToBottom();
        },
        complete: () => {
          this.isProcessing = false;
        }
      });
  }

  setActiveTab(tab: AiSupportTab): void {
    this.activeTab = tab;

    if (tab === 'history' && !this.conversations.length && !this.isLoadingConversations) {
      this.loadConversationSummaries();
    }
  }

  startNewChat(): void {
    if (this.isProcessing) {
      return;
    }

    this.resetComposerState();
    this.syncRouteConversationId(null);
  }

  openConversation(conversationId: string, syncRoute = true): void {
    if (!conversationId || this.isProcessing || this.isDeletingConversationId === conversationId) {
      return;
    }

    this.isLoadingConversation = true;
    this.conversationError = null;
    this.activeTab = 'chat';

    this.aiSupportService.getConversation(conversationId).subscribe({
      next: (conversation) => {
        this.isLoadingConversation = false;
        this.applyConversationDetail(conversation, syncRoute);
      },
      error: () => {
        this.isLoadingConversation = false;
        this.conversationError = 'That conversation could not be loaded. It may have been removed.';

        if (this.activeConversationId === conversationId) {
          this.resetComposerState();
        }

        this.loadConversationSummaries(false);

        if (syncRoute) {
          this.syncRouteConversationId(null);
        }
      }
    });
  }

  deleteConversation(conversation: AiSupportConversationSummary, event?: Event): void {
    event?.stopPropagation();

    if (this.isProcessing || this.isDeletingConversationId === conversation.conversationId) {
      return;
    }

    if (!confirm(`Delete "${conversation.title}"? This will permanently remove the saved conversation.`)) {
      return;
    }

    this.isDeletingConversationId = conversation.conversationId;
    this.aiSupportService.deleteConversation(conversation.conversationId).subscribe({
      next: () => {
        this.isDeletingConversationId = null;
        this.conversations = this.conversations.filter(item => item.conversationId !== conversation.conversationId);

        if (this.activeConversationId === conversation.conversationId) {
          this.resetComposerState();
          this.syncRouteConversationId(null);
        }
      },
      error: () => {
        this.isDeletingConversationId = null;
        this.historyError = 'The conversation could not be deleted right now.';
      }
    });
  }

  onComposerKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  trackByMessageId(_: number, message: ChatMessage): string {
    return message.id;
  }

  trackByConversationId(_: number, conversation: AiSupportConversationSummary): string {
    return conversation.conversationId;
  }

  formatConversationDate(value: string | null | undefined): string {
    if (!value) {
      return 'Unknown date';
    }

    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return 'Unknown date';
    }

    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: 'numeric',
      minute: '2-digit'
    }).format(date);
  }

  get visibleConversations(): AiSupportConversationSummary[] {
    return this.conversations.slice(0, this.visibleConversationCount);
  }

  get canLoadMoreConversations(): boolean {
    return this.visibleConversationCount < this.conversations.length;
  }

  loadMoreConversations(): void {
    this.visibleConversationCount = Math.min(
      this.visibleConversationCount + AiSupportComponent.historyPageSize,
      this.conversations.length
    );
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
      if (!paragraphLines.length) {
        return;
      }

      blocks.push(`<p>${paragraphLines.map((line) => this.applyInlineFormatting(line)).join('<br>')}</p>`);
      paragraphLines.length = 0;
    };

    const flushList = () => {
      if (!listItems.length) {
        return;
      }

      blocks.push(`<ul>${listItems.map((item) => `<li>${this.applyInlineFormatting(item)}</li>`).join('')}</ul>`);
      listItems.length = 0;
    };

    const flushCodeBlock = () => {
      if (!codeLines.length) {
        return;
      }

      blocks.push(`<pre><code>${codeLines.join('\n')}</code></pre>`);
      codeLines.length = 0;
    };

    for (const rawLine of lines) {
      const line = rawLine.trimEnd();

      if (line.startsWith('```')) {
        if (inCodeBlock) {
          flushCodeBlock();
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

      if (!line.trim()) {
        flushParagraph();
        flushList();
        continue;
      }

      const headingMatch = line.match(/^(#{1,3})\s+(.+)$/);
      if (headingMatch) {
        flushParagraph();
        flushList();
        const level = headingMatch[1].length;
        blocks.push(`<h${level}>${this.applyInlineFormatting(headingMatch[2])}</h${level}>`);
        continue;
      }

      const listMatch = line.match(/^\s*[-*]\s+(.+)$/);
      if (listMatch) {
        flushParagraph();
        listItems.push(listMatch[1]);
        continue;
      }

      if (listItems.length) {
        flushList();
      }

      paragraphLines.push(line);
    }

    if (inCodeBlock) {
      flushCodeBlock();
    }

    flushParagraph();
    flushList();

    return blocks.join('');
  }

  loadConversationSummaries(showLoading = true): void {
    if (showLoading) {
      this.isLoadingConversations = true;
    }

    this.historyError = null;
    this.aiSupportService.getConversations().subscribe({
      next: (conversations) => {
        this.isLoadingConversations = false;
        this.conversations = conversations;
        this.visibleConversationCount = Math.min(
          Math.max(this.visibleConversationCount, AiSupportComponent.historyPageSize),
          conversations.length || AiSupportComponent.historyPageSize
        );

        if (this.activeConversationId && !conversations.some(item => item.conversationId === this.activeConversationId)) {
          this.resetComposerState();
          this.syncRouteConversationId(null);
        }
      },
      error: () => {
        this.isLoadingConversations = false;
        this.historyError = 'Previous conversations could not be loaded right now.';
      }
    });
  }

  private applyConversationDetail(conversation: AiSupportConversationDetail, syncRoute: boolean): void {
    const normalizedConversation = this.normalizeConversationSummary(conversation);
    this.activeConversationId = conversation.conversationId;
    this.activeConversationTitle = normalizedConversation.title;
    this.activeConversationCreatedAt = conversation.createdAt;
    this.messages = conversation.messages.length
      ? conversation.messages.map(message => this.toChatMessage(message))
      : [this.createWelcomeMessage()];

    this.mergeConversationSummary(normalizedConversation);

    if (syncRoute) {
      this.syncRouteConversationId(conversation.conversationId);
    }

    this.scrollToBottom();
  }

  private applyConversationSummary(conversation: AiSupportConversationSummary, syncRoute: boolean): void {
    const normalizedConversation = this.normalizeConversationSummary(conversation);
    this.activeConversationId = normalizedConversation.conversationId;
    this.activeConversationTitle = normalizedConversation.title;
    this.activeConversationCreatedAt = normalizedConversation.createdAt;
    this.mergeConversationSummary(normalizedConversation);

    if (syncRoute) {
      this.syncRouteConversationId(normalizedConversation.conversationId);
    }
  }

  private mergeConversationSummary(conversation: AiSupportConversationSummary): void {
    const normalizedConversation = this.normalizeConversationSummary(conversation);
    const next = [...this.conversations];
    const existingIndex = next.findIndex(item => item.conversationId === normalizedConversation.conversationId);

    if (existingIndex >= 0) {
      next.splice(existingIndex, 1);
    }

    next.unshift(normalizedConversation);
    this.conversations = next;
    this.visibleConversationCount = Math.min(
      Math.max(this.visibleConversationCount, AiSupportComponent.historyPageSize),
      this.conversations.length
    );
  }

  private resetComposerState(): void {
    this.activeStream?.unsubscribe();
    this.isProcessing = false;
    this.isLoadingConversation = false;
    this.draftMessage = '';
    this.conversationError = null;
    this.activeConversationId = null;
    this.activeConversationTitle = 'New conversation';
    this.activeConversationCreatedAt = null;
    this.messages = [this.createWelcomeMessage()];
    this.scrollToBottom();
  }

  private patchMessage(messageId: string, patch: Partial<ChatMessage>): void {
    this.messages = this.messages.map((message) =>
      message.id === messageId ? { ...message, ...patch } : message
    );
  }

  private toChatMessage(message: AiSupportConversationMessage): ChatMessage {
    return {
      id: message.messageId,
      role: message.role,
      content: message.content,
      state: 'done',
      createdAt: message.createdAt
    };
  }

  private normalizeConversationSummary(conversation: AiSupportConversationSummary): AiSupportConversationSummary {
    const existingConversation = this.conversations.find(
      (item) => item.conversationId === conversation.conversationId
    );
    const normalizedTitle = this.resolveConversationTitle(conversation.title);
    const normalizedCreatedAt = this.resolveConversationCreatedAt(conversation.createdAt, existingConversation?.createdAt);
    const normalizedMessageCount = this.resolveConversationMessageCount(conversation.messageCount, existingConversation?.messageCount);

    return {
      ...conversation,
      title: normalizedTitle,
      createdAt: normalizedCreatedAt,
      updatedAt: conversation.updatedAt || existingConversation?.updatedAt || normalizedCreatedAt,
      messageCount: normalizedMessageCount
    };
  }

  private resolveConversationTitle(title: string | null | undefined): string {
    const trimmedTitle = (title || '').trim();
    if (trimmedTitle && trimmedTitle.toLowerCase() !== 'conversation') {
      return trimmedTitle;
    }

    const firstUserMessage = this.messages.find((message) => message.role === 'user' && message.content.trim().length > 0);
    if (!firstUserMessage) {
      return 'Conversation';
    }

    const compactTitle = firstUserMessage.content.replace(/\s+/g, ' ').trim();
    return compactTitle.length > 80 ? `${compactTitle.slice(0, 77).trimEnd()}...` : compactTitle;
  }

  private resolveConversationCreatedAt(
    createdAt: string | null | undefined,
    existingCreatedAt?: string | null
  ): string {
    if (createdAt && !Number.isNaN(new Date(createdAt).getTime())) {
      return createdAt;
    }

    if (existingCreatedAt && !Number.isNaN(new Date(existingCreatedAt).getTime())) {
      return existingCreatedAt;
    }

    const firstMessageDate = this.messages.find((message) => !!message.createdAt)?.createdAt;
    if (firstMessageDate && !Number.isNaN(new Date(firstMessageDate).getTime())) {
      return firstMessageDate;
    }

    return new Date().toISOString();
  }

  private resolveConversationMessageCount(
    messageCount: number | null | undefined,
    existingMessageCount?: number | null
  ): number {
    if (typeof messageCount === 'number' && messageCount > 0) {
      return messageCount;
    }

    if (typeof existingMessageCount === 'number' && existingMessageCount > 0) {
      return existingMessageCount;
    }

    const persistedMessagesCount = this.messages.filter((message) => message.state === 'done' && message.content.trim().length > 0).length;
    return persistedMessagesCount > 0 ? persistedMessagesCount : 0;
  }

  private syncRouteConversationId(conversationId: string | null): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        conversationId: conversationId || null
      },
      queryParamsHandling: 'merge',
      replaceUrl: true
    });
  }

  private escapeHtml(value: string): string {
    return value
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');
  }

  private applyInlineFormatting(value: string): string {
    return value
      .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
      .replace(/`([^`]+)`/g, '<code>$1</code>')
      .replace(/\[(.+?)\]\((.+?)\)/g, '<a href="$2" target="_blank" rel="noopener noreferrer">$1</a>')
      .replace(/\*(.+?)\*/g, '<em>$1</em>');
  }

  private scrollToBottom(): void {
    window.setTimeout(() => {
      this.conversationViewport?.nativeElement.scrollTo({
        top: this.conversationViewport.nativeElement.scrollHeight,
        behavior: 'smooth'
      });
    }, 0);
  }

  private createWelcomeMessage(): ChatMessage {
    return {
      id: this.createId(),
      role: 'assistant',
      content: "Hey, I'm your AI IT agent. Ask me anything about equipment management or technical support, and I’ll help you troubleshoot or point you in the right direction.",
      state: 'done'
    };
  }

  private createId(): string {
    return globalThis.crypto?.randomUUID?.() ?? `${Date.now()}-${Math.random().toString(16).slice(2)}`;
  }
}
