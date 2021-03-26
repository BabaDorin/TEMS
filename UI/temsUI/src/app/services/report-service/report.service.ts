import { API_REP_URL } from './../../models/backend.config';
import { TEMSService } from './../tems-service/tems.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddReportTemplate } from './../../models/report/add-report.model';
import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ReportService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  getCustomTemplates(){
    let custom = [
      new ViewReportSimplified(), // TODO: Interface to display default and, after that, custom report templates
      new ViewReportSimplified(), // TODO: Interface to display default and, after that, custom report templates
    ];

    custom.forEach(element => {
      element.isDefault = false;
    });

    return custom;
  }

  getDefaultTemplates(){
    let def = [
      new ViewReportSimplified(), // TODO: Interface to display default and, after that, custom report templates
      new ViewReportSimplified(), // TODO: Interface to display default and, after that, custom report templates
      new ViewReportSimplified(), // TODO: Interface to display default and, after that, custom report templates
      new ViewReportSimplified(), // TODO: Interface to display default and, after that, custom report templates
    ];

    def.forEach(element => {
      element.isDefault = true;
    });

    return def;
  }

  addReportTemplate(addReportTemplate: AddReportTemplate): Observable<any>{
    return this.http.post(
      API_REP_URL + '/addTemplate',
      JSON.stringify(addReportTemplate),
      this.httpOptions
    );
  }

  getReportTemplateToUpdate(reportTemplateId: string): Observable<AddReportTemplate>{
    return this.http.get<AddReportTemplate>(
      API_REP_URL + '/gettemplatetoupdate/' + reportTemplateId,
      this.httpOptions
    );
  }
}
