import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { TemsFormsModule } from './../../tems-forms/tems-forms.module';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { IssueStatusComponent } from '../../../tems-components/issues/issue-status/issue-status.component';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { IssueContainerSimplifiedComponent } from './../../../tems-components/issues/issue-container-simplified/issue-container-simplified.component';
import { IssueContainerComponent } from './../../../tems-components/issues/issue-container/issue-container.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatOptionModule } from '@angular/material/core';



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
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule,
    MatIconModule,
    TemsFormsModule,
  ],
  exports: [
    IssueContainerComponent,
    IssueContainerSimplifiedComponent,
    IssueStatusComponent
  ]
})
export class IssueContainerModule { }
