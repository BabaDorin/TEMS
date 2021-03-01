import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-ag-grid-personnel',
  templateUrl: './ag-grid-personnel.component.html',
  styleUrls: ['./ag-grid-personnel.component.scss']
})
export class AgGridPersonnelComponent implements OnInit {

  personnel: ViewPersonnelSimplified[];

  columnDefs = [
    { field: 'name', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
    { field: 'pozition', sortable: true, filter: true },
    { field: 'allocatedEquipment', sortable: true, filter: true },
    { field: 'issues', sortable: true, filter: true },
  ];

  rowData: any;

  constructor(
    private personnelService: PersonnelService
  ) { }

  ngOnInit(): void {
    this.rowData = this.personnelService.getPersonnel();
    console.log(this.rowData);
  }

}
