import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-room-details-issues',
  templateUrl: './room-details-issues.component.html',
  styleUrls: ['./room-details-issues.component.scss']
})
export class RoomDetailsIssuesComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  constructor() { }

  ngOnInit(): void {
  }
}
