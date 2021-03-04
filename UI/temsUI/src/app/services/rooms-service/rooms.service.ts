import { viewClassName } from '@angular/compiler';
import { IOption } from 'src/app/models/option.model';
import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { ViewRoom } from './../../models/room/view-room.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RoomsService {

  constructor() { }

  getAllAutocompleteOptions(): IOption[]{
    return [
      {value: '1', label: '100'},
      {value: '2', label: '101'},
      {value: '3', label: '103'},
      {value: '4', label: '105'},
      {value: '5', label: '106'},
      {value: '6', label: '107'},
      {value: '7', label: '108'},
      {value: '8', label: '109'},
    ]
  }

  getRooms(): ViewRoomSimplified[]{
    return [
      new ViewRoomSimplified(),
      new ViewRoomSimplified(),
      new ViewRoomSimplified(),
    ];
  }

  getRoomLabels(): IOption[]{
    return[
      { value: '1', label: 'Laboratory'},
      { value: '2', label: 'Simple ClassRoom'},
      { value: '3', label: 'Deposit'},
      { value: '4', label: 'Idk'},
    ]
  }
  
  getRoomById(id: string): ViewRoom{
    return new ViewRoom();
  }

  getRoomSimplified(id: string): ViewRoomSimplified{
    return new ViewRoomSimplified();
  }
}
