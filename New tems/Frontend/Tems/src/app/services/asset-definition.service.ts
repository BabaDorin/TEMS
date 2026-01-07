import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_ASSET_DEFINITION_URL } from '../models/backend.config';
import { AssetDefinition, CreateAssetDefinitionRequest, UpdateAssetDefinitionRequest } from '../models/asset/asset-definition.model';

@Injectable({
  providedIn: 'root'
})
export class AssetDefinitionService {
  private readonly baseUrl = API_ASSET_DEFINITION_URL;

  constructor(private http: HttpClient) {}

  getAll(): Observable<AssetDefinition[]> {
    return this.http.get<{ assetDefinitions: AssetDefinition[] }>(this.baseUrl, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => response.assetDefinitions)
    );
  }

  getById(id: string): Observable<AssetDefinition> {
    return this.http.get<AssetDefinition>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  getByAssetTypeId(assetTypeId: string): Observable<AssetDefinition[]> {
    return this.http.get<{ assetDefinitions: AssetDefinition[] }>(`${this.baseUrl}?assetTypeId=${assetTypeId}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => response.assetDefinitions)
    );
  }

  create(definition: CreateAssetDefinitionRequest): Observable<AssetDefinition> {
    return this.http.post<AssetDefinition>(this.baseUrl, definition, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  update(id: string, definition: UpdateAssetDefinitionRequest): Observable<AssetDefinition> {
    return this.http.put<AssetDefinition>(`${this.baseUrl}/${id}`, definition, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }
}
