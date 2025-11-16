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
import { TEMS_FORMS_IMPORTS } from './../tems-forms/tems-forms.module';
import { IssuesRoutingModule } from './issues-routing.module';

@NgModule({
  declarations: [
  ],
  imports: [
    ViewIssuesComponent,
    PinnedIssuesComponent,
    CommonModule,
    IssuesRoutingModule,
    MatIconModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule,
    TranslateModule,
    ...TEMS_FORMS_IMPORTS,
    EntitySharedModule,
    AnalyticsModule,
  ], 
  exports: [
    ViewIssuesComponent,
    PinnedIssuesComponent
  ]
})
export class IssuesModule { }

export const ISSUES_IMPORTS = [
  CommonModule,
  MatIconModule,
  MatTabsModule,
  MatFormFieldModule,
  MatInputModule,
  MatSelectModule,
  MatOptionModule,
  TranslateModule,
];
