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
  constructor(private equipmentService: EquipmentService) { 
    this.equipment = equipmentService.getEquipmentByID(this.equipmentId);
    console.log(this.equipment);
  }

  ngOnInit(): void {
  }

}
