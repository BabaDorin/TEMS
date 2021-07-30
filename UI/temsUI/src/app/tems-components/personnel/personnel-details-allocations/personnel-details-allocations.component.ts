import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { EquipmentFilter } from 'src/app/helpers/filters/equipment.filter';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { AgGridEquipmentComponent } from './../../equipment/ag-grid-equipment/ag-grid-equipment.component';

@Component({
  selector: 'app-personnel-details-allocations',
  templateUrl: './personnel-details-allocations.component.html',
  styleUrls: ['./personnel-details-allocations.component.scss']
})
export class PersonnelDetailsAllocationsComponent implements OnInit {
  
  @Input() personnel: ViewPersonnelSimplified;
  equipmentFilter: EquipmentFilter;

  constructor() { 
    this.equipmentFilter = new EquipmentFilter();
  }

  ngOnInit(): void {
    if(this.personnel == undefined)
      return;
    
    this.equipmentFilter.personnel = [this.personnel.id];
    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
  }
}
