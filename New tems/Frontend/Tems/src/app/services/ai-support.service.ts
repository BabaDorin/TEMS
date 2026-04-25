import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { API_AI_SUPPORT_URL } from '../models/backend.config';

export interface AiSupportStreamEvent {
  type: 'delta' | 'done' | 'error';
  content: string;
}

interface ParsedSseEvent {
  event: string;
  data: Record<string, unknown> | string;
}

@Injectable({
  providedIn: 'root'
})
export class AiSupportService {
  private readonly requestTimeoutMs = 30 * 60 * 1000;

  constructor(private oauthService: OAuthService) {}

  streamResponse(message: string): Observable<AiSupportStreamEvent> {
    return new Observable<AiSupportStreamEvent>((observer) => {
      const controller = new AbortController();
      const timeoutId = window.setTimeout(() => controller.abort(), this.requestTimeoutMs);

      const token = this.oauthService.getAccessToken();
      const headers: Record<string, string> = {
        'Content-Type': 'application/json; charset=utf-8',
        Accept: 'text/event-stream'
      };

      if (token) {
        headers.Authorization = `Bearer ${token}`;
      }

      fetch(`${API_AI_SUPPORT_URL}/chat/stream`, {
        method: 'POST',
        headers,
        body: JSON.stringify({ message }),
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
                if (parsedEvent.event === 'delta' && typeof parsedEvent.data === 'object') {
                  const chunk = String(parsedEvent.data['chunk'] ?? '');
                  if (chunk) {
                    observer.next({ type: 'delta', content: chunk });
                  }
                } else if (parsedEvent.event === 'done' && typeof parsedEvent.data === 'object') {
                  const content = String(parsedEvent.data['content'] ?? '');
                  observer.next({ type: 'done', content });
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
}
