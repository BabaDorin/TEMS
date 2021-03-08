import { API_LOG_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { TEMSService } from '../tems-service/tems.service';

@Injectable({
  providedIn: 'root'
})
export class LogsService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  getLogsByEquipmentId(
    equipmentId: string, 
    includeArchieved?: boolean,
    onlyArchieved?: boolean): Observable<any>{
      
      if(includeArchieved == undefined) includeArchieved = false;
      if(onlyArchieved == undefined) onlyArchieved = false;
      
      return this.http.get(
        API_LOG_URL + '/equipment/' + equipmentId + '/' + includeArchieved + '/' + onlyArchieved,
        this.httpOptions
        );
  }

  getLogsByRoomId(roomId: string){
    // return this.getLogsByEquipmentId('1'); // testing purposes
  }

  getLogsByPersonnelId(personnelId: string){
    // return this.getLogsByEquipmentId('1'); // testing purposes
  }

  getLogs(){
    return [
      new ViewLog(),
      new ViewLog(),
      new ViewLog(),
      new ViewLog(),
      new ViewLog(),
    ]
  }

  getLogTypes(): Observable<any>{
    return this.http.get(
      API_LOG_URL + '/getlogtypes',
      this.httpOptions
      );
  }
}
