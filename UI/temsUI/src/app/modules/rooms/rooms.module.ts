import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { FormlyModule } from '@ngx-formly/core';
import { ViewRoomsComponent } from '../../tems-components/room/view-rooms/view-rooms.component';
import { AgGridModule } from 'ag-grid-angular';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RoomsRoutingModule } from './rooms-routing.module';
import { SummaryRoomsAnalyticsComponent } from '../../tems-components/analytics/summary-rooms-analytics/summary-rooms-analytics.component';
import { AgGridRoomsComponent } from '../../tems-components/room/ag-grid-rooms/ag-grid-rooms.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AddRoomComponent } from '../../tems-components/room/add-room/add-room.component';
import { RoomDetailsGeneralComponent } from '../../tems-components/room/room-details-general/room-details-general.component';
import { RoomDetailsComponent } from '../../tems-components/room/room-details/room-details.component';
import { RoomDetailsLogsComponent } from '../../tems-components/room/room-details-logs/room-details-logs.component';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { MaterialModule } from '../material/material.module';
import { RoomDetailsIssuesComponent } from '../../tems-components/room/room-details-issues/room-details-issues.component';
import { RoomDetailsAllocationsComponent } from '../../tems-components/room/room-details-allocations/room-details-allocations.component';

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
    MaterialModule,
    TemsFormsModule,
    ReactiveFormsModule,

    // Shared modules
    EntitySharedModule,
  ],
  exports: [
  ],
})
export class RoomsModule { }
