import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_ASSET_URL } from '../models/backend.config';
import { Asset, CreateAssetRequest, UpdateAssetRequest } from '../models/asset/asset.model';

@Injectable({
  providedIn: 'root'
})
export class AssetService {
  private readonly baseUrl = API_ASSET_URL;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Asset[]> {
    return this.http.get<{ assets: Asset[] }>(this.baseUrl, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => response.assets)
    );
  }

  getById(id: string): Observable<Asset> {
    return this.http.get<Asset>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  getByAssetTag(tag: string): Observable<Asset> {
    return this.http.get<Asset>(`${this.baseUrl}/by-tag/${tag}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  getBySerialNumber(serialNumber: string): Observable<Asset> {
    return this.http.get<Asset>(`${this.baseUrl}/by-serial/${serialNumber}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  create(asset: CreateAssetRequest): Observable<Asset> {
    return this.http.post<Asset>(this.baseUrl, asset, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  update(id: string, asset: UpdateAssetRequest): Observable<Asset> {
    return this.http.put<Asset>(`${this.baseUrl}/${id}`, asset, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  // Legacy methods for backward compatibility with old asset components
  // These should not be used in new code - use the new asset module instead
  getFullDefinition(id: string): Observable<any> {
    console.warn('Using deprecated method getFullDefinition - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/definition/${id}`);
  }

  getDefinitionsSimplified(): Observable<any[]> {
    console.warn('Using deprecated method getDefinitionsSimplified - migrate to new asset module');
    return this.http.get<any[]>(`${API_ASSET_URL}/definitions/simplified`);
  }

  getTypesSimplified(): Observable<any[]> {
    console.warn('Using deprecated method getTypesSimplified - migrate to new asset module');
    return this.http.get<any[]>(`${API_ASSET_URL}/types/simplified`);
  }

  getPropertiesSimplified(): Observable<any[]> {
    console.warn('Using deprecated method getPropertiesSimplified - migrate to new asset module');
    return this.http.get<any[]>(`${API_ASSET_URL}/properties/simplified`);
  }

  generateAddAssetOfDefinition(definition: any): any {
    console.warn('Using deprecated method generateAddAssetOfDefinition - migrate to new asset module');
    return {};
  }

  getEquipmentToUpdate(id: string): Observable<any> {
    console.warn('Using deprecated method getEquipmentToUpdate - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/${id}`);
  }

  addEquipment(data: any): Observable<any> {
    console.warn('Using deprecated method addEquipment - migrate to new asset module');
    return this.http.post<any>(API_ASSET_URL, data);
  }

  updateEquipment(data: any): Observable<any> {
    console.warn('Using deprecated method updateEquipment - migrate to new asset module');
    return this.http.put<any>(`${API_ASSET_URL}/${data.id}`, data);
  }

  getDefinitionToUpdate(id: string): Observable<any> {
    console.warn('Using deprecated method getDefinitionToUpdate - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/definition/${id}`);
  }

  addDefinition(data: any): Observable<any> {
    console.warn('Using deprecated method addDefinition - migrate to new asset module');
    return this.http.post<any>(`${API_ASSET_URL}/definition`, data);
  }

  updateDefinition(data: any): Observable<any> {
    console.warn('Using deprecated method updateDefinition - migrate to new asset module');
    return this.http.put<any>(`${API_ASSET_URL}/definition/${data.id}`, data);
  }

  getPropertyById(id: string): Observable<any> {
    console.warn('Using deprecated method getPropertyById - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/property/${id}`);
  }

  addProperty(data: any): Observable<any> {
    console.warn('Using deprecated method addProperty - migrate to new asset module');
    return this.http.post<any>(`${API_ASSET_URL}/property`, data);
  }

  updateProperty(data: any): Observable<any> {
    console.warn('Using deprecated method updateProperty - migrate to new asset module');
    return this.http.put<any>(`${API_ASSET_URL}/property/${data.id}`, data);
  }

  addType(data: any): Observable<any> {
    console.warn('Using deprecated method addType - migrate to new asset module');
    return this.http.post<any>(`${API_ASSET_URL}/type`, data);
  }

  updateType(data: any): Observable<any> {
    console.warn('Using deprecated method updateType - migrate to new asset module');
    return this.http.put<any>(`${API_ASSET_URL}/type/${data.id}`, data);
  }

  getProperties(): Observable<any[]> {
    console.warn('Using deprecated method getProperties - migrate to new asset module');
    return this.http.get<any[]>(`${API_ASSET_URL}/properties`);
  }

  archieveEquipment(id: string, isArchived?: boolean): Observable<any> {
    console.warn('Using deprecated method archieveEquipment - migrate to new asset module');
    return this.http.patch<any>(`${API_ASSET_URL}/${id}/archive`, { isArchived });
  }

  getEquipmentSimplified(filter?: any): Observable<any[]> {
    console.warn('Using deprecated method getEquipmentSimplified - migrate to new asset module');
    return this.http.get<any[]>(`${API_ASSET_URL}/simplified`, { params: filter });
  }

  attach(data: any): Observable<any> {
    console.warn('Using deprecated method attach - migrate to new asset module');
    return this.http.post<any>(`${API_ASSET_URL}/attach`, data);
  }

  getEquipmentByID(id: string): Observable<any> {
    console.warn('Using deprecated method getEquipmentByID - migrate to new asset module');
    return this.getById(id);
  }

  changeWorkingState(id: string): Observable<any> {
    console.warn('Using deprecated method changeWorkingState - migrate to new asset module');
    return this.http.patch<any>(`${API_ASSET_URL}/${id}/working-state`, {});
  }

  changeUsingState(id: string): Observable<any> {
    console.warn('Using deprecated method changeUsingState - migrate to new asset module');
    return this.http.patch<any>(`${API_ASSET_URL}/${id}/using-state`, {});
  }

  getEquipmentSimplifiedById(id: string): Observable<any> {
    console.warn('Using deprecated method getEquipmentSimplifiedById - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/${id}/simplified`);
  }

  bulkUpload(formData: FormData): Observable<any> {
    console.warn('Using deprecated method bulkUpload - migrate to new asset module');
    return this.http.post<any>(`${API_ASSET_URL}/bulk-upload`, formData);
  }

  detach(id: string): Observable<any> {
    console.warn('Using deprecated method detach - migrate to new asset module');
    return this.http.post<any>(`${API_ASSET_URL}/detach`, { id });
  }

  getIdByTEMSID(temsId: string): Observable<any> {
    console.warn('Using deprecated method getIdByTEMSID - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/by-temsid/${temsId}`);
  }

  isTEMSIDAvailable(temsId: string): Observable<boolean> {
    console.warn('Using deprecated method isTEMSIDAvailable - migrate to new asset module');
    return this.http.get<boolean>(`${API_ASSET_URL}/temsid-available/${temsId}`);
  }

  getTypeSimplifiedById(id: string): Observable<any> {
    console.warn('Using deprecated method getTypeSimplifiedById - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/type/${id}/simplified`);
  }

  archieveType(id: string, isArchived?: boolean): Observable<any> {
    console.warn('Using deprecated method archieveType - migrate to new asset module');
    return this.http.patch<any>(`${API_ASSET_URL}/type/${id}/archive`, { isArchived });
  }

  getDefinitionSimplifiedById(id: string): Observable<any> {
    console.warn('Using deprecated method getDefinitionSimplifiedById - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/definition/${id}/simplified`);
  }

  archieveDefinition(id: string, isArchived?: boolean): Observable<any> {
    console.warn('Using deprecated method archieveDefinition - migrate to new asset module');
    return this.http.patch<any>(`${API_ASSET_URL}/definition/${id}/archive`, { isArchived });
  }

  getPropertySimplifiedById(id: string): Observable<any> {
    console.warn('Using deprecated method getPropertySimplifiedById - migrate to new asset module');
    return this.http.get<any>(`${API_ASSET_URL}/property/${id}/simplified`);
  }

  archieveProperty(id: string, isArchived?: boolean): Observable<any> {
    console.warn('Using deprecated method archieveProperty - migrate to new asset module');
    return this.http.patch<any>(`${API_ASSET_URL}/property/${id}/archive`, { isArchived });
  }

  removeAsset(id: string): Observable<any> {
    console.warn('Using deprecated method removeAsset - migrate to new asset module');
    return this.delete(id);
  }

  removeProperty(id: string): Observable<any> {
    console.warn('Using deprecated method removeProperty - migrate to new asset module');
    return this.http.delete<any>(`${API_ASSET_URL}/property/${id}`);
  }

  removeAllocation(id: string): Observable<any> {
    console.warn('Using deprecated method removeAllocation - migrate to new asset module');
    return this.http.delete<any>(`${API_ASSET_URL}/allocation/${id}`);
  }
}
