import { AddKeyAllocation } from '../models/key/add-key-allocation.model';
import { AddKey } from '../models/key/add-key.model';
import { API_KEY_URL } from '../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './tems.service';
import { Observable } from 'rxjs';
import { IOption } from 'src/app/models/option.model';
import { ViewKeyAllocation } from '../models/key/view-key-allocation.model';
import { Injectable } from '@angular/core';
import { ViewKeySimplified } from 'src/app/models/key/view-key.model';

@Injectable({
  providedIn: 'root'
})
export class KeysService extends TEMSService {

  constructor(
    private http: HttpClient
  ){
    super();
  }

  remove(keyId: string): Observable<any>{
    return this.http.delete(
      API_KEY_URL + '/remove/' + keyId,
      this.httpOptions
    );
  }

  removeAllocation(allocationId: string): Observable<any>{
    return this.http.delete(
      API_KEY_URL + '/removeAllocation/' + allocationId,
      this.httpOptions
    );
  }

  getKeys(): Observable<any> {
    return this.http.get(
      API_KEY_URL + '/get',
      this.httpOptions
    )
  }

  createKey(addKey: AddKey): Observable<any>{
    return this.http.post(
      API_KEY_URL + '/create',
      JSON.stringify(addKey),
      this.httpOptions
    );
  }

  createAllocation(addKeyAllocation: AddKeyAllocation): Observable<any>{
    return this.http.post(
      API_KEY_URL + '/createAllocation',
      JSON.stringify(addKeyAllocation),
      this.httpOptions
    );
  }

  getAllocations(keyId: string, roomId: string, personnelId: string): Observable<ViewKeyAllocation[]>{
    return this.http.get<ViewKeyAllocation[]>(
      API_KEY_URL + '/getallocations/' + keyId + '/' + roomId + '/' +personnelId,
      this.httpOptions
    );
  }

  getAllAutocompleteOptions(filter?: string): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_KEY_URL + '/getallautocompleteoptions',
      this.httpOptions
    );
  }

  markAsReturned(keyId: string): Observable<any>{
    return this.http.get(
      API_KEY_URL + '/markAsReturned/' + keyId,
      this.httpOptions
    );
  }

  archieveKey(keyId: string): Observable<any>{
    return this.http.get(
      API_KEY_URL+ '/archieve/' + keyId,
      this.httpOptions
    );
  }

  archieveAllocation(keyAllocationId: string): Observable<any>{
    return this.http.get(
      API_KEY_URL + '/archieveAllocation/' + keyAllocationId,
      this.httpOptions
    );
  }
}
