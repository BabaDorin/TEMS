import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_ASSET_TYPE_URL } from '../models/backend.config';
import { AssetType, CreateAssetTypeRequest, UpdateAssetTypeRequest } from '../models/asset/asset-type.model';

@Injectable({
  providedIn: 'root'
})
export class AssetTypeService {
  private readonly baseUrl = API_ASSET_TYPE_URL;

  constructor(private http: HttpClient) {}

  getAll(): Observable<AssetType[]> {
    return this.http.get<{ assetTypes: AssetType[] }>(this.baseUrl, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => response.assetTypes)
    );
  }

  getById(id: string): Observable<AssetType> {
    return this.http.get<AssetType>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  create(type: CreateAssetTypeRequest): Observable<AssetType> {
    return this.http.post<AssetType>(this.baseUrl, type, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  update(id: string, type: UpdateAssetTypeRequest): Observable<AssetType> {
    return this.http.put<AssetType>(`${this.baseUrl}/${id}`, type, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }
}
