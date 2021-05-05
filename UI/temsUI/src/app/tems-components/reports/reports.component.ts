import { GeneratedReport } from './../../models/report/generated-report.model';
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
  lastGeneratedReports = [] as GeneratedReport[];
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
    this.fetchReportTemplates();
    this.fetchLastGeneratedReports();
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

  fetchReportTemplates(){
    this.subscriptions.push(
      this.reportService.getTemplates()
      .subscribe(result => {
        this.defaultTemplates = result.filter(q => q.isDefault == true);
        this.customTemplates = result.filter(q => q.isDefault == false);
      })
    )
  }

  fetchLastGeneratedReports(){
    this.subscriptions.push(
      this.reportService.getLastGeneratedReports()
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;
        
        this.lastGeneratedReports = result;
      })
    )
  }

  printFromGeneratedReport(repId: string){
    alert(repId);
  }

  removeGeneratedReport(repId: string){
    alert(repId);
  }

  createTemplate(){
    this.router.navigate(["/reports/createtemplate"]);
  }
}
