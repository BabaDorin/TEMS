import { PieChartData } from './../../models/analytics/pieChart-model';
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

  getEquipmentTotalCost(entityType?: string, entityId?: string): Observable<number>{
    let endPoint = API_ANALYTICS_URL+ '/getEquipmentTotalCost';
    if(entityType != undefined)
      endPoint += '/' + entityType + '/' + entityId;

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }

  getEquipmentUtilizationRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = API_ANALYTICS_URL+ '/getEquipmentUtilizationRate';
    if(entityType != undefined)
      endPoint += '/' + entityType + '/' + entityId;

    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }

  getEquipmentTypeRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = API_ANALYTICS_URL+ '/getequipmenttyperate';
    if(entityType != undefined)
      endPoint += '/' + entityType + '/' + entityId;

    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }
}
