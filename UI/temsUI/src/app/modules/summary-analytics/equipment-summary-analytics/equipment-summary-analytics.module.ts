import { AnalyticsModule } from './../../analytics/analytics.module';
import { SummaryEquipmentAnalyticsComponent } from './../../../tems-components/analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    SummaryEquipmentAnalyticsComponent
  ],
  imports: [
    CommonModule,
    AnalyticsModule,
  ],
  exports: [
    SummaryEquipmentAnalyticsComponent
  ]
})
export class EquipmentSummaryAnalyticsModule { }