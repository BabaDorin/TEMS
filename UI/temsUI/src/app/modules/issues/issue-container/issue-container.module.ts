import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatOptionModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IssueStatusComponent } from '../../../tems-components/issues/issue-status/issue-status.component';
import { IssueStatusModule } from '../issue-status/issue-status.module';
import { IssueContainerSimplifiedComponent } from './../../../tems-components/issues/issue-container-simplified/issue-container-simplified.component';
import { IssueContainerComponent } from './../../../tems-components/issues/issue-container/issue-container.component';
import { TemsFormsModule } from './../../tems-forms/tems-forms.module';


@NgModule({
  declarations: [
    IssueContainerComponent,
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
    TranslateModule,
    IssueStatusModule,
  ],
  exports: [
    IssueContainerComponent,
  ]
})
export class IssueContainerModule { }
