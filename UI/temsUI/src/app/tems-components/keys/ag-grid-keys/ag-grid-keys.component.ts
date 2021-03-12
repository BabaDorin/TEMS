import { ViewKeySimplified } from 'src/app/models/key/view-key.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { KeysService } from './../../../services/keys-service/keys.service';
import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';

@Component({
  selector: 'app-ag-grid-keys',
  templateUrl: './ag-grid-keys.component.html',
  styleUrls: ['./ag-grid-keys.component.scss']
})
export class AgGridKeysComponent extends TEMSComponent implements OnInit, OnChanges {

  @Input() keys;
  private gridApi;
  private gridColumnApi;

  private columnDefs;
  private defaultColDef;
  private rowSelection;
  private rowData: [];

  constructor(
    private keysService: KeysService
  ) { 
    super();

    this.columnDefs = [
      { headerName: 'Indentifier',  field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
      { headerName: 'Room', field: 'room.label', sortable: true, filter: true },
      { headerName: 'Allocated to', field: 'allocatedTo.label', sortable: true, filter: true },
      { headerName: 'Time', field: 'timePassed', sortable: true, filter: true },
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
  ngOnChanges(): void {
    console.log(this.keys);

    if(this.keys != undefined)
      this.rowData = this.keys;
  }

  ngOnInit(): void {
    console.log(this.keys);
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    if(this.keys == undefined)
      this.subscriptions.push(this.keysService.getKeys()
        .subscribe(result => {
          console.log(result);
          this.rowData = result;
        }));
    else
      this.rowData = this.keys;
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
