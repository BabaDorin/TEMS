import { Observable } from 'rxjs';
import { IOption } from '../models/option.model';
import { API_ROOM_URL } from '../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './tems.service';
import { ViewRoomSimplified } from '../models/room/view-room-simplified.model';
import { ViewRoom } from '../models/room/view-room.model';
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

  remove(roomId: string): Observable<any>{
    return this.http.get(
      API_ROOM_URL + '/remove/' + roomId,
      this.httpOptions
    );
  }

  getAllAutocompleteOptions(filter?: string): Observable<IOption[]>{
    let endPoint = API_ROOM_URL + '/getallautocompleteoptions';
    if(filter != undefined)
      endPoint += '/' + filter;
      
    return this.http.get<IOption[]>(
      endPoint,
      this.httpOptions
    );
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


  getRoomToUpdate(roomId: string): Observable<AddRoom>{
    return this.http.get<AddRoom>(
      API_ROOM_URL + '/getroomtoupdate/' + roomId,
      this.httpOptions
    );
  } 

  updateRoom(room: AddRoom): Observable<any>{
    return this.http.post(
      API_ROOM_URL + '/update',
      JSON.stringify(room),
      this.httpOptions
    );
  }

  getRoomById(id: string): Observable<any>{
    return this.http.get(
      API_ROOM_URL + '/getbyid/' + id,
      this.httpOptions 
    );
  }

  archieveRoom(roomId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved

    return this.http.get(
      API_ROOM_URL + '/archieve/' + roomId + '/' + archivationStatus,
      this.httpOptions
    )
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
    roomSimplified.isArchieved = room.isArchieved;

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
