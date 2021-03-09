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

  getIssues(onlyClosed?: boolean, includingClosed?: boolean){
    return [
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
    ];
  }

  getIssuesOfRoom(roomId: string, onlyClosed?: boolean, includingClosed?: boolean){
    return this.getIssues(onlyClosed, includingClosed);
  }

  getIssuesOfEquipment(equipmentId: string, includingClosed?: boolean, onlyClosed?: boolean, ): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/equipment/' + equipmentId + '/' + includingClosed + '/' + onlyClosed,
      this.httpOptions
    );
  }

  getIssuesOfPersonnel(personnelId: string, onlyClosed?: boolean, includingClosed?: boolean){
    return this.getIssues(onlyClosed, includingClosed);
  }

  getStatuses(): Observable<any>{
    return this.http.get(
      API_ISU_URL + '/getstatuses',
      this.httpOptions
    );
  }
}
