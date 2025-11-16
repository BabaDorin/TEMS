import { Downloader } from './../../shared/downloader/fileDownloader';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { SnackService } from './../../services/snack.service';
import { BugReportService } from './../../services/bug-report.service';
import { TEMSComponent } from './../../tems/tems.component';
import { IOption } from './../../models/option.model';
import { ClaimService } from './../../services/claim.service';
import { ViewBugReport } from './../../models/bug-report/bug-report.model';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ConfirmService } from 'src/app/confirm.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-bug-report-container',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatExpansionModule,
    MatProgressBarModule,
    TranslateModule
  ],
  templateUrl: './bug-report-container.component.html',
  styleUrls: ['./bug-report-container.component.scss']
})
export class BugReportContainerComponent extends TEMSComponent implements OnInit {

  @Input() bugReportSimplified: IOption;
  bugReport: ViewBugReport;
  loading: boolean = false;
  downloader: Downloader;
  
  @Output() removed = new EventEmitter();

  constructor(
    public claims: ClaimService,
    private bugReportService: BugReportService,
    private snack: SnackService,
    public translate: TranslateService,
    private confirmService: ConfirmService
  ) {
    super();
  }

  ngOnInit(): void {
  }

  fetchFullBugReport(){
    if(this.bugReport != undefined)
      return;

    this.loading = true;
    this.subscriptions.push(
      this.bugReportService.getFullBugReport(this.bugReportSimplified.value)
      .subscribe(result => {
        this.loading = false;
        if(this.snack.snackIfError(result))
          return;

        this.bugReport = result;
      })
    );
  }

  fetchFile(i: number){
    this.subscriptions.push(
      this.bugReportService.fetchAttachment(this.bugReport.id, i)
      .subscribe(result => {
        if(this.snack.snackIfError(result))
          return;

        if(this.downloader == undefined)
          this.downloader = new Downloader();

        this.downloader.downloadFile(result, this.bugReport.attachments[i]);
      })
    );
  }

  async deleteReport(){
    if(!await this.confirmService.confirm('Are you sure you want to remove the specified bug report?'))
      return;
    
    this.subscriptions.push(
      this.bugReportService.removeReport(this.bugReport.id)
      .subscribe(result => {
        if(this.snack.snackIfError(result))
          return;
        
        if(result.status == 1)
          this.removed.emit();
      })
    );
  }
}
