import { Observable } from 'rxjs';
import { API_ROOM_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './../tems-service/tems.service';
import { viewClassName } from '@angular/compiler';
import { IOption } from 'src/app/models/option.model';
import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { ViewRoom } from './../../models/room/view-room.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RoomsService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

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

  getRoomsSimplified(pageNumber: number, recordsPerPage: number): Observable<any>{
    return this.http.get(
      API_ROOM_URL + '/getSimplified' + '/' + pageNumber + '/' + recordsPerPage,
      this.httpOptions
    );
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
