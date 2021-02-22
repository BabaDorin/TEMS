import { ViewRoomsComponent } from './../tems-components/room/view-rooms/view-rooms.component';
import { AgGridModule } from 'ag-grid-angular';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RoomsRoutingModule } from './rooms-routing.module';
import { SummaryRoomsAnalyticsComponent } from '../tems-components/analytics/summary-rooms-analytics/summary-rooms-analytics.component';
import { AgGridRoomsComponent } from '../tems-components/room/ag-grid-rooms/ag-grid-rooms.component';


@NgModule({
  declarations: [
    SummaryRoomsAnalyticsComponent,
    AgGridRoomsComponent,
    ViewRoomsComponent,
  ],
  imports: [
    CommonModule,
    RoomsRoutingModule,
    AgGridModule,
  ]
})
export class RoomsModule { }
