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

  private gridApi;
  private gridColumnApi;

  private columnDefs;
  private defaultColDef;
  private rowSelection;
  private rowData: [];

  constructor(
    private equipmentService: EquipmentService) {
    super();

    this.columnDefs = [
      { field: 'temsId', sortable: true, filter: true },
      { field: 'serialNumber', sortable: true, filter: true },
      { field: 'definition', sortable: true, filter: true },
      { field: 'assignee', sortable: true, filter: true },
      { field: 'type', sortable: true, filter: true },
      { field: 'isUsed', sortable: false, filter: true },
      { field: 'isDefect', sortable: false, filter: true },
    ];

    this.defaultColDef = {
      flex: 1,
      minWidth: 100,
      resizable: true,
      headerCheckboxSelection: this.isFirstColumn,
      checkboxSelection: this.isFirstColumn,
    };

    this.rowSelection = 'multiple';
  }

  ngOnInit() {

  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.subscriptions.push(this.equipmentService.getEquipmentSimplified(20, 20, true)
      .subscribe(result => {
        console.log(result);
        this.rowData = result;
      }));
  }

  isFirstColumn(params) {
    var displayedColumns = params.columnApi.getAllDisplayedColumns();
    var thisIsFirstColumn = displayedColumns[0] === params.column;
    return thisIsFirstColumn;
  }

  getSelectedNodes(){
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }
}
