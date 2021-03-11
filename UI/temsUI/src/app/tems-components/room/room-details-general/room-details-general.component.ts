import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { ViewRoom } from './../../../models/room/view-room.model';
import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { Property } from 'src/app/models/equipment/view-property.model';

@Component({
  selector: 'app-room-details-general',
  templateUrl: './room-details-general.component.html',
  styleUrls: ['./room-details-general.component.scss']
})
export class RoomDetailsGeneralComponent implements OnInit, OnChanges{

  @Input() room: ViewRoom;

  roomProperties: Property[];
  edit: boolean = false;

  constructor() { }

  ngOnInit(): void {
    
  }

  ngOnChanges(){
    if(this.room != undefined)
    this.roomProperties = [
      { displayName: 'Identifier', value: this.room.identifier },
      { displayName: 'Description', value: this.room.description },
      { displayName: 'Floor', value: this.room.floor },
      { displayName: 'Active issues', value: this.room.activeTickets },
      { displayName: 'Labels', value: "display them in a fancy way" },
    ]
  }
}
