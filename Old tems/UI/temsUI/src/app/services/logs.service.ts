import { LogFilter } from './../helpers/filters/log.filter';
import { AddLog } from '../models/communication/logs/add-log.model';
import { API_LOG_URL } from '../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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

  // API Route changed
  // api post => accepts filter
  // Also, there is a case when an empty filter is provided, which should return all of the allocations (Well, unless there aren't any default filters)
  getEntityLogs(filter: LogFilter): Observable<any>{
    return this.http.post(
      API_LOG_URL + '/getLogs',
      JSON.stringify(filter),
      this.httpOptions
    );
  }

  remove(logId: string): Observable<any>{
    return this.http.get(
      API_LOG_URL + '/remove/' + logId,
      this.httpOptions
    );
  }

  // api => post, accepts filter
  getTotalItems(filter: LogFilter): Observable<number>{
    return this.http.post<number>(
      API_LOG_URL + '/getAmountOfLogs',
      JSON.stringify(filter),
      this.httpOptions
    );
  }  

  getLogs(): Observable<any>{
    return this.getEntityLogs(new LogFilter());
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
