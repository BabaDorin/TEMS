import { IOption } from 'src/app/models/option.model';
import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';

@Component({
  selector: 'app-equipment-details-logs',
  templateUrl: './equipment-details-logs.component.html',
  styleUrls: ['./equipment-details-logs.component.scss']
})
export class EquipmentDetailsLogsComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  equipmentOption: IOption;

  constructor() { 
  }

  ngOnInit(): void {
    if(this.equipment != undefined)
      this.equipmentOption = { 
        value: this.equipment.id,
        label: this.equipment.temsIdOrSerialNumber,
      };
  }
}
