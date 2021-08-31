import { ClaimService } from './../../../services/claim.service';
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
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
export class AgGridRoomsComponent extends TEMSComponent {

  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
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
    private translate: TranslateService,
    private claims: ClaimService
  ) {
    super();

    this.pagination = true;
    this.paginationPageSize = 20;

    this.frameworkComponents = {
      btnCellRendererComponent: BtnCellRendererComponent
    };

    this.defaultColDef = {
      flex: 1,
      resizable: true,
      filter: true,
      sortable: true,
      minWidth: 80,
      width: 150
    }

    this.columnDefs = [
      { 
        headerName: this.translate.instant('form.identifier'), 
        field: 'identifier', 
        checkboxSelection: true, 
        headerCheckboxSelection: true,
        lockPosition: true
      },
      { 
        headerName: this.translate.instant('room.labels'), 
        field: 'label'
      },
      { 
        headerName: this.translate.instant('form.description'), 
        field: 'description', 
      },
      { 
        headerName: this.translate.instant('entities.activeTickets'), 
        field: 'activeTickets', 
      },
      { 
        headerName: this.translate.instant('entities.allocatedEquipment'), 
        field: 'allocatedEquipments'
      },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.details.bind(this),
          matIcon: 'more_horiz'
        },
        width: 100,
        suppressSizeToFit: true,
      },
    ];

    if(this.claims.canManage){
      this.columnDefs.push(      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          matIcon: 'delete',
          matIconClass: 'text-muted'
        },
        width: 100,
        suppressSizeToFit: true,
      });
    }
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
        this.loading = false;

        if(this.rowData != result)
          this.rowData = result;
        
        this.sizeToFit();
      }));
  }

  getSelectedNodes() {
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }

  sizeToFit() {
    this.gridApi.sizeColumnsToFit();
  }
}
