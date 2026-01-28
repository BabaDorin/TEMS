import { AddIssue } from 'src/app/models/communication/issues/add-issue.model';
import { Observable } from 'rxjs';
import { API_ISU_URL } from '../models/backend.config';
import { TEMSService } from './tems.service';
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

  remove(issueId: string): Observable<any>{
    return this.http.delete(
      API_ISU_URL + '/remove/' + issueId,
      this.httpOptions
    );
  }

  getIssues(
    assetId: string,
    roomId: string,
    personnelId: string,
    includingClosed: boolean,
    onlyClosed: boolean): Observable<ViewIssueSimplified[]>
  {
    return this.http.get<ViewIssueSimplified[]>(
      API_ISU_URL + '/gettickets/' + assetId + '/' + roomId + '/' + personnelId + '/' 
      + includingClosed + '/' + onlyClosed,
      this.httpOptions
    );
  }

  archieve(issueId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved

    return this.http.get(
      API_ISU_URL + '/archieve/' + issueId + '/' + archivationStatus,
      this.httpOptions
    );
  }

  getIssuesOfEntity(
    entityType: string, 
    entityId: string, 
    includingClosed: boolean,
    onlyClosed: boolean,
    orderBy?: string,
    skip?: number,
    take?: number): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/getticketsofentity/' + entityType + '/' + entityId + '/' + includingClosed + '/' + onlyClosed + '/' + orderBy + '/' + skip + '/' + take,
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

  getIssuesOfEquipment(assetId: string, includingClosed?: boolean, onlyClosed?: boolean, ): Observable<any>{
    return this.getIssuesOfEntity(
      'asset', 
      assetId, 
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

  reopenIssue(issueId: string): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/reopen/' + issueId,
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

  changePinStatus(ticketId: string, pinStatus: boolean): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/changePinStatus/' + ticketId + '/' + pinStatus,
      this.httpOptions
    );
  }

  getPinnedTickets(): Observable<ViewIssueSimplified[]>{
    return this.http.get<ViewIssueSimplified[]>(
      API_ISU_URL + '/getpinnedtickets',
      this.httpOptions
    );
  }
}
