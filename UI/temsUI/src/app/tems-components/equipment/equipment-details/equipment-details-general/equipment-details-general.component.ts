import { Property } from './../../../../models/equipment/view-property.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit } from '@angular/core';
import { ViewEquipment } from 'src/app/models/equipment/view-equipment.model';

@Component({
  selector: 'app-equipment-details-general',
  templateUrl: './equipment-details-general.component.html',
  styleUrls: ['./equipment-details-general.component.scss']
})
export class EquipmentDetailsGeneralComponent implements OnInit {

  @Input() equipmentId: string;

  equipment: ViewEquipment;
  generalProperties: Property[];
  specificProperties: Property[];
  edit = false;

  constructor(private equipmentService: EquipmentService) {
    this.equipment = this.equipmentService.getEquipmentByID(this.equipmentId);

    this.generalProperties= [
      { displayName: 'Identifier', value: this.equipment.identifier},
      {displayName: 'Type', value: this.equipment.type.name},
      {displayName: 'TemsID', value: this.equipment.temsID },
      {displayName: 'Serial Number', value: this.equipment.serialNumber},
      { displayName: 'Is Used', dataType:{id: '0', name: 'boolean'}, value: this.equipment.isUsed},
      { displayName: 'Is Defect', dataType:{id: '0', name: 'boolean'}, value: this.equipment.isUsed},
    ];

    this.specificProperties = this.equipment.specificTypeProperties;
  }

  ngOnInit(): void {
    
  }

}
