import { SnackService } from './../../services/snack.service';
import { TEMSComponent } from './../../tems/tems.component';
import { BugReportService } from './../../services/bug-report.service';
import { IOption } from './../../models/option.model';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { TranslateModule } from '@ngx-translate/core';
import { BugReportContainerComponent } from '../bug-report-container/bug-report-container.component';
import { NgxPaginationModule } from 'ngx-pagination';

@Component({
  selector: 'app-view-bug-reports',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatExpansionModule,
    TranslateModule,
    BugReportContainerComponent,
    NgxPaginationModule
  ],
  templateUrl: './view-bug-reports.component.html',
  styleUrls: ['./view-bug-reports.component.scss']
})
export class ViewBugReportsComponent extends TEMSComponent implements OnInit, OnDestroy {

  reports: IOption[];
  pageNumber = 1;
  itemsPerPage = 10;
  totalItems = 0;
  subscriptions: any[] = [];

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

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => {
      try { s?.unsubscribe?.(); } catch (e) { /* ignore cleanup errors */ }
    });
  }
}