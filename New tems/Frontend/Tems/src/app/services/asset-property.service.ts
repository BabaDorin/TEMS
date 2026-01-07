import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_ASSET_PROPERTY_URL } from '../models/backend.config';
import { AssetProperty, CreateAssetPropertyRequest, UpdateAssetPropertyRequest } from '../models/asset/asset-property.model';

@Injectable({
  providedIn: 'root'
})
export class AssetPropertyService {
  private readonly baseUrl = API_ASSET_PROPERTY_URL;

  constructor(private http: HttpClient) {}

  getAll(): Observable<AssetProperty[]> {
    return this.http.get<{ properties: AssetProperty[] }>(this.baseUrl, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => response.properties)
    );
  }

  getById(id: string): Observable<AssetProperty> {
    return this.http.get<AssetProperty>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  create(property: CreateAssetPropertyRequest): Observable<AssetProperty> {
    return this.http.post<AssetProperty>(this.baseUrl, property, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  update(id: string, property: UpdateAssetPropertyRequest): Observable<AssetProperty> {
    return this.http.put<AssetProperty>(`${this.baseUrl}/${id}`, property, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }
}
