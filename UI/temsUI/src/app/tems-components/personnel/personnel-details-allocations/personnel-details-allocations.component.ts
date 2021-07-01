import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { AgGridEquipmentComponent } from './../../equipment/ag-grid-equipment/ag-grid-equipment.component';

@Component({
  selector: 'app-personnel-details-allocations',
  templateUrl: './personnel-details-allocations.component.html',
  styleUrls: ['./personnel-details-allocations.component.scss']
})
export class PersonnelDetailsAllocationsComponent implements OnInit {

  @Input() personnel: ViewPersonnelSimplified;
  @ViewChild('agGridEquipment') adGridEquipment: AgGridEquipmentComponent;
  personnelParameter;
  constructor() { }

  ngOnInit(): void {
    this.personnelParameter = [this.personnel.id];
  }

  refreshAgGrid(){
    this.adGridEquipment.fetchEquipments();
  }
}
