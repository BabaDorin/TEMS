import { Observable } from 'rxjs';
import { API_ROOM_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './../tems-service/tems.service';
import { viewClassName } from '@angular/compiler';
import { IOption } from 'src/app/models/option.model';
import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { ViewRoom } from './../../models/room/view-room.model';
import { Injectable } from '@angular/core';
import { AddRoom } from 'src/app/models/room/add-room.model';

@Injectable({
  providedIn: 'root'
})
export class RoomsService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  getAllAutocompleteOptions(): Observable<any>{
    return this.http.get(
      API_ROOM_URL + '/getallautocompleteoptions',
      this.httpOptions
    );
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
    
  getRoomLabels(): Observable<any>{
    return this.http.get(
      API_ROOM_URL + '/getlabels',
      this.httpOptions
    );
  }
  
  getRoomById(id: string): Observable<any>{
    return this.http.get(
      API_ROOM_URL + '/getbyid/' + id,
      this.httpOptions 
    );
  }

  getRoomSimplified(id: string): ViewRoomSimplified{
    return new ViewRoomSimplified();
  }
  
  getRoomSimplifiedFromRoom(room: ViewRoom): ViewRoomSimplified{
    let roomSimplified = new ViewRoomSimplified();
    roomSimplified.id = room.id;
    roomSimplified.description = room.description;
    roomSimplified.identifier = room.identifier;
    roomSimplified.activeIssues =  room.activeTickets;
    roomSimplified.allocatedEquipment = 999; // room does not have allocatedEquipment
    roomSimplified.label = room.labels.join(", ");

    return roomSimplified;
  }

  createRoom(addRoom: AddRoom): Observable<any> {
    return this.http.post(
      API_ROOM_URL + '/create',
      addRoom,
      this.httpOptions
    );
  }
}
