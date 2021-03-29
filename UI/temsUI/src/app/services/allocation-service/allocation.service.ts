import { Observable } from 'rxjs';
import { API_ALL_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { AddAllocation } from './../../models/allocation/add-allocation.model';
import { TEMSService } from './../tems-service/tems.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AllocationService extends TEMSService{

  constructor(
    private http: HttpClient
  ){
    super();
  }

  getEntityAllocations(entityType: string, entityId: string): Observable<any>{
    return this.http.get(
      API_ALL_URL + '/getofentity/' + entityType + '/' + entityId,
      this.httpOptions
    );
  }

  getEquipmentAllocations(equipmentId: string): Observable<any>{
    return this.getEntityAllocations('equipment', equipmentId);
  }

  getEquipmentAllocationsToRoom(roomId: string){
    return this.getEntityAllocations('room', roomId);
  }

  getEquipmentAllocationsToPersonnel(personnelId: string){
    return this.getEntityAllocations('personnel', personnelId);
  }

  createAllocation(addAllocation: AddAllocation): Observable<any>{
    return this.http.post(
      API_ALL_URL + '/create',
      addAllocation,
      this.httpOptions
    );
  }

  markAsReturned(allocationId: string): Observable<any>{
    return this.http.get(
      API_ALL_URL + '/markasreturned/' + allocationId,
      this.httpOptions
    );
  }

  remove(allocationId: string): Observable<any>{
    return this.http.get(
      API_ALL_URL + '/remove/' + allocationId,
      this.httpOptions
    );
  }
}
