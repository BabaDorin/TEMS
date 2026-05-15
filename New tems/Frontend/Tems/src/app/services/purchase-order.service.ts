import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  DeletePurchaseOrderResponse,
  GetAllPurchaseOrdersResponse,
  GetPurchaseOrderByIdResponse,
  PurchaseOrder
} from '../models/purchase-order/purchase-order.model';

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderService {
  private apiUrl = `${environment.apiUrl}/purchase-orders`;

  private httpOptions = {
    headers: new HttpHeaders({
      'X-Tenant-Id': 'default'
    })
  };

  constructor(private http: HttpClient) {}

  getAll(): Observable<PurchaseOrder[]> {
    return this.http.get<GetAllPurchaseOrdersResponse>(this.apiUrl, this.httpOptions).pipe(
      map(response => response.purchaseOrders || [])
    );
  }

  getById(id: string): Observable<PurchaseOrder | null> {
    return this.http.get<GetPurchaseOrderByIdResponse>(`${this.apiUrl}/${id}`, this.httpOptions).pipe(
      map(response => response.purchaseOrder || null)
    );
  }

  delete(id: string): Observable<DeletePurchaseOrderResponse> {
    return this.http.delete<DeletePurchaseOrderResponse>(`${this.apiUrl}/${id}`, this.httpOptions);
  }
}
