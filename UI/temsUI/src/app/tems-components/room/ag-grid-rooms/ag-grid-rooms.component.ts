import { Component, OnInit } from '@angular/core';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { DialogService } from '../../../services/dialog.service';
import { RoomsService } from '../../../services/rooms.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { RoomDetailsGeneralComponent } from './../room-details-general/room-details-general.component';

@Component({
  selector: 'app-ag-grid-rooms',
  templateUrl: './ag-grid-rooms.component.html',
  styleUrls: ['./ag-grid-rooms.component.scss']
})
export class AgGridRoomsComponent extends TEMSComponent implements OnInit {

  private gridApi;
  private gridColumnApi;

  columnDefs;
  defaultColDef;
  rowSelection;
  rowData: [];
  frameworkComponents: any;
  pagination
  paginationPageSize;
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
    };

    this.defaultColDef = {
      resizable: true
    }


    this.columnDefs = [
      { field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true, resizeable: true },
      { field: 'label', sortable: true, filter: true, resizeable: true },
      { field: 'description', sortable: true, filter: true, resizeable: true },
      { field: 'activeTickets', sortable: true, filter: true, resizeable: true },
      { field: 'allocatedEquipments', sortable: true, filter: true, resizeable: true },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.details.bind(this),
          matIcon: 'more_horiz'
        }
      },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          matIcon: 'delete',
          matIconClass: 'text-muted'
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

  archieve(e) {
    if (!confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
      return;

    this.subscriptions.push(
      this.roomService.archieveRoom(e.rowData.id) // HERE
        .subscribe(result => {
          if (this.snackService.snack(result)) {
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

  fetchRooms() {
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

  getSelectedNodes() {
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }
}
