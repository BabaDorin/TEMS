import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { Component, Input, OnInit } from '@angular/core';
import { IOption } from 'src/app/models/option.model';

@Component({
  selector: 'app-room-details-logs',
  templateUrl: './room-details-logs.component.html',
  styleUrls: ['./room-details-logs.component.scss']
})
export class RoomDetailsLogsComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  roomOption: IOption;
  
  constructor(
    
  ) { }

  ngOnInit(): void {
    if(this.room != undefined)
      this.roomOption = {
        value: this.room.id,
        label: this.room.identifier
      }
  }
}
