import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { IssueContainerSimplifiedComponent } from './../../../tems-components/issues/issue-container-simplified/issue-container-simplified.component';
import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IssueStatusModule } from '../issue-status/issue-status.module';



@NgModule({
  declarations: [
    IssueContainerSimplifiedComponent
  ],
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    TranslateModule,
    IssueStatusModule,
    RouterModule,
    MatIconModule,
  ],
  exports:[
    IssueContainerSimplifiedComponent
  ]
})
export class IssueContainerSimplifiedModule { }
