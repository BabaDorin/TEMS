import { IOption } from './../models/option.model';
import { API_BUG_URL } from './../models/backend.config';
import { Observable } from 'rxjs';
import { BugReport, ViewBugReport } from './../models/bug-report/bug-report.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { TEMSService } from './tems.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BugReportService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  sendReport(report: BugReport): Observable<any>{
    const formData = new FormData();
    for (const prop in report) {
      if(prop == "attachments")
        continue;

      if (!report.hasOwnProperty(prop)) { continue; }
      formData.append(prop , report[prop]);
    }

    for(let i = 0; i < report.attachments.length; i++){
      formData.append("attachments" , report.attachments[i]);
    }

    console.log(formData["attachments"]);
    return this.http.post(
        API_BUG_URL + '/sendReport',
        formData
    );
  };

  getFullBugReport(reportId: string): Observable<ViewBugReport>{
    let params = new HttpParams()
      .append('reportId', reportId);
    
    return this.http.get<ViewBugReport>(
      API_BUG_URL + '/getFullReport',
      { params: params },
    )
  }

  fetchAttachment(reportId: string, attachmentIndex: number): Observable<any>{
    let params = new HttpParams()
      .append('reportId', reportId)
      .append('attachmentIndex', attachmentIndex.toString());

    return this.http.get(
      API_BUG_URL + '/fetchAttachment',
      { params: params,
        reportProgress: true,
        responseType: 'blob', 
      }
    );
  }

  getBugReportsSimplified(pageNumber: number, itemsPerPage: number): Observable<IOption[]>{

    let skip = (pageNumber - 1) * itemsPerPage;
    let take = itemsPerPage;

    let params = new HttpParams()
      .append('skip', skip.toString())
      .append('take', take.toString());
    
    return this.http.get<IOption[]>(
      API_BUG_URL + '/getBugReportsSimplified',
      { params: params }
    );
  }

  getTotalBugReportsAmount(): Observable<number>{
    return this.http.get<number>(
      API_BUG_URL + '/getTotalBugReportsAmount'
    );
  }

  removeReport(reportId: string): Observable<any>{
    let params = new HttpParams()
      .append('reportId', reportId);
    
    return this.http.delete(
      API_BUG_URL + '/removeReport',
      { params: params }
    )
  }
}
