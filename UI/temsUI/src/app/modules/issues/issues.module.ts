import { ViewIssuesComponent } from './../../tems-components/issue/view-issues/view-issues.component';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IssuesRoutingModule } from './issues-routing.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { AnalyticsModule } from '../analytics/analytics.module';


@NgModule({
  declarations: [
    // CreateIssueComponent,
    ViewIssuesComponent,
  ],
  imports: [
    CommonModule,
    IssuesRoutingModule,

    // Shared modules
    EntitySharedModule,
    AnalyticsModule,
  ]
})
export class IssuesModule { }
