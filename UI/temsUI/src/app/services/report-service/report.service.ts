import { API_REP_URL } from './../../models/backend.config';
import { TEMSService } from './../tems-service/tems.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddReportTemplate } from './../../models/report/add-report.model';
import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { Injectable } from '@angular/core';
import { GeneratedReport } from 'src/app/models/report/generated-report.model';

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

  getTemplates(): Observable<ViewReportSimplified[]>{
    return this.http.get<ViewReportSimplified[]>(
      API_REP_URL + '/gettemplates',
      this.httpOptions
    );
  }

  getLastGeneratedReports(): Observable<GeneratedReport[]>{
    return this.http.get<GeneratedReport[]>(
      API_REP_URL + '/getlastgeneratedreports',
      this.httpOptions
    );
  }

  updateReportTemplate(addReportTemplate: AddReportTemplate): Observable<any>{
    return this.http.post(
      API_REP_URL + '/updatetemplate',
      JSON.stringify(addReportTemplate),
      this.httpOptions
    );
  }

  addReportTemplate(addReportTemplate: AddReportTemplate): Observable<any>{
    return this.http.post(
      API_REP_URL + '/addtemplate',
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

  archieveTemplate(reportTemplateId: string, archivationStatus?: Boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved

    return this.http.get(
      API_REP_URL + '/archieveTemplate/' + reportTemplateId + '/' + archivationStatus,
      this.httpOptions
    );
  }

  generateReport(reportTemplateId: string): Observable<any>{
    return this.http.get(
      API_REP_URL + '/generatereport/' + reportTemplateId,
      {
        reportProgress: true,
        responseType: 'blob',
      }
    );
  }

  generateReportFromRawTemplate(addReportTemplateModel: AddReportTemplate): Observable<any>{
    return this.http.post(
      API_REP_URL + '/generatereportfromrawtemplate', 
      addReportTemplateModel,
      {
        reportProgress: true,
        responseType: 'blob',
      }
    )
  }

  getReport(reportId: string): Observable<any>{
    return this.http.get(
      API_REP_URL + '/getreport/' + reportId,
      {
        reportProgress: true,
        responseType: 'blob',
      }
    )
  }

  removeReport(reportId: string): Observable<any>{
    return this.http.get(
      API_REP_URL + '/removereport/' + reportId,
      this.httpOptions
    )
  }
}
