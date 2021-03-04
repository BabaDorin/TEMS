import { IOption } from 'src/app/models/option.model';
import { ViewKeyAllocation } from './../../models/key/view-key-allocation.model';
import { Injectable } from '@angular/core';
import { ViewKeySimplified } from 'src/app/models/key/view-key.model';

@Injectable({
  providedIn: 'root'
})
export class KeysService {

  getKeys(): ViewKeySimplified[]{
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
