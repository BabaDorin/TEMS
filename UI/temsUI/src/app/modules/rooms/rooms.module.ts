import { IncludeEquipmentLabelsModule } from './../equipment/include-equipment-tags.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { AgGridModule } from 'ag-grid-angular';
import { SummaryRoomsAnalyticsComponent } from '../../tems-components/analytics/summary-rooms-analytics/summary-rooms-analytics.component';
import { AddRoomComponent } from '../../tems-components/room/add-room/add-room.component';
import { RoomDetailsAllocationsComponent } from '../../tems-components/room/room-details-allocations/room-details-allocations.component';
import { RoomDetailsIssuesComponent } from '../../tems-components/room/room-details-issues/room-details-issues.component';
import { RoomDetailsLogsComponent } from '../../tems-components/room/room-details-logs/room-details-logs.component';
import { RoomDetailsComponent } from '../../tems-components/room/room-details/room-details.component';
import { ViewRoomsComponent } from '../../tems-components/room/view-rooms/view-rooms.component';
import { AnalyticsModule } from '../analytics/analytics.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { EquipmentSummaryAnalyticsModule } from './../summary-analytics/equipment-summary-analytics/equipment-summary-analytics.module';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { RoomsRoutingModule } from './rooms-routing.module';


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
    IncludeEquipmentLabelsModule,
    EntitySharedModule,
    EquipmentSummaryAnalyticsModule,
  ],
  exports: [
  ],
})
export class RoomsModule { }
