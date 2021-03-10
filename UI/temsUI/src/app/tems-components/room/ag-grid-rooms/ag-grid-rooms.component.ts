import { TEMSComponent } from './../../../tems/tems.component';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { Observable } from 'rxjs';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-ag-grid-rooms',
  templateUrl: './ag-grid-rooms.component.html',
  styleUrls: ['./ag-grid-rooms.component.scss']
})
export class AgGridRoomsComponent extends TEMSComponent implements OnInit {

  private gridApi;
  private gridColumnApi;

  private columnDefs;
  private defaultColDef;
  private rowSelection;
  private rowData: [];

  rooms: ViewRoomSimplified[];

  constructor(
    private roomService: RoomsService
  ) { 
    super();

    this.columnDefs = [
      { field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
      { field: 'label', sortable: true, filter: true },
      { field: 'description', sortable: true, filter: true },
      { field: 'openedIssues', sortable: true, filter: true },
      { field: 'allocatedEquipment', sortable: true, filter: true },
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

    // this.subscriptions.push(this.roomService.getRoomsSimplified(20, 20)
    //   .subscribe(result => {
    //     console.log(result);
    //     this.rowData = result;
    //   }));
  }

  isFirstColumn(params) {
    var displayedColumns = params.columnApi.getAllDisplayedColumns();
    var thisIsFirstColumn = displayedColumns[0] === params.column;
    return thisIsFirstColumn;
  }


  

}
