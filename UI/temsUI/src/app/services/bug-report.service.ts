import { API_BUG_URL } from './../models/backend.config';
import { Observable } from 'rxjs';
import { BugReport } from './../models/bug-report/bug-report.model';
import { HttpClient } from '@angular/common/http';
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
  }
}
