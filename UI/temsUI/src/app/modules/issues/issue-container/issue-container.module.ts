import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { IssueStatusComponent } from './../../../issues/issue-status/issue-status.component';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { IssueContainerSimplifiedComponent } from './../../../tems-components/issues/issue-container-simplified/issue-container-simplified.component';
import { IssueContainerComponent } from './../../../tems-components/issues/issue-container/issue-container.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';



@NgModule({
  declarations: [
    IssueContainerComponent,
    IssueContainerSimplifiedComponent,
    IssueStatusComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatExpansionModule,
    MatCardModule,
    MatIconModule,
  ],
  exports: [
    IssueContainerComponent,
    IssueContainerSimplifiedComponent,
    IssueStatusComponent
  ]
})
export class IssueContainerModule { }
