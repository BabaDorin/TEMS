import { QRCodeModule } from 'angularx-qrcode';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { EquipmentLabelComponent } from './../../tems-components/equipment/equipment-label/equipment-label.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@NgModule({
  declarations: [
    EquipmentLabelComponent
  ],
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    QRCodeModule
  ],
  exports: [
    EquipmentLabelComponent
  ]
})
export class EquipmentLabelModule { }
