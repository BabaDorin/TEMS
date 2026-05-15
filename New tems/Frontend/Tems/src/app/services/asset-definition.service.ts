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

  getAll(includeArchived: boolean = false): Observable<AssetDefinition[]> {
    const url = includeArchived ? `${this.baseUrl}?includeArchived=true` : this.baseUrl;

    return this.http.get<{ assetDefinitions: AssetDefinition[] }>(url, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => response.assetDefinitions)
    );
  }

  getById(id: string): Observable<AssetDefinition> {
    return this.http.get<{ assetDefinition: AssetDefinition }>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => response.assetDefinition)
    );
  }

  getByAssetTypeId(assetTypeId: string): Observable<AssetDefinition[]> {
    return this.getAll().pipe(
      map(definitions => definitions.filter(definition => definition.assetTypeId === assetTypeId))
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
