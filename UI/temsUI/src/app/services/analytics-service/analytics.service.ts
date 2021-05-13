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

  private buildEndpointAddresWithEntity(uri: string, entityType: string, entityId: string){
    let endPoint = uri;
    if(entityType != undefined)
      endPoint += '/' + entityType + '/' + entityId;

    return endPoint;
  }

  getEquipmentAmount(entityType?: string, entityId?: string): Observable<number>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getequipmentamount', entityType, entityId);

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }

  getEquipmentTotalCost(entityType?: string, entityId?: string): Observable<number>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getEquipmentTotalCost', entityType, entityId);

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }

  getEquipmentUtilizationRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getEquipmentUtilizationRate', entityType, entityId);

    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }

  getEquipmentTypeRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getEquipmentTypeRate', entityType, entityId);

    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }

  getEquipmentAllocationRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getEquipmentAllocationRate', entityType, entityId);

    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }

  getEquipmentWorkabilityRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getEquipmentWorkabilityRate', entityType, entityId);
    
    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }

  getTicketClosingRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getticketclosingrate', entityType, entityId);
    
    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }

  getTicketClosingByRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getTicketClosingByRate', entityType, entityId);
    
    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }

  getOpenTicketStatusRate(entityType?: string, entityId?: string): Observable<PieChartData>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getOpenTicketStatusRate', entityType, entityId);
    
    return this.http.get<PieChartData>(
      endPoint,
      this.httpOptions
    );
  }

  getAmountOfCreatedIssues(entityType?: string, entityId?: string): Observable<number>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getAmountOfCreatedTickets', entityType, entityId);

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }

  getAmountOfClosedIssues(entityType: string, entityId: string): Observable<number>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getAmountOfClosedTickets', entityType, entityId);

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }

  getAmountOfTicketsEverCreatedByUser(userId: string): Observable<number>{
    let endPoint = API_ANALYTICS_URL+ '/getAmountOfTicketsEverClosedByUser/' + userId;

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }

  getAmountOfTicketsClosedByUserThatWereReopenedAfterwards(userId: string){
    let endPoint = API_ANALYTICS_URL+ 
    '/getAmountOfTicketsClosedByUserThatWereReopenedAfterwards/' + userId;

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }
}
