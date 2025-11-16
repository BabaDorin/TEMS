import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { SummaryEquipmentAnalyticsComponent } from './../../../tems-components/analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { AnalyticsModule } from './../../analytics/analytics.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    AnalyticsModule,
    TranslateModule,
    SummaryEquipmentAnalyticsComponent
  ],
  exports: [
    SummaryEquipmentAnalyticsComponent
  ]
})
export class EquipmentSummaryAnalyticsModule { }
