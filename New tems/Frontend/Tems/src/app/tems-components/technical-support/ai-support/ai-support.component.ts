import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnDestroy, ViewChild, afterNextRender } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AiSupportService } from 'src/app/services/ai-support.service';

type ChatRole = 'user' | 'assistant';
type ChatState = 'done' | 'streaming';

interface ChatMessage {
  id: string;
  role: ChatRole;
  content: string;
  state: ChatState;
}

@Component({
  selector: 'app-ai-support',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-support.component.html',
  styleUrls: ['./ai-support.component.scss']
})
export class AiSupportComponent implements OnDestroy {
  @ViewChild('conversationViewport') private conversationViewport?: ElementRef<HTMLDivElement>;

  public draftMessage = '';
  public isProcessing = false;
  public messages: ChatMessage[] = [this.createWelcomeMessage()];

  private activeStream?: Subscription;

  constructor(private aiSupportService: AiSupportService) {
    afterNextRender(() => this.scrollToBottom());
  }

  ngOnDestroy(): void {
    this.activeStream?.unsubscribe();
  }

  sendMessage(): void {
    const message = this.draftMessage.trim();
    if (!message || this.isProcessing) {
      return;
    }

    this.messages = [
      ...this.messages,
      {
        id: this.createId(),
        role: 'user',
        content: message,
        state: 'done'
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
    this.activeStream = this.aiSupportService.streamResponse(message).subscribe({
      next: (event) => {
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
            state: 'done'
          });
          this.isProcessing = false;
          this.scrollToBottom();
          return;
        }

        if (event.type === 'error') {
          this.patchMessage(assistantMessageId, {
            content: event.content,
            state: 'done'
          });
          this.isProcessing = false;
          this.scrollToBottom();
        }
      },
      error: () => {
        this.patchMessage(assistantMessageId, {
          content: 'Sorry, I could not reach the AI support backend right now. Please try again in a moment.',
          state: 'done'
        });
        this.isProcessing = false;
        this.scrollToBottom();
      },
      complete: () => {
        this.isProcessing = false;
      }
    });
  }

  clearConversation(): void {
    this.activeStream?.unsubscribe();
    this.isProcessing = false;
    this.draftMessage = '';
    this.messages = [this.createWelcomeMessage()];
    this.scrollToBottom();
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

  private patchMessage(messageId: string, patch: Partial<ChatMessage>): void {
    this.messages = this.messages.map((message) =>
      message.id === messageId ? { ...message, ...patch } : message
    );
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

  private createId(): string {
    return globalThis.crypto?.randomUUID?.() ?? `${Date.now()}-${Math.random().toString(16).slice(2)}`;
  }

  private createWelcomeMessage(): ChatMessage {
    return {
      id: this.createId(),
      role: 'assistant',
      content: "Hey, I'm your AI IT agent. Ask me anything about equipment management or technical support, and I’ll help you troubleshoot or point you in the right direction.",
      state: 'done'
    };
  }
}
