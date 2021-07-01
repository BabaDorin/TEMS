import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CAN_MANAGE_ENTITIES } from 'src/app/models/claims';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { ReportService } from '../../services/report.service';
import { SnackService } from '../../services/snack.service';
import { TokenService } from '../../services/token.service';
import { GeneratedReport } from './../../models/report/generated-report.model';
import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { Downloader } from './../../shared/downloader/fileDownloader';

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
  templatePageNumber = 1;
  generatedReportsPageNumber = 1;
  downloader: Downloader;
  canManage = false;
  
  constructor(
    private reportService: ReportService,
    private router: Router,
    private snackService: SnackService,
    private tokenService: TokenService
  ) {
    super();
  }

  ngOnInit(): void {
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);
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

  removeGeneratedReport(index: number){
    this.lastGeneratedReports.splice(index, 1);
  }

  createTemplate(){
    this.router.navigate(["/reports/createtemplate"]);
  }

  remove(eventData, index){
    // to be implemented or removed completely
  }
}
