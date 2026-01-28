import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Site } from '../models/location/site.model';
import { Building } from '../models/location/building.model';
import { RoomWithHierarchy } from '../models/location/room.model';

interface GetAllSitesResponse {
  success: boolean;
  message?: string;
  data: Site[];
}

interface GetAllBuildingsResponse {
  success: boolean;
  message?: string;
  data: Building[];
}

interface RoomDtoFromBackend {
  id: string;
  buildingId: string;
  name: string;
  roomNumber?: string;
  floorLabel?: string;
  type: string;
  capacity?: number;
  area?: number;
  status: string;
  description?: string;
  tenantId: string;
  createdAt: string;
  updatedAt: string;
  siteName?: string;
  siteId?: string;
  buildingName?: string;
  assetCounts?: Record<string, number>;
}

interface GetAllRoomsResponse {
  success: boolean;
  message?: string;
  data: RoomDtoFromBackend[];
}

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private readonly baseUrl = `${environment.apiUrl}/location`;

  constructor(private http: HttpClient) {}

  // Get all sites
  getAllSites(): Observable<Site[]> {
    return this.http.get<GetAllSitesResponse>(`${this.baseUrl}/sites`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => response.data || [])
    );
  }

  // Get all buildings (optionally filtered by siteId)
  getAllBuildings(siteId?: string): Observable<Building[]> {
    let params = new HttpParams();
    if (siteId) {
      params = params.set('siteId', siteId);
    }

    return this.http.get<GetAllBuildingsResponse>(`${this.baseUrl}/buildings`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      params
    }).pipe(
      map(response => response.data || [])
    );
  }

  // Get rooms with hierarchy (optionally filtered by siteId and/or buildingId)
  getRoomsWithHierarchy(siteId?: string, buildingId?: string): Observable<RoomWithHierarchy[]> {
    let params = new HttpParams();
    if (siteId) {
      params = params.set('siteId', siteId);
    }
    if (buildingId) {
      params = params.set('buildingId', buildingId);
    }

    return this.http.get<GetAllRoomsResponse>(`${this.baseUrl}/rooms`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      params
    }).pipe(
      map(response => {
        if (!response.data) return [];
        
        return response.data.map(room => ({
          id: room.id,
          buildingId: room.buildingId,
          name: room.name,
          roomNumber: room.roomNumber,
          floorLabel: room.floorLabel,
          type: room.type as any,
          capacity: room.capacity,
          area: room.area,
          status: room.status as any,
          description: room.description,
          createdAt: new Date(room.createdAt),
          updatedAt: new Date(room.updatedAt),
          siteName: room.siteName,
          siteId: room.siteId,
          buildingName: room.buildingName,
          assetCounts: room.assetCounts
        }));
      })
    );
  }

  // Get room by ID
  getRoomById(id: string): Observable<RoomWithHierarchy> {
    return this.http.get<any>(`${this.baseUrl}/rooms/${id}`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(response => {
        const room = response.data;
        return {
          id: room.id,
          buildingId: room.buildingId,
          name: room.name,
          roomNumber: room.roomNumber,
          floorLabel: room.floorLabel,
          type: room.type as any,
          capacity: room.capacity,
          area: room.area,
          status: room.status as any,
          description: room.description,
          createdAt: new Date(room.createdAt),
          updatedAt: new Date(room.updatedAt),
          siteName: room.siteName,
          siteId: room.siteId,
          buildingName: room.buildingName,
          assetCounts: room.assetCounts
        };
      })
    );
  }

  // Create a new room
  createRoom(roomData: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/rooms`, roomData, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  // Get assets by room ID
  getAssetsByRoom(roomId: string, pageNumber: number = 1, pageSize: number = 50): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<any>(`${this.baseUrl}/rooms/${roomId}/assets`, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      params
    });
  }
}
