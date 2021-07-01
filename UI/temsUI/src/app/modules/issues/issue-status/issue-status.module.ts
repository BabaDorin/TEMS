import { IssueStatusComponent } from './../../../tems-components/issues/issue-status/issue-status.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@NgModule({
  declarations: [
    IssueStatusComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    IssueStatusComponent
  ]
})
export class IssueStatusModule { }
