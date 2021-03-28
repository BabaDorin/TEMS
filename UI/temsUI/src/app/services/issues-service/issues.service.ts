import { AddIssue } from 'src/app/models/communication/issues/add-issue.model';
import { Observable } from 'rxjs';
import { API_ISU_URL } from './../../models/backend.config';
import { TEMSService } from './../tems-service/tems.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';

@Injectable({
  providedIn: 'root'
})
export class IssuesService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  getIssues(
    equipmentId: string,
    roomId: string,
    personnelId: string,
    includingClosed: boolean,
    onlyClosed: boolean): Observable<ViewIssueSimplified[]>
  {
    return this.http.get<ViewIssueSimplified[]>(
      API_ISU_URL + '/gettickets/' + equipmentId + '/' + roomId + '/' + personnelId + '/' 
      + includingClosed + '/' + onlyClosed,
      this.httpOptions
    );
  }

  remove(issueId: string): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/remove/' + issueId,
      this.httpOptions
    );
  }

  getIssuesOfEntity(
    entityType: string, 
    entityId: string, 
    includingClosed: boolean,
    onlyClosed: boolean): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/getticketsofentity/' + entityType + '/' + entityId + '/' + includingClosed + '/' + onlyClosed,
      this.httpOptions
    );
  }

  getIssuesOfRoom(roomId: string, includingClosed?: boolean, onlyClosed?: boolean): Observable<any>{
    return this.getIssuesOfEntity(
      'room', 
      roomId, 
      onlyClosed ?? false, 
      includingClosed ?? false
      );
  }

  getIssuesOfEquipment(equipmentId: string, includingClosed?: boolean, onlyClosed?: boolean, ): Observable<any>{
    return this.getIssuesOfEntity(
      'equipment', 
      equipmentId, 
      includingClosed ?? false, 
      onlyClosed ?? false
      );
  }

  getIssuesOfPersonnel(personnelId: string, includingClosed?: boolean, onlyClosed?: boolean){
    return this.getIssuesOfEntity(
      'personnel', 
      personnelId, 
      includingClosed ?? false, 
      onlyClosed ?? false
      );
  }

  // getIssues(includingClosed?: boolean, onlyClosed?: boolean):Observable<any>{
  //   // all issues (tickets)
  //   return this.getIssuesOfEntity(
  //     'any',
  //     'any',
  //     includingClosed ?? false,
  //     onlyClosed ?? false
  //   )
  // }

  getStatuses(): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/getstatuses',
      this.httpOptions
    );
  }

  changeIssueStatus(issueId: string, statusId: string): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/changeStatus/' + issueId + '/' + statusId,
      this.httpOptions 
    );
  }

  closeIssue(issueId: string): Observable<any> {
    return this.http.get(
      API_ISU_URL + '/close/' + issueId,
      this.httpOptions 
    );
  }

  createIssue(addIssue: AddIssue): Observable<any>{
    return this.http.post(
      API_ISU_URL + '/create',
      addIssue,
      this.httpOptions
    );
  }
}
