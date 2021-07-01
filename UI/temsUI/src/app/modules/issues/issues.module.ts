import { TranslateModule } from '@ngx-translate/core';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { PinnedIssuesComponent } from './../../tems-components/issue/pinned-issues/pinned-issues.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { ViewIssuesComponent } from './../../tems-components/issue/view-issues/view-issues.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IssuesRoutingModule } from './issues-routing.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { AnalyticsModule } from '../analytics/analytics.module';
import { MatOptionModule } from '@angular/material/core';


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
