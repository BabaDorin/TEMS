import { NgxPaginationModule } from 'ngx-pagination';
import { TranslateModule } from '@ngx-translate/core';
import { ViewBugReportsComponent } from './../../../tems-components/view-bug-reports/view-bug-reports.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BugReportContainerModule } from '../bug-report-container/bug-report-container.module';



@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    ViewBugReportsComponent,
    BugReportContainerModule,
    TranslateModule,
    NgxPaginationModule
  ],
  exports: [
    ViewBugReportsComponent
  ]
})
export class BugReportListingModule { }
