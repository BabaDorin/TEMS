import { AllocationFilter } from './../helpers/filters/allocation.filter';
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

  getTotalItems(filter: AllocationFilter): Observable<number>{
    console.log('called getTotalItems');
    return this.http.post<number>(
      API_ALL_URL + '/gettotalitems',
      JSON.stringify(filter),
      this.httpOptions
    );
  }

  getAllocations(filter: AllocationFilter): Observable<ViewAllocationSimplified[]> {
    console.log('called getAllocations');
      return this.http.post<ViewAllocationSimplified[]>(
        API_ALL_URL + '/getallocations',
        JSON.stringify(filter),
        this.httpOptions
      );
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
