import { Component, Input, OnInit } from '@angular/core';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';

@Component({
  selector: 'app-room-details-allocations',
  templateUrl: './room-details-allocations.component.html',
  styleUrls: ['./room-details-allocations.component.scss']
})
export class RoomDetailsAllocationsComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  roomsParameter;
  constructor() { }

  ngOnInit(): void {
    this.roomsParameter = [this.room.id];
  }
}
