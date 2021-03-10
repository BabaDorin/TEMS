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

  getEquipmentAllocations(equipmentId: string): Observable<any>{
    return this.http.get(
      API_ALL_URL + '/getSimplifiedOfEquipment/' + equipmentId,
      this.httpOptions
    );
  }

  getEquipmentAllocationsToRoom(roomId: string){
    // return this.getEquipmentAllocations('1'); // testing purposes!
  }

  getEquipmentAllocationsToPersonnel(personnelId: string){
    // return this.getEquipmentAllocations('1'); // testing purposes!
  }

  createAllocation(addAllocation: AddAllocation): Observable<any>{
    return this.http.post(
      API_ALL_URL + '/create',
      addAllocation,
      this.httpOptions
    );
  }
}
