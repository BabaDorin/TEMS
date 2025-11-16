import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { NgChartsModule } from 'ng2-charts';
import { FractionCardComponent } from './../../tems-components/analytics/fraction-card/fraction-card.component';
import { LastCreatedTicketsChartComponent } from './../../tems-components/analytics/last-created-tickets-chart/last-created-tickets-chart.component';
import { PiechartCardComponent } from './../../tems-components/analytics/piechart-card/piechart-card.component';
import { SimpleInfoCardComponent } from './../../tems-components/analytics/simple-info-card/simple-info-card.component';
import { AnalyticsRoutingModule } from './analytics-routing.module';


@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    AnalyticsRoutingModule,
    NgChartsModule,
    LastCreatedTicketsChartComponent,
    TranslateModule,
    MatProgressBarModule,
    SimpleInfoCardComponent,
    FractionCardComponent,
    PiechartCardComponent,
  ],
  exports:[
    SimpleInfoCardComponent,
    PiechartCardComponent,
    NgChartsModule,
    FractionCardComponent,
    LastCreatedTicketsChartComponent,
    MatProgressBarModule,
  ]
})
export class AnalyticsModule { }
