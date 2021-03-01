// import { SummaryIssuesAnalyticsComponent } from 'src/app/tems-components/analytics/summary-issues-analytics/summary-issues-analytics.component';
import { SummaryEquipmentAnalyticsComponent } from './../../tems-components/analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AnalyticsRoutingModule } from './analytics-routing.module';
import { SummaryPersonnelAnalyticsComponent } from 'src/app/tems-components/analytics/summary-personnel-analytics/summary-personnel-analytics.component';


@NgModule({
  declarations: [
    // SummaryEquipmentAnalyticsComponent,
    // SummaryIssuesAnalyticsComponent,
    // SummaryPersonnelAnalyticsComponent
  ],
  imports: [
    CommonModule,
    AnalyticsRoutingModule
  ],
  exports:[
    // SummaryEquipmentAnalyticsComponent,
    // SummaryIssuesAnalyticsComponent,
    // SummaryPersonnelAnalyticsComponent
  ]
})
export class AnalyticsModule { }
