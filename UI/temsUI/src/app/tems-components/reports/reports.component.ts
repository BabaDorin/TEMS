import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { ReportService } from './../../services/report-service/report.service';
import { Report } from './../../models/report/report.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {


  defaultTemplates: ViewReportSimplified[];
  customTemplates: ViewReportSimplified[];

  constructor(
    private reportService: ReportService
  ) { }

  ngOnInit(): void {
    this.defaultTemplates = this.reportService.getDefaultTemplates();
    this.customTemplates = this.reportService.getCustomTemplates();  
  }
}
