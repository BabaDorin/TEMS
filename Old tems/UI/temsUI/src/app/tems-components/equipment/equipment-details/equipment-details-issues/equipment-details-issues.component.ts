import { Component, Input, OnInit } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';

@Component({
  selector: 'app-equipment-details-issues',
  templateUrl: './equipment-details-issues.component.html',
  styleUrls: ['./equipment-details-issues.component.scss']
})
export class EquipmentDetailsIssuesComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  equipmentAlreadySelected : IOption;
  constructor() { }

  ngOnInit(): void {
    this.equipmentAlreadySelected = {
      value: this.equipment.id,
      label: this.equipment.temsIdOrSerialNumber,
    }
  }
}
