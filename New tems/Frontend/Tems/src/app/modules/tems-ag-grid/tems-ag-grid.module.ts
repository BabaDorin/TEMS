import { DefectCellRenderedComponent } from './../../public/ag-grid/defect-cell-rendered/defect-cell-rendered.component';
import { UsedCellRenderedComponent } from './../../public/ag-grid/used-cell-rendered/used-cell-rendered.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AgGridModule } from 'ag-grid-angular';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { BooleanCellRendererComponent } from './../../public/ag-grid/boolean-cell-renderer/boolean-cell-renderer.component';
import { AgGridEquipmentComponent } from './../../tems-components/equipment/ag-grid-equipment/ag-grid-equipment.component';
// import { AgGridKeysComponent } from './../../tems-components/keys/ag-grid-keys/ag-grid-keys.component';
import { AgGridPersonnelComponent } from './../../tems-components/personnel/ag-grid-personnel/ag-grid-personnel.component';
import { AgGridRoomsComponent } from './../../tems-components/room/ag-grid-rooms/ag-grid-rooms.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
    AgGridModule,
    AgGridEquipmentComponent,
    AgGridPersonnelComponent,
    AgGridRoomsComponent,
    BtnCellRendererComponent,
    BooleanCellRendererComponent,
    UsedCellRenderedComponent,
    DefectCellRenderedComponent
  ],
  exports: [
    AgGridEquipmentComponent,
    AgGridPersonnelComponent,
    AgGridRoomsComponent,
    BtnCellRendererComponent,
    BooleanCellRendererComponent,
    UsedCellRenderedComponent,
    DefectCellRenderedComponent
  ]
})
export class TemsAgGridModule { }
