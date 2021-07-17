import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { RoomsService } from '../../../services/rooms.service';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { ViewRoom } from './../../../models/room/view-room.model';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-room-details',
  templateUrl: './room-details.component.html',
  styleUrls: ['./room-details.component.scss']
})
export class RoomDetailsComponent extends TEMSComponent implements OnInit {

  @Input() roomId;
  edit: boolean = false;
  roomSimplified = new ViewRoomSimplified();
  room: ViewRoom;
  mainHeaderLabel="General";
  
  constructor(
    private activatedroute: ActivatedRoute,
    private roomService: RoomsService,
    public translate: TranslateService
  ) { 
    super();
  }

  ngOnInit(): void {
    if(this.roomId == undefined)
      this.roomId = this.activatedroute.snapshot.paramMap.get("id");
      
    this.subscriptions.push(this.roomService.getRoomById(this.roomId)
      .subscribe(result => {
        this.room = result;
        this.roomSimplified = this.roomService.getRoomSimplifiedFromRoom(this.room);
        
        if(this.roomSimplified.isArchieved)
          this.mainHeaderLabel += " (Archieved)"
      }));
  }

  archivationStatusChanged(){
    this.mainHeaderLabel = "General"

    this.roomSimplified.isArchieved = !this.roomSimplified.isArchieved;
    if(this.roomSimplified.isArchieved)
          this.mainHeaderLabel += " (Archieved)"
  }
}
