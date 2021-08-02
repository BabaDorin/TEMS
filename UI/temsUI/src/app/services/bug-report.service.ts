import { IOption } from './../models/option.model';
import { API_BUG_URL } from './../models/backend.config';
import { Observable, of } from 'rxjs';
import { BugReport, ViewBugReport } from './../models/bug-report/bug-report.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { TEMSService } from './tems.service';
import { Injectable } from '@angular/core';
import { param } from 'jquery';

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
    // let params = new HttpParams();
    // params.append('reportId', reportId);
    
    // return this.http.get<ViewBugReport>(
    //   API_BUG_URL + '/getfullbugreport',
    //   { params: params },
    // )

    let bugRep = new ViewBugReport();
    bugRep.id = '1';
    bugRep.createdBy = { label: 'Baby Dory', value: '1' },
    bugRep.dateCreated = new Date();
    bugRep.description = ' the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lore'
    bugRep.attachments = ['img3.jpg', 'analysis.pdf']
    return of(bugRep)
  }

  fetchAttachment(reportId: string, attachmentIndex: number): Observable<any>{
    let params = new HttpParams();
    params.append('reportId', reportId);
    params.append('attachmentIndex', attachmentIndex.toString());

    return this.http.get(
      API_BUG_URL + '/fetchAttachment',
      { params: params }
    );
  }

  getBugReportsSimplified(pageNumber: number, itemsPerPage: number): Observable<IOption[]>{
    // let params = new HttpParams();
    // params.append('pageNumber', pageNumber.toString());
    // params.append('itemsPerPage', itemsPerPage.toString());
    
    // return this.http.get<IOption[]>(
    //   API_BUG_URL + '/getBugReportsSimplified',
    //   { params: params }
    // );

    return of([
      { label: 'Bug rep 1', value: '1', additional: '13.03.2021  12:00' },
      { label: 'Bug rep 2', value: '2', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
      { label: 'Feature request 3', value: '3', additional: '13.03.2021  12:00' },
    ] as IOption[])
  }

  getTotalBugReportsAmount(): Observable<number>{
    // return this.http.get<number>(
    //   API_BUG_URL + '/getTotalBugReportsAmount'
    // );

    return of(22);
  }

  removeReport(reportId: string): Observable<any>{
    return of('not implemented yet');
  }
}
