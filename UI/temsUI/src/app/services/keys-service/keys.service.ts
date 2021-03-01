import { IOption } from './../../models/option.model';
import { ViewKeyAllocation } from './../../models/key/view-key-allocation.model';
import { Injectable } from '@angular/core';
import { ViewKeySimplified } from 'src/app/models/key/view-key.model';

@Injectable({
  providedIn: 'root'
})
export class KeysService {

  getKeys(){
    return [
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
    ]
  }

  getAllocationsOfKey(keyId: string){
    return [
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
    ];
  }

  getAllocations(){
    return [
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
      new ViewKeyAllocation(),
    ];
  }

  getAutocompleteOptions(){
    return [
      {id: '1', value: '101'},      
      {id: '2', value: '102'},      
      {id: '3', value: '103'},      
      {id: '4', value: '104'},      
      {id: '5', value: '105'},      
      {id: '6', value: '106'},      
    ]
  }
}
