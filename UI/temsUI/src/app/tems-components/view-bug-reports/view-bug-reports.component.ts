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
    this.fetchBugReports();
  }

  fetchBugReports(
    appendToExistingReports: boolean = false, 
    skip: number = (this.pageNumber - 1) * this.itemsPerPage,
    take: number = this.itemsPerPage){

    this.fetchTotalBugReportsAmount();
    this.subscriptions.push(
      this.bugReportService.getBugReportsSimplified(skip, take)
      .subscribe(result => {
        if(this.snack.snackIfError(result))
          return;

        if(appendToExistingReports)
          this.reports = this.reports.concat(result);
        else
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
    this.totalItems--;

    // Fetch one more
    let skip = this.pageNumber * this.itemsPerPage - 1;
    if(skip < 0) skip = 0;
    let take = 1;

    this.fetchBugReports(true, skip, take);
  }
}