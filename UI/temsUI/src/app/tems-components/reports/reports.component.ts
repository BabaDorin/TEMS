import { TEMSComponent } from 'src/app/tems/tems.component';
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
export class ReportsComponent extends TEMSComponent implements OnInit {


  defaultTemplates: ViewReportSimplified[];
  customTemplates: ViewReportSimplified[];
  templates: ViewReportSimplified[];

  constructor(
    private reportService: ReportService,
    private router: Router,
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.reportService.getTemplates()
      .subscribe(result => {
        this.defaultTemplates = result.filter(q => q.isDefault == true);
        this.customTemplates = result.filter(q => q.isDefault == false);
      })
    )
  }

  edit(templateId: string){
    this.router.navigate(["/reports/updatetemplate/" + templateId]);
  }

  remove(templateId: string, index){
    // stuff
  }

  createTemplate(){
    this.router.navigate(["/reports/createtemplate"]);
  }
}
