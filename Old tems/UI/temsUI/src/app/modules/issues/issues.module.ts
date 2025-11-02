import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { AnalyticsModule } from '../analytics/analytics.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { PinnedIssuesComponent } from './../../tems-components/issue/pinned-issues/pinned-issues.component';
import { ViewIssuesComponent } from './../../tems-components/issue/view-issues/view-issues.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { IssuesRoutingModule } from './issues-routing.module';



@NgModule({
  declarations: [
    ViewIssuesComponent,
    PinnedIssuesComponent,
  ],
  imports: [
    CommonModule,
    IssuesRoutingModule,
    MatIconModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule,
    TranslateModule,
    // Shared modules
    TemsFormsModule,
    EntitySharedModule,
    AnalyticsModule,
  ], 
  exports: [
    ViewIssuesComponent,
    PinnedIssuesComponent
  ]
})
export class IssuesModule { }
