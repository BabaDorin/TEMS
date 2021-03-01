import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-room-details',
  templateUrl: './room-details.component.html',
  styleUrls: ['./room-details.component.scss']
})
export class RoomDetailsComponent implements OnInit {

  @Input() roomId;
  edit: boolean = false;
  roomSimplified: ViewRoomSimplified;

  constructor(
    private activatedroute: ActivatedRoute,
    private roomService: RoomsService
  ) { 

  }

  ngOnInit(): void {
    if(this.roomId == undefined)
      this.roomId = this.activatedroute.snapshot.paramMap.get("id");
    this.edit=false;

    this.roomSimplified = this.roomService.getRoomSimplified(this.roomId);
  }

}
