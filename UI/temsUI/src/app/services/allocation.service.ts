import { ViewAllocationSimplified } from '../models/equipment/view-equipment-allocation.model';
import { ViewEquipmentSimplified } from '../models/equipment/view-equipment-simplified.model';
import { IOption } from '../models/option.model';
import { Observable } from 'rxjs';
import { API_ALL_URL, API_EQ_URL } from '../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { AddAllocation } from '../models/allocation/add-allocation.model';
import { TEMSService } from './tems.service';
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

  getAllocations(
    eqIds: string[], 
    defIds:string[],
    persIds: string[],
    rIds: string[],
    include?: string): Observable<ViewAllocationSimplified[]> {
      if(include == undefined)
        include = 'any';

      let entityCollection = {
        equipmentIds: eqIds,
        definitionIds: defIds,
        personnelIds: persIds,
        roomIds: rIds,
        include: include
      }

      return this.http.post<ViewAllocationSimplified[]>(
        API_ALL_URL + '/getallocations',
        JSON.stringify(entityCollection),
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

  archieve(allocationId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved
    
    return this.http.get(
      API_ALL_URL + '/archieve/' + allocationId + '/' + archivationStatus, 
      this.httpOptions
    );
  }
}
