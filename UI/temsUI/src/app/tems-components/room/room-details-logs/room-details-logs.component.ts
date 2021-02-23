import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-room-details-logs',
  templateUrl: './room-details-logs.component.html',
  styleUrls: ['./room-details-logs.component.scss']
})
export class RoomDetailsLogsComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  constructor() { }

  ngOnInit(): void {
  }

}
