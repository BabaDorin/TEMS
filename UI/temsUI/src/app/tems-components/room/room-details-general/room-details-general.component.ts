import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { ViewRoom } from './../../../models/room/view-room.model';
import { Component, Input, OnInit } from '@angular/core';
import { Property } from 'src/app/models/equipment/view-property.model';

@Component({
  selector: 'app-room-details-general',
  templateUrl: './room-details-general.component.html',
  styleUrls: ['./room-details-general.component.scss']
})
export class RoomDetailsGeneralComponent implements OnInit {

  @Input() roomId;

  room: ViewRoom;
  roomProperties: Property[];
  edit: boolean = false;

  constructor(
    private roomService: RoomsService
  ) { }

  ngOnInit(): void {
    this.room = this.roomService.getRoomById(this.roomId);

    this.roomProperties = [
      { displayName: 'Identifier', value: this.room.identifier },
      { displayName: 'Description', value: this.room.description },
      { displayName: 'Floor', value: this.room.floor },
      { displayName: 'Issue State', value: this.room.issueState },
    ]
  }
}
