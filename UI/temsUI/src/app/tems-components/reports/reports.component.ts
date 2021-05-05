import { Downloader } from './../../shared/downloader/fileDownloader';
import { SnackService } from './../../services/snack/snack.service';
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
  downloader: Downloader;
  
  constructor(
    private reportService: ReportService,
    private router: Router,
    private snackService: SnackService
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
    if(this.downloader == undefined) this.downloader = new Downloader();

    this.subscriptions.push(
      this.reportService.generateReport(templateId)
      .subscribe(event => {
        if(this.snackService.snackIfError(event))
          return;
        this.downloader.downloadFile(event, "Report.xlsx");
      })
    )
  }

  createTemplate(){
    this.router.navigate(["/reports/createtemplate"]);
  }
}
