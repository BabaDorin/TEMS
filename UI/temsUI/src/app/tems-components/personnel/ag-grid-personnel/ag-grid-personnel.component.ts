import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-ag-grid-personnel',
  templateUrl: './ag-grid-personnel.component.html',
  styleUrls: ['./ag-grid-personnel.component.scss']
})
export class AgGridPersonnelComponent extends TEMSComponent implements OnInit {

  private gridApi;
  private gridColumnApi;

  private columnDefs;
  private defaultColDef;
  private rowSelection;
  private rowData: [];

  personnel: ViewPersonnelSimplified[];

  constructor(
    private personnelService: PersonnelService
  ) { 
    super();

    this.columnDefs = [
      { field: 'name', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
      { field: 'positions', sortable: true, filter: true },
      { field: 'allocatedEquipments', sortable: true, filter: true },
      { field: 'activeTickets', sortable: true, filter: true },
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

  ngOnInit(): void {
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.subscriptions.push(this.personnelService.getPersonnelSimplified(20, 20)
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
}
