import { Component, Input, OnInit } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';

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
