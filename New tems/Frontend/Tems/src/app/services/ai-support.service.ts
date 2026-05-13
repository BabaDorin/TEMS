import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { API_AI_SUPPORT_URL } from '../models/backend.config';

export interface AiSupportConversationSummary {
  conversationId: string;
  title: string;
  createdAt: string;
  updatedAt: string;
  messageCount: number;
}

export interface AiSupportConversationMessage {
  messageId: string;
  role: 'user' | 'assistant';
  content: string;
  createdAt: string;
}

export interface AiSupportConversationDetail extends AiSupportConversationSummary {
  messages: AiSupportConversationMessage[];
}

export type AiSupportStreamEvent =
  | { type: 'conversation'; conversation: AiSupportConversationSummary }
  | { type: 'delta'; content: string }
  | { type: 'done'; content: string; conversation?: AiSupportConversationSummary }
  | { type: 'error'; content: string };

interface ParsedSseEvent {
  event: string;
  data: Record<string, unknown> | string;
}

@Injectable({
  providedIn: 'root'
})
export class AiSupportService {
  private readonly requestTimeoutMs = 30 * 60 * 1000;
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'X-Tenant-Id': 'default'
    })
  };

  constructor(
    private oauthService: OAuthService,
    private http: HttpClient
  ) {}

  getConversations(): Observable<AiSupportConversationSummary[]> {
    return this.http.get<AiSupportConversationSummary[]>(`${API_AI_SUPPORT_URL}/conversations`, this.httpOptions);
  }

  getConversation(conversationId: string): Observable<AiSupportConversationDetail> {
    return this.http.get<AiSupportConversationDetail>(`${API_AI_SUPPORT_URL}/conversations/${conversationId}`, this.httpOptions);
  }

  deleteConversation(conversationId: string): Observable<void> {
    return this.http.delete<void>(`${API_AI_SUPPORT_URL}/conversations/${conversationId}`, this.httpOptions);
  }

  streamResponse(message: string, conversationId?: string | null): Observable<AiSupportStreamEvent> {
    return new Observable<AiSupportStreamEvent>((observer) => {
      const controller = new AbortController();
      const timeoutId = window.setTimeout(() => controller.abort(), this.requestTimeoutMs);

      const token = this.oauthService.getAccessToken();
      const headers: Record<string, string> = {
        'Content-Type': 'application/json; charset=utf-8',
        Accept: 'text/event-stream',
        'X-Tenant-Id': 'default'
      };

      if (token) {
        headers.Authorization = `Bearer ${token}`;
      }

      fetch(`${API_AI_SUPPORT_URL}/chat/stream`, {
        method: 'POST',
        headers,
        body: JSON.stringify({
          message,
          conversationId: conversationId ?? null
        }),
        signal: controller.signal
      })
        .then(async (response) => {
          if (!response.ok) {
            throw new Error(`AI support request failed with status ${response.status}`);
          }

          if (!response.body) {
            throw new Error('AI support response stream is unavailable');
          }

          const reader = response.body.getReader();
          const decoder = new TextDecoder();
          let buffer = '';
          let completed = false;

          while (true) {
            const { value, done } = await reader.read();
            if (done) {
              break;
            }

            buffer += decoder.decode(value, { stream: true });

            let eventBoundary = buffer.indexOf('\n\n');
            while (eventBoundary !== -1) {
              const rawEvent = buffer.slice(0, eventBoundary).trim();
              buffer = buffer.slice(eventBoundary + 2);

              const parsedEvent = this.parseEvent(rawEvent);
              if (parsedEvent) {
                if (parsedEvent.event === 'conversation' && typeof parsedEvent.data === 'object') {
                  observer.next({
                    type: 'conversation',
                    conversation: this.toConversationSummary(parsedEvent.data)
                  });
                } else if (parsedEvent.event === 'delta' && typeof parsedEvent.data === 'object') {
                  const chunk = String(parsedEvent.data['chunk'] ?? '');
                  if (chunk) {
                    observer.next({ type: 'delta', content: chunk });
                  }
                } else if (parsedEvent.event === 'done' && typeof parsedEvent.data === 'object') {
                  const content = String(parsedEvent.data['content'] ?? '');
                  const conversation = this.tryReadConversation(parsedEvent.data['conversation']);
                  observer.next({ type: 'done', content, conversation });
                  completed = true;
                  observer.complete();
                  controller.abort();
                  break;
                } else if (parsedEvent.event === 'error' && typeof parsedEvent.data === 'object') {
                  const message = String(parsedEvent.data['message'] ?? 'AI support is temporarily unavailable.');
                  observer.next({ type: 'error', content: message });
                  completed = true;
                  observer.complete();
                  controller.abort();
                  break;
                }
              }

              eventBoundary = buffer.indexOf('\n\n');
            }

            if (completed) {
              break;
            }
          }

          if (!completed) {
            observer.complete();
          }
        })
        .catch((error) => {
          if (controller.signal.aborted) {
            return;
          }

          observer.error(error);
        })
        .finally(() => {
          window.clearTimeout(timeoutId);
        });

      return () => {
        window.clearTimeout(timeoutId);
        controller.abort();
      };
    });
  }

  private parseEvent(rawEvent: string): ParsedSseEvent | null {
    if (!rawEvent) {
      return null;
    }

    let event = 'message';
    const dataLines: string[] = [];

    for (const line of rawEvent.split(/\r?\n/)) {
      if (line.startsWith('event:')) {
        event = line.slice(6).trim();
        continue;
      }

      if (line.startsWith('data:')) {
        dataLines.push(line.slice(5).trimStart());
      }
    }

    const dataString = dataLines.join('\n');
    if (!dataString) {
      return { event, data: '' };
    }

    try {
      return { event, data: JSON.parse(dataString) as Record<string, unknown> };
    } catch {
      return { event, data: dataString };
    }
  }

  private tryReadConversation(value: unknown): AiSupportConversationSummary | undefined {
    if (!value || typeof value !== 'object') {
      return undefined;
    }

    return this.toConversationSummary(value as Record<string, unknown>);
  }

  private toConversationSummary(value: Record<string, unknown>): AiSupportConversationSummary {
    return {
      conversationId: String(value['conversationId'] ?? ''),
      title: String(value['title'] ?? 'Conversation'),
      createdAt: String(value['createdAt'] ?? ''),
      updatedAt: String(value['updatedAt'] ?? ''),
      messageCount: Number(value['messageCount'] ?? 0)
    };
  }
}
