import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-room-details-allocations',
  templateUrl: './room-details-allocations.component.html',
  styleUrls: ['./room-details-allocations.component.scss']
})
export class RoomDetailsAllocationsComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  constructor() { }

  ngOnInit(): void {
  }
}
