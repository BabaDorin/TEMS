import { Downloader } from './../../shared/downloader/fileDownloader';
import { TranslateService } from '@ngx-translate/core';
import { SnackService } from './../../services/snack.service';
import { BugReportService } from './../../services/bug-report.service';
import { TEMSComponent } from './../../tems/tems.component';
import { IOption } from './../../models/option.model';
import { ClaimService } from './../../services/claim.service';
import { ViewBugReport } from './../../models/bug-report/bug-report.model';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-bug-report-container',
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
    public translate: TranslateService
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
        console.log(this.bugReport);
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

  deleteReport(){
    if(!confirm('Are you sure you want to remove the specified bug report?'))
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
