import { RoomDetailsGeneralComponent } from './../room-details-general/room-details-general.component';
import { SnackService } from './../../../services/snack/snack.service';
import { DialogService } from './../../../services/dialog-service/dialog.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { Observable } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';

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
  private frameworkComponents: any;
  private pagination
  private paginationPageSize;
  loading: boolean = true;

  rooms: ViewRoomSimplified[];

  constructor(
    private roomService: RoomsService,
    private dialogService: DialogService,
    private snackService: SnackService,
  ) { 
    super();

     // enables pagination in the grid
     this.pagination = true;

     // sets 10 rows per page (default is 100)
     this.paginationPageSize = 20;
 
     this.frameworkComponents = {
       btnCellRendererComponent: BtnCellRendererComponent
     }

    this.columnDefs = [
      { field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
      { field: 'label', sortable: true, filter: true },
      { field: 'description', sortable: true, filter: true },
      { field: 'activeTickets', sortable: true, filter: true },
      { field: 'allocatedEquipments', sortable: true, filter: true },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.details.bind(this),
          label: 'Details'
        }
      },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          label: 'Archieve'
        }
      }
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

  details(e) {
    this.dialogService.openDialog(
      RoomDetailsGeneralComponent,
      [
        { label: "displayViewMore", value: true },
        { label: "roomId", value: e.rowData.id },
      ]
    )
  }

  archieve(e){
    if(!confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
    return;

    this.subscriptions.push(
      this.roomService.archieveRoom(e.rowData.id) // HERE
      .subscribe(result => {
        if(this.snackService.snack(result)){
          this.gridApi.applyTransaction({ remove: [e.rowData] });
        }
      })
    )
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.fetchRooms();
  }

  fetchRooms(){
    this.loading = true;
    this.subscriptions.push(this.roomService.getRoomsSimplified(20, 20)
      .subscribe(result => {
        console.log(result);
        this.rowData = result;
        this.loading = false;
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
