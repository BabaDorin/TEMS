import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { ChartsModule } from 'ng2-charts';
import { FractionCardComponent } from './../../tems-components/analytics/fraction-card/fraction-card.component';
import { PiechartCardComponent } from './../../tems-components/analytics/piechart-card/piechart-card.component';
import { SimpleInfoCardComponent } from './../../tems-components/analytics/simple-info-card/simple-info-card.component';
import { AnalyticsRoutingModule } from './analytics-routing.module';


@NgModule({
  declarations: [
    SimpleInfoCardComponent,
    PiechartCardComponent,
    FractionCardComponent,
  ],
  imports: [
    CommonModule,
    AnalyticsRoutingModule,
    ChartsModule,
    TranslateModule,
    MatProgressBarModule,
  ],
  exports:[
    SimpleInfoCardComponent,
    PiechartCardComponent,
    ChartsModule,
    FractionCardComponent,
    MatProgressBarModule,
  ]
})
export class AnalyticsModule { }
