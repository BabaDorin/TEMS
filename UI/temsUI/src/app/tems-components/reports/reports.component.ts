import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { ReportService } from './../../services/report-service/report.service';
import { Report } from './../../models/report/report.model';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {


  defaultTemplates: ViewReportSimplified[];
  customTemplates: ViewReportSimplified[];

  constructor(
    private reportService: ReportService,
    private router: Router,
    private activatedroute: ActivatedRoute,
    private http: HttpClient
  ) {

  }

  ngOnInit(): void {
    this.defaultTemplates = this.reportService.getDefaultTemplates();
    this.customTemplates = this.reportService.getCustomTemplates();
  }

  edit(templateId: string){
    this.router.navigate(["/reports/updatereport/" + templateId]);
  }

  remove(templateId: string){
    // stuff
  }

  createTemplate(){
    this.router.navigate(["/reports/createtemplate"]);
  }
}
