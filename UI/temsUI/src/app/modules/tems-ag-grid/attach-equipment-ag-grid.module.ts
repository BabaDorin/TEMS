import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { AgGridModule } from 'ag-grid-angular';
import { AgGridAttachEquipmentComponent } from './../../tems-components/equipment/ag-grid-attach-equipment/ag-grid-attach-equipment.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    AgGridAttachEquipmentComponent
  ],
  imports: [
    CommonModule,
    AgGridModule,
    MatButtonModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatCheckboxModule
  ],
  exports: [
    AgGridAttachEquipmentComponent
  ]
})
export class AttachEquipmentAgGridModule { }