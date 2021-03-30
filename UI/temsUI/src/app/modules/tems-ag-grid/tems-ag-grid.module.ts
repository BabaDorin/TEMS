import { MaterialModule } from './../material/material.module';
import { AgGridRoomsComponent } from './../../tems-components/room/ag-grid-rooms/ag-grid-rooms.component';
import { AgGridTooltipComponent } from './../../public/ag-grid/ag-grid-tooltip/ag-grid-tooltip.component';
import { AgGridPersonnelComponent } from './../../tems-components/personnel/ag-grid-personnel/ag-grid-personnel.component';
import { AgGridKeysComponent } from './../../tems-components/keys/ag-grid-keys/ag-grid-keys.component';
import { AgGridEquipmentComponent } from './../../tems-components/equipment/ag-grid-equipment/ag-grid-equipment.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';



@NgModule({
  declarations: [
    AgGridEquipmentComponent,
    AgGridKeysComponent,
    AgGridPersonnelComponent,
    AgGridTooltipComponent,
    AgGridRoomsComponent,
    BtnCellRendererComponent
  ],
  imports: [
    CommonModule,
    AgGridModule.withComponents([BtnCellRendererComponent]),
    CommonModule,
    MaterialModule,
  ],
  exports: [
    AgGridEquipmentComponent,
    AgGridKeysComponent,
    AgGridPersonnelComponent,
    AgGridTooltipComponent,
    AgGridRoomsComponent,
    BtnCellRendererComponent,
    
  ]
})
export class TemsAgGridModule { }
