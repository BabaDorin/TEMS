import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TicketType, CreateTicketTypeRequest, UpdateTicketTypeRequest } from '../models/ticket/ticket-type.model';

@Injectable({
  providedIn: 'root'
})
export class TicketTypeService {
  private apiUrl = `${environment.apiUrl}/ticket-types`;

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'X-Tenant-ID': 'default-tenant'
    })
  };

  constructor(private http: HttpClient) { }

  getAll(): Observable<TicketType[]> {
    return this.http.get<TicketType[]>(this.apiUrl, this.httpOptions);
  }

  getById(id: string): Observable<TicketType> {
    return this.http.get<TicketType>(`${this.apiUrl}/${id}`, this.httpOptions);
  }

  create(request: CreateTicketTypeRequest): Observable<{ ticketTypeId: string }> {
    return this.http.post<{ ticketTypeId: string }>(this.apiUrl, request, this.httpOptions);
  }

  update(id: string, request: UpdateTicketTypeRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request, this.httpOptions);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.httpOptions);
  }
}
