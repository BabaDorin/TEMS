import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';
import { ViewEquipmentAllocation } from './../../../../models/equipment/view-equipment-allocation.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-equipment-details-allocations',
  templateUrl: './equipment-details-allocations.component.html',
  styleUrls: ['./equipment-details-allocations.component.scss']
})
export class EquipmentDetailsAllocationsComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified; 
  constructor() {
  }

  ngOnInit(): void {
  }

}
