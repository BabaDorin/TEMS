import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatBadgeModule } from '@angular/material/badge';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { EquipmentSummaryAnalyticsModule } from './../summary-analytics/equipment-summary-analytics/equipment-summary-analytics.module';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { ViewRoomsComponent } from '../../tems-components/room/view-rooms/view-rooms.component';
import { AgGridModule } from 'ag-grid-angular';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RoomsRoutingModule } from './rooms-routing.module';
import { SummaryRoomsAnalyticsComponent } from '../../tems-components/analytics/summary-rooms-analytics/summary-rooms-analytics.component';
import { AddRoomComponent } from '../../tems-components/room/add-room/add-room.component';
import { RoomDetailsComponent } from '../../tems-components/room/room-details/room-details.component';
import { RoomDetailsLogsComponent } from '../../tems-components/room/room-details-logs/room-details-logs.component';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { RoomDetailsIssuesComponent } from '../../tems-components/room/room-details-issues/room-details-issues.component';
import { RoomDetailsAllocationsComponent } from '../../tems-components/room/room-details-allocations/room-details-allocations.component';
import { AnalyticsModule } from '../analytics/analytics.module';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    ViewRoomsComponent,
    AddRoomComponent,
    SummaryRoomsAnalyticsComponent,
    RoomDetailsComponent,
    RoomDetailsLogsComponent,
    RoomDetailsIssuesComponent,
    RoomDetailsAllocationsComponent,
  ],
  imports: [
    CommonModule,
    RoomsRoutingModule,
    AgGridModule,
    TemsFormsModule,
    ReactiveFormsModule,
    AnalyticsModule,
    MatProgressBarModule,
    MatMenuModule,
    MatIconModule,
    MatBadgeModule,
    TranslateModule,
    MatTabsModule,
    MatButtonModule,

    // Shared modules
    EntitySharedModule,
    EquipmentSummaryAnalyticsModule,
  ],
  exports: [
  ],
})
export class RoomsModule { }
