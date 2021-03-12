import { AddKey } from './../../models/key/add-key.model';
import { API_KEY_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './../tems-service/tems.service';
import { Observable } from 'rxjs';
import { IOption } from 'src/app/models/option.model';
import { ViewKeyAllocation } from './../../models/key/view-key-allocation.model';
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

  getAllocationsOfKey(keyId: string): ViewKeyAllocation[]{
    return [
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
    ];
  }

  getAllocations(): ViewKeyAllocation[]{
    return [
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
    ];
  }

  getAutocompleteOptions(): IOption[]{
    return [
      {value: '1', label: '101'},      
      {value: '2', label: '102'},      
      {value: '3', label: '103'},      
      {value: '4', label: '104'},      
      {value: '5', label: '105'},      
      {value: '6', label: '106'},      
    ]
  }
}
