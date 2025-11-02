import { ClaimService } from './../../../services/claim.service';
import { Component, Input, OnInit } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';

@Component({
  selector: 'app-room-details-issues',
  templateUrl: './room-details-issues.component.html',
  styleUrls: ['./room-details-issues.component.scss']
})
export class RoomDetailsIssuesComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  roomAlreadySelected: IOption;

  constructor() { }

  ngOnInit(): void {
    this.roomAlreadySelected = {
      value: this.room.id,
      label: this.room.identifier,
    }
  }
}
