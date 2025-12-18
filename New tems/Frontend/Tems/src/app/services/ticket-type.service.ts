import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap, catchError, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { TicketType, CreateTicketTypeRequest, UpdateTicketTypeRequest } from '../models/ticket/ticket-type.model';
import { TicketManagementStateService } from '../state/ticket-management.state';

@Injectable({
  providedIn: 'root'
})
export class TicketTypeService {
  private apiUrl = `${environment.apiUrl}/ticket-types`;

  private httpOptions = {
    headers: new HttpHeaders({
      'X-Tenant-Id': 'default'
    })
  };

  constructor(
    private http: HttpClient,
    private stateService: TicketManagementStateService
  ) { }

  getAll(forceRefresh = false): Observable<TicketType[]> {
    // Return cached data if available and not forcing refresh
    const cachedTypes = this.stateService.ticketTypes();
    if (!forceRefresh && this.stateService.ticketTypesLoaded() && cachedTypes.length > 0) {
      return of(cachedTypes);
    }

    this.stateService.setTicketTypesLoading(true);
    
    // Backend returns { ticketTypes: [...] } structure (camelCase from JSON serialization)
    // Map that response to TicketType[] so the method signature is correct.
    return this.http.get<{ ticketTypes: TicketType[] }>(this.apiUrl, this.httpOptions).pipe(
      tap(response => this.stateService.setTicketTypes(response.ticketTypes)),
      map(response => response.ticketTypes),
      catchError(error => {
        this.stateService.setTicketTypesError(error.message || 'Failed to load ticket types');
        return of([] as TicketType[]);
      })
    );
  }

  getById(id: string): Observable<TicketType> {
    return this.http.get<TicketType>(`${this.apiUrl}/${id}`, this.httpOptions);
  }

  create(request: CreateTicketTypeRequest): Observable<{ ticketTypeId: string }> {
    return this.http.post<{ ticketTypeId: string }>(this.apiUrl, request, this.httpOptions).pipe(
      tap(() => this.stateService.invalidateTicketTypes())
    );
  }

  update(id: string, request: UpdateTicketTypeRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request, this.httpOptions).pipe(
      tap(() => this.stateService.invalidateTicketTypes())
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.httpOptions).pipe(
      tap(() => this.stateService.removeTicketType(id))
    );
  }
}
