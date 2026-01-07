import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { TEMSComponent } from '../../tems/tems.component';
import { ReportService } from '../../services/report.service';
import { SnackService } from '../../services/snack.service';
import { TokenService } from '../../services/token.service';
import { GeneratedReport } from './../../models/report/generated-report.model';
import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { Downloader } from './../../shared/downloader/fileDownloader';
import { ReportContainerComponent } from './report-container/report-container.component';
import { GeneratedReportContainerComponent } from './generated-report-container/generated-report-container.component';
import { CommonModule } from '@angular/common';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatTooltipModule,
    TranslateModule,
    NgxPaginationModule,
    ReportContainerComponent,
    GeneratedReportContainerComponent
  ],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent extends TEMSComponent implements OnInit, OnDestroy {

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
    private tokenService: TokenService,
    public translate: TranslateService
  ) {
    super();
  }

  ngOnInit(): void {
    this.canManage = this.tokenService.canManageAssets();
    this.fetchReportTemplates();
    this.fetchLastGeneratedReports();
  }


  templateRemoved(templateId: string){
    const index = this.customTemplates.findIndex(t => t.id === templateId);
    if(index === -1)
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

  remove(templateId: string, index: number){
    if (!templateId) {
      return;
    }

    const defaultIdx = this.defaultTemplates?.findIndex(t => t.id === templateId) ?? -1;
    if (defaultIdx !== -1) {
      this.defaultTemplates.splice(defaultIdx, 1);
    }

    const customIdx = this.customTemplates?.findIndex(t => t.id === templateId) ?? -1;
    if (customIdx !== -1) {
      this.customTemplates.splice(customIdx, 1);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
