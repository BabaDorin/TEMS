import { TEMSComponent } from 'src/app/tems/tems.component';
import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { ReportService } from './../../services/report-service/report.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent extends TEMSComponent implements OnInit {

  defaultTemplates: ViewReportSimplified[];
  customTemplates: ViewReportSimplified[];
  templates: ViewReportSimplified[];
  pageNumber = 1;
  
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

  templateRemoved(template){
    let index = this.customTemplates.indexOf(template);
    if(index == -1)
      return;

    this.customTemplates.splice(index, 1);
  }

  generateReport(templateId: string){
    console.log('got it');
    this.subscriptions.push(
      this.reportService.generateReport(templateId)
      .subscribe(result => {
        console.log(result);
      })
    )
  }

  createTemplate(){
    this.router.navigate(["/reports/createtemplate"]);
  }
}
