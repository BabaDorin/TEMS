import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { Observable } from 'rxjs';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-ag-grid-rooms',
  templateUrl: './ag-grid-rooms.component.html',
  styleUrls: ['./ag-grid-rooms.component.scss']
})
export class AgGridRoomsComponent implements OnInit {

  rooms: ViewRoomSimplified[];

  columnDefs = [
    { field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
    { field: 'label', sortable: true, filter: true },
    { field: 'description', sortable: true, filter: true },
    { field: 'openedIssues', sortable: true, filter: true },
    { field: 'allocatedEquipment', sortable: true, filter: true },
  ];

  rowData: any;

  constructor(
    private roomService: RoomsService
  ) { }

  ngOnInit(): void {
    this.rowData = this.roomService.getRooms();
    console.log(this.rowData);
  }

}
