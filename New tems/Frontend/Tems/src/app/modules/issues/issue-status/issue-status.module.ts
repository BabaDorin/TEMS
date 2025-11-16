import { TranslateModule } from '@ngx-translate/core';
import { IssueStatusComponent } from './../../../tems-components/issues/issue-status/issue-status.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    IssueStatusComponent,
    TranslateModule
  ],
  exports: [
    IssueStatusComponent
  ]
})
export class IssueStatusModule { }
