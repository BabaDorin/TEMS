import { TEMSComponent } from './../../../tems/tems.component';
import { KeysService } from './../../../services/keys-service/keys.service';
import { Component, OnInit, OnChanges } from '@angular/core';
import { ViewKeySimplified } from 'src/app/models/key/view-key.model';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { RoomsService } from 'src/app/services/rooms-service/rooms.service';

@Component({
  selector: 'app-ag-grid-keys',
  templateUrl: './ag-grid-keys.component.html',
  styleUrls: ['./ag-grid-keys.component.scss']
})
export class AgGridKeysComponent extends TEMSComponent implements OnInit {

  private gridApi;
  private gridColumnApi;

  private columnDefs;
  private defaultColDef;
  private rowSelection;
  private rowData: [];

  keys: ViewRoomSimplified[];

  constructor(
    private keysService: KeysService
  ) { 
    super();

    this.columnDefs = [
      { field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
      { field: 'numberOfCopies', sortable: true, filter: true },
      { field: 'allocatedTo', sortable: true, filter: true },
      { field: 'TimePassed', sortable: true, filter: true },
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

    this.subscriptions.push(this.keysService.getKeys()
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
