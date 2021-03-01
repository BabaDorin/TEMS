import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { ViewRoom } from './../../models/room/view-room.model';
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

  // Mainly used by Ag-Grid-rooms
  getRooms(){
    return [
      new ViewRoomSimplified(),
      new ViewRoomSimplified(),
      new ViewRoomSimplified(),
    ];
  }

  getRoomLabels(){
    return[
      { id: '1', value: 'Laboratory'},
      { id: '2', value: 'Simple ClassRoom'},
      { id: '3', value: 'Deposit'},
      { id: '4', value: 'Idk'},
    ]
  }
  
  getRoomById(id: string){
    return new ViewRoom();
  }

  getRoomSimplified(id: string){
    return new ViewRoomSimplified();
  }
}
