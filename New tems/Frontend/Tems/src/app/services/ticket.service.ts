import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Ticket, CreateTicketRequest, UpdateTicketRequest, TicketConversation, AddMessageRequest } from '../models/ticket/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl = `${environment.apiUrl}/tickets`;

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'X-Tenant-ID': 'default-tenant'
    })
  };

  constructor(private http: HttpClient) { }

  getAll(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(this.apiUrl, this.httpOptions);
  }

  getById(id: string): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.apiUrl}/${id}`, this.httpOptions);
  }

  create(request: CreateTicketRequest): Observable<{ ticketId: string }> {
    return this.http.post<{ ticketId: string }>(this.apiUrl, request, this.httpOptions);
  }

  update(id: string, request: UpdateTicketRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request, this.httpOptions);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.httpOptions);
  }

  getMessages(ticketId: string): Observable<TicketConversation> {
    return this.http.get<TicketConversation>(`${this.apiUrl}/${ticketId}/messages`, this.httpOptions);
  }

  addMessage(ticketId: string, request: AddMessageRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${ticketId}/messages`, request, this.httpOptions);
  }
}
