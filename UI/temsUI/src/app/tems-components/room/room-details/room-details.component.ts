import { TEMSComponent } from './../../../tems/tems.component';
import { ViewRoom } from './../../../models/room/view-room.model';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-room-details',
  templateUrl: './room-details.component.html',
  styleUrls: ['./room-details.component.scss']
})
export class RoomDetailsComponent extends TEMSComponent implements OnInit {

  @Input() roomId;
  edit: boolean = false;
  roomSimplified: ViewRoomSimplified;
  room: ViewRoom;

  constructor(
    private activatedroute: ActivatedRoute,
    private roomService: RoomsService
  ) { 
    super();
  }

  ngOnInit(): void {
    if(this.roomId == undefined)
      this.roomId = this.activatedroute.snapshot.paramMap.get("id");
      
    this.subscriptions.push(this.roomService.getRoomById(this.roomId)
      .subscribe(result => {
        console.log(result)
        this.room = result;
        this.roomSimplified = this.roomService.getRoomSimplifiedFromRoom(this.room);
      }));
  }
}
