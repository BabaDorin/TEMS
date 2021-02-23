import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { PropertyRenderComponent } from './../public/property-render/property-render.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTabsModule } from '@angular/material/tabs';
import { FormlyModule } from '@ngx-formly/core';
import { ViewRoomsComponent } from './../tems-components/room/view-rooms/view-rooms.component';
import { AgGridModule } from 'ag-grid-angular';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RoomsRoutingModule } from './rooms-routing.module';
import { SummaryRoomsAnalyticsComponent } from '../tems-components/analytics/summary-rooms-analytics/summary-rooms-analytics.component';
import { AgGridRoomsComponent } from '../tems-components/room/ag-grid-rooms/ag-grid-rooms.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule } from '@angular/material/dialog';
import { MatOptionModule } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { EquipmentService } from '../services/equipment-service/equipment.service';
import { FormlyParserService } from '../services/formly-parser-service/formly-parser.service';
import { AddRoomComponent } from './add-room/add-room.component';
import { RoomDetailsGeneralComponent } from '../tems-components/room/room-details-general/room-details-general.component';
import { RoomDetailsComponent } from '../tems-components/room/room-details/room-details.component';
import { ImageCarouselComponent } from '../public/image-carousel/image-carousel.component';
import { EntityLogsListComponent } from '../tems-components/entity-logs-list/entity-logs-list.component';
import { RoomDetailsLogsComponent } from '../tems-components/room/room-details-logs/room-details-logs.component';
import { AddLogComponent } from '../tems-components/communication/add-log/add-log.component';
import { EntitySharedModuleModule } from '../tems-components/entity-shared-module/entity-shared-module.module';


@NgModule({
  declarations: [
    SummaryRoomsAnalyticsComponent,
    AgGridRoomsComponent,
    ViewRoomsComponent,
    AddRoomComponent,
    RoomDetailsComponent,
    RoomDetailsGeneralComponent,
    EntityLogsListComponent,
    RoomDetailsLogsComponent,
  ],
  imports: [
    CommonModule,
    RoomsRoutingModule,
    AgGridModule,

    FormsModule,
    MatInputModule,
    FormsModule,
    FormlyModule,
    ReactiveFormsModule,
    MatInputModule,
    MatTabsModule,
    MatExpansionModule,
    MatSelectModule,
    MatOptionModule,
    EntitySharedModuleModule,
  ],
  exports: [
    MatFormFieldModule, 
    MatButtonModule,
    MatTooltipModule,
    MatAutocompleteModule,
    MatChipsModule,
    MatDialogModule,
    MatOptionModule,
    MatCardModule,
    MatRadioModule,
    EquipmentService,
    FormlyParserService,
  ],
})
export class RoomsModule { }
