import { AddLog } from '../models/communication/logs/add-log.model';
import { API_LOG_URL } from '../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { TEMSService } from './tems.service';

@Injectable({
  providedIn: 'root'
})
export class LogsService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  getEntityLogs(entityType: string, entityId: string, pageNumber?: number, itemsPerPage?: number): Observable<any>{
    return this.http.get(
      API_LOG_URL + '/getentitylogs/' + entityType + '/' + entityId + '/' + pageNumber + '/' + itemsPerPage,
      this.httpOptions
    );
  }

  remove(logId: string): Observable<any>{
    return this.http.get(
      API_LOG_URL + '/remove/' + logId,
      this.httpOptions
    );
  }

  getTotalItems(entityType: string, entityId: string): Observable<number>{
    return this.http.get<number>(
      API_LOG_URL + '/getitemsnumber/' + entityType + '/' + entityId,
      this.httpOptions
    );
  }  
  getLogsByEquipmentId(equipmentId: string, pageNumber: number, itemsPerPage: number): Observable<any>{
    return this.getEntityLogs('equipment', equipmentId, pageNumber, itemsPerPage);
  }

  getLogsByRoomId(roomId: string, pageNumber: number, itemsPerPage: number): Observable<any>{
    return this.getEntityLogs('room', roomId, pageNumber, itemsPerPage);
  }

  getLogsByPersonnelId(personnelId: string, pageNumber: number, itemsPerPage: number): Observable<any>{
    return this.getEntityLogs('personnel', personnelId, pageNumber, itemsPerPage);
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
