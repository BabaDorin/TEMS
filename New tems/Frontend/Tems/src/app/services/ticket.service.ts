import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap, catchError, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Ticket, CreateTicketRequest, UpdateTicketRequest, TicketConversation, AddMessageRequest } from '../models/ticket/ticket.model';
import { TicketManagementStateService } from '../state/ticket-management.state';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl = `${environment.apiUrl}/tickets`;

  private httpOptions = {
    headers: new HttpHeaders({
      'X-Tenant-Id': 'default'
    })
  };

  constructor(
    private http: HttpClient,
    private stateService: TicketManagementStateService
  ) { }

  getAll(forceRefresh = false): Observable<Ticket[]> {
    // Return cached data if available and not forcing refresh
    if (!forceRefresh && this.stateService.ticketsLoaded()) {
      return of(this.stateService.tickets());
    }

    this.stateService.setTicketsLoading(true);
    
    // Backend returns { tickets: [...] } structure (camelCase from JSON serialization)
    // Map that response to Ticket[] so the method signature is correct.
    return this.http.get<{ tickets: Ticket[] }>(this.apiUrl, this.httpOptions).pipe(
      tap(response => this.stateService.setTickets(response.tickets)),
      map(response => response.tickets),
      catchError(error => {
        this.stateService.setTicketsError(error.message || 'Failed to load tickets');
        return of([] as Ticket[]);
      })
    );
  }

  getById(id: string): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.apiUrl}/${id}`, this.httpOptions);
  }

  create(request: CreateTicketRequest): Observable<{ ticketId: string; humanReadableId: string }> {
    return this.http.post<{ ticketId: string; humanReadableId: string }>(this.apiUrl, request, this.httpOptions).pipe(
      tap(() => this.stateService.invalidateTickets())
    );
  }

  update(id: string, request: UpdateTicketRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request, this.httpOptions).pipe(
      tap(() => this.stateService.invalidateTickets())
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.httpOptions).pipe(
      tap(() => this.stateService.removeTicket(id))
    );
  }

  getMessages(ticketId: string): Observable<TicketConversation> {
    return this.http.get<TicketConversation>(`${this.apiUrl}/${ticketId}/messages`, this.httpOptions);
  }

  addMessage(ticketId: string, request: AddMessageRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${ticketId}/messages`, request, this.httpOptions);
  }
}
