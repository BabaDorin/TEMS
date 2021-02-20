import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RoomsService {

  constructor() { }

  getAllAutocompleteOptions(){
    return [
      {id: '1', value: '100'},
      {id: '2', value: '101'},
      {id: '3', value: '103'},
      {id: '4', value: '105'},
      {id: '5', value: '106'},
      {id: '6', value: '107'},
      {id: '7', value: '108'},
      {id: '8', value: '109'},
    ]
  }
}
