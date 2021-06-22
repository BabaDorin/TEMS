import { AddLog } from './../../models/communication/logs/add-log.model';
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

  getEntityLogs(entityType: string, entityId: string): Observable<any>{
    return this.http.get(
      API_LOG_URL + '/getentitylogs/' + entityType + '/' + entityId,
      this.httpOptions
    );
  }

  remove(logId: string): Observable<any>{
    return this.http.get(
      API_LOG_URL + '/remove/' + logId,
      this.httpOptions
    );
  }

  getLogsByEquipmentId(equipmentId: string): Observable<any>{
    return this.getEntityLogs('equipment', equipmentId);
  }

  getLogsByRoomId(roomId: string): Observable<any>{
    return this.getEntityLogs('room', roomId);
  }

  getLogsByPersonnelId(personnelId: string): Observable<any>{
    return this.getEntityLogs('personnel', personnelId);
  }

  getLogs(): Observable<any>{
    return this.getEntityLogs('any', 'any');
  }

  archieve(logId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved

    return this.http.get(
      API_LOG_URL + '/archieve/' + logId + '/' + archivationStatus,
      this.httpOptions
    );
  }

  getLogTypes(): Observable<any>{
    return this.http.get(
      API_LOG_URL + '/getlogtypes',
      this.httpOptions
      );
  }

  addLog(addLog: AddLog): Observable<any>{
    return this.http.post(
      API_LOG_URL + '/create',
      addLog,
      this.httpOptions
    );
  }
}
