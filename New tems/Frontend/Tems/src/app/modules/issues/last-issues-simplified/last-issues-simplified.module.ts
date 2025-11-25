import { TranslateModule } from '@ngx-translate/core';
import { IssueSimplifiedModule } from './../issue-simplified/issue-simplified.module';
import { LastIssuesSimplifiedComponent } from './../../../tems-components/analytics/last-issues-simplified/last-issues-simplified.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    IssueSimplifiedModule,
    TranslateModule,
    LastIssuesSimplifiedComponent
  ],
  exports: [
    LastIssuesSimplifiedComponent
  ]
})
export class LastIssuesSimplifiedModule { }
