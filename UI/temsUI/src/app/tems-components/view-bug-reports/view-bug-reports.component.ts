import { SnackService } from './../../services/snack.service';
import { TEMSComponent } from './../../tems/tems.component';
import { BugReportService } from './../../services/bug-report.service';
import { IOption } from './../../models/option.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-bug-reports',
  templateUrl: './view-bug-reports.component.html',
  styleUrls: ['./view-bug-reports.component.scss']
})
export class ViewBugReportsComponent extends TEMSComponent implements OnInit {

  reports: IOption[];
  pageNumber = 1;
  itemsPerPage = 10;
  totalItems = 0;

  constructor(
    private bugReportService: BugReportService,
    private snack: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.fetchTotalBugReportsAmount();
    this.fetchBugReports();
  }

  fetchBugReports(){
    this.subscriptions.push(
      this.bugReportService.getBugReportsSimplified(this.pageNumber, this.itemsPerPage)
      .subscribe(result => {
        if(this.snack.snackIfError(result))
          return;

        console.log('reports fetched');
        console.log(this.reports);
        this.reports = result;
      })
    );
  }

  fetchTotalBugReportsAmount(){
    this.subscriptions.push(
      this.bugReportService.getTotalBugReportsAmount()
      .subscribe(result => {
        if(this.snack.snackIfError(result))
          return;

        this.totalItems = result;
      })
    )
  }

  reportRemoved(index: number){
    this.reports.splice(index, 1);
  }
}
