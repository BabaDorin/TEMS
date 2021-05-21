import { DatePipe } from '@angular/common';
import { PieChartData } from './../../models/analytics/pieChart-model';
import { API_ANALYTICS_URL, API_ANN_URL } from './../../models/backend.config';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TEMSService } from './../tems-service/tems.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService extends TEMSService {

  constructor(
    private http: HttpClient,
    private datePipe: DatePipe
  ) {
    super();
  }

  private buildEndpointAddresWithEntity(uri: string, entityType: string, entityId: string){
    let endPoint = uri;
    if(entityType != undefined)
      endPoint += '/' + entityType + '/' + entityId;

    return endPoint;
  }

  private buildParamsWithIntervals(start: Date, end: Date, interval: string){
    let params = new HttpParams();

    params = params.append('start', start.toDateString());
    params = params.append('end', end.toDateString());
    params = params.append('interval', interval);

    return params;
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
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getAmountOfCreatedTicketsOfEntity', entityType, entityId);

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

  getAmountOfOpenTickets(entityType: string, entityId: string): Observable<number>{
    let endPoint = this.buildEndpointAddresWithEntity(API_ANALYTICS_URL+ '/getAmountOfOpenTickets', entityType, entityId);

    return this.http.get<number>(
      endPoint,
      this.httpOptions
    );
  }

  getAmountOfLastCreatedTickets(start: Date, end: Date, interval: string): Observable<PieChartData>{
    let params = this.buildParamsWithIntervals(start, end, interval);
    return this.http.get<PieChartData>(
      API_ANALYTICS_URL + '/getAmountOfLastCreatedTickets', 
      { params: params }
    );
  }

  getAmountOfLastClosedTickets(start: Date, end: Date, interval: string): Observable<PieChartData>{
    let params = this.buildParamsWithIntervals(start, end, interval);
    return this.http.get<PieChartData>(
      API_ANALYTICS_URL + '/getAmountOfLastClosedTickets', 
      { params: params }
    );
  }
}
