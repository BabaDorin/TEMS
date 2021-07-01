import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { IssueContainerSimplifiedComponent } from '../../../tems-components/issues/issue-container-simplified/issue-container-simplified.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@NgModule({
  declarations: [
    IssueContainerSimplifiedComponent
  ],
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    TranslateModule
  ]
})
export class IssueContainerSimplifiedModule { }
