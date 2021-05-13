import { ChartsModule } from 'ng2-charts';
import { PiechartCardComponent } from './../../tems-components/analytics/piechart-card/piechart-card.component';
import { SimpleInfoCardComponent } from './../../tems-components/analytics/simple-info-card/simple-info-card.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnalyticsRoutingModule } from './analytics-routing.module';


@NgModule({
  declarations: [
    SimpleInfoCardComponent,
    PiechartCardComponent
  ],
  imports: [
    CommonModule,
    AnalyticsRoutingModule,
    ChartsModule
  ],
  exports:[
    SimpleInfoCardComponent,
    PiechartCardComponent,
    ChartsModule
  ]
})
export class AnalyticsModule { }
