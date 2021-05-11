import { API_ANALYTICS_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TEMSService } from './../tems-service/tems.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  getEquipmentAmount(entityType?: string, entityId?: string): Observable<number>{
    let endPoint = API_ANALYTICS_URL+ '/getequipmentamount';
    if(entityType != undefined)
      endPoint += '/' + entityType + '/' + entityId;

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }
}
