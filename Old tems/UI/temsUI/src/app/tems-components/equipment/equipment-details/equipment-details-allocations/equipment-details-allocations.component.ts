import { Component, Input, OnInit } from '@angular/core';
import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';

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
