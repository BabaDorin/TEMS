import { SnackService } from './../../../services/snack/snack.service';
import { RoomsService } from 'src/app/services/rooms-service/rooms.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { DialogService } from './../../../services/dialog-service/dialog.service';
import { ViewRoom } from './../../../models/room/view-room.model';
import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { Property } from 'src/app/models/equipment/view-property.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-room-details-general',
  templateUrl: './room-details-general.component.html',
  styleUrls: ['./room-details-general.component.scss']
})
export class RoomDetailsGeneralComponent extends TEMSComponent implements OnInit, OnChanges{

  @Input() roomId: string;
  @Input() room: ViewRoom;

  roomProperties: Property[];
  edit: boolean = false;
  displayViewMore = false;
  dialogRef;

  constructor(
    private roomService: RoomsService,
    private route: Router,
    private dialogService: DialogService,
    private snackService: SnackService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.roomService.getRoomById(this.roomId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.room = result;

        this.roomProperties = [
          { displayName: 'Identifier', value: this.room.identifier },
          { displayName: 'Description', value: this.room.description },
          { displayName: 'Floor', value: this.room.floor },
          { displayName: 'Active issues', value: this.room.activeTickets },
          { displayName: 'Labels', value: "display them in a fancy way" },
        ]
      })
    )
  }

  ngOnChanges(){
  }

  viewMore(){
    console.log(this.roomId);
    this.route.navigateByUrl('/rooms/details/' + this.roomId);
    
    if(this.dialogRef != undefined)
      this.dialogRef.close();
  }
}
