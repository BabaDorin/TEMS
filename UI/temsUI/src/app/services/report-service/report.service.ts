import { Report } from './../../models/report/report.model';
import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  constructor() { }

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
}
