import { TranslateService } from '@ngx-translate/core';
import { SnackService } from './../../services/snack.service';
import { BugReportService } from './../../services/bug-report.service';
import { TEMSComponent } from './../../tems/tems.component';
import { IOption } from './../../models/option.model';
import { ClaimService } from './../../services/claim.service';
import { ViewBugReport } from './../../models/bug-report/bug-report.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-bug-report-container',
  templateUrl: './bug-report-container.component.html',
  styleUrls: ['./bug-report-container.component.scss']
})
export class BugReportContainerComponent extends TEMSComponent implements OnInit {

  @Input() bugReportSimplified: IOption;
  bugReport: ViewBugReport;

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
    this.subscriptions.push(
      this.bugReportService.getFullBugReport(this.bugReportSimplified.value)
      .subscribe(result => {
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
        
        console.log(result);
      })
    );
  }

  deleteReport(){
    if(!confirm('Are you sure you want to remove the specified bug report?'))
      return;
    
    this.subscriptions.push(
      this.bugReportService.removeReport(this.bugReport.id)
      .subscribe(result => {
        this.snack.snackIfError(result);
      })
    );
  }
}
