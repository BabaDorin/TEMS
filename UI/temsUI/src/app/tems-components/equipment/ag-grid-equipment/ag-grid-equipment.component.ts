import { TEMSComponent } from './../../../tems/tems.component';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-ag-grid-equipment',
  templateUrl: './ag-grid-equipment.component.html',
  styleUrls: ['./ag-grid-equipment.component.scss']
})

export class AgGridEquipmentComponent extends TEMSComponent implements OnInit {

  columnDefs = [
    { field: 'temsID', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
    { field: 'serialNumber', sortable: true, filter: true },
    { field: 'definition', sortable: true, filter: true },
    { field: 'room', sortable: true, filter: true },
    { field: 'type', sortable: true, filter: true },
    { field: 'isUsed', sortable: false, filter: true },
    { field: 'isDefect', sortable: false, filter: true },
  ];

  rowData: any;

  constructor(
    private http: HttpClient,
    private equipmentService: EquipmentService) {
      super();
  }

  ngOnInit() {
    this.subscriptions.push(this.equipmentService.getEquipmentSimplified()
      .subscribe(result => {
        console.log(result);
        this.rowData = result;
      })); 
  }
}
