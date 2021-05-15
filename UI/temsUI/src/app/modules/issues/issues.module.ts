import { PinnedIssueComponent } from './../../tems-components/issue/pinned-issue/pinned-issue.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { ViewIssuesComponent } from './../../tems-components/issue/view-issues/view-issues.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IssuesRoutingModule } from './issues-routing.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { AnalyticsModule } from '../analytics/analytics.module';


@NgModule({
  declarations: [
    ViewIssuesComponent,
    PinnedIssueComponent
  ],
  imports: [
    CommonModule,
    IssuesRoutingModule,

    // Shared modules
    TemsFormsModule,
    EntitySharedModule,
    AnalyticsModule,
    MaterialModule,
  ],
})
export class IssuesModule { }
