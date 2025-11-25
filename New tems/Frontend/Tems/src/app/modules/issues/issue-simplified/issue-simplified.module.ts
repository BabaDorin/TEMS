import { IssueContainerSimplifiedModule } from './../issue-container-simplified/issue-container-simplified.module';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { MatCardModule } from '@angular/material/card';
import { IssueStatusModule } from './../issue-status/issue-status.module';
import { IssueContainerSimplifiedComponent } from './../../../tems-components/issues/issue-container-simplified/issue-container-simplified.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  imports: [
    CommonModule,
    IssueStatusModule,
    MatIconModule,
    RouterModule,
    MatCardModule,
    TranslateModule,
    MatButtonModule,
    IssueContainerSimplifiedModule
  ],
  exports: [
    IssueContainerSimplifiedComponent
  ]
})
export class IssueSimplifiedModule { }
