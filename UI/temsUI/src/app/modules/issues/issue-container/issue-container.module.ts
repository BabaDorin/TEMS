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
import { IssueContainerComponent } from './../../../tems-components/issues/issue-container/issue-container.component';
import { IssueSimplifiedModule } from './../issue-simplified/issue-simplified.module';
import { IssueStatusModule } from './../issue-status/issue-status.module';



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
    TranslateModule,
    IssueSimplifiedModule,
    IssueStatusModule
  ]
})
export class IssueContainerModule { }
