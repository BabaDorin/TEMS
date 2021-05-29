import { CAN_MANAGE_ENTITIES } from './../../../models/claims';
import { TokenService } from './../../../services/token-service/token.service';
import { AddRoomComponent } from './../add-room/add-room.component';
import { SnackService } from './../../../services/snack/snack.service';
import { RoomsService } from 'src/app/services/rooms-service/rooms.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { DialogService } from './../../../services/dialog-service/dialog.service';
import { ViewRoom } from './../../../models/room/view-room.model';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { Property } from 'src/app/models/equipment/view-property.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-room-details-general',
  templateUrl: './room-details-general.component.html',
  styleUrls: ['./room-details-general.component.scss']
})
export class RoomDetailsGeneralComponent extends TEMSComponent implements OnInit{

  @Input() roomId: string;
  @Input() room: ViewRoom;
  @Output() archivationStatusChanged = new EventEmitter();

  roomProperties: Property[];
  displayViewMore = false;
  dialogRef;
  headerClass;
  canManage: boolean = false;

  constructor(
    private roomService: RoomsService,
    private route: Router,
    private dialogService: DialogService,
    private snackService: SnackService,
    private tokenService: TokenService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.fetchRoom();
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);
  }

  fetchRoom(){
    this.subscriptions.push(
      this.roomService.getRoomById(this.roomId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.room = result;
        this.headerClass = (this.room.isArchieved) ? 'text-muted' : '';

        this.roomProperties = [
          { displayName: 'Identifier', value: this.room.identifier },
          { displayName: 'Description', value: this.room.description },
          { displayName: 'Floor', value: this.room.floor },
          { displayName: 'Active issues', value: this.room.activeTickets },
        ]
      })
    )
  }
  viewMore(){
    if(this.dialogRef != undefined)
      this.dialogRef.close();
  }

  archieve(){
    if(!this.room.isArchieved && !confirm("Are you sure you want to archieve this room? Allocations and logs associated with this room will get archieved as well."))
      return;
      
      let newArchivationStatus = !this.room.isArchieved;
      this.subscriptions.push(
        this.roomService.archieveRoom(this.roomId, newArchivationStatus)
        .subscribe(result => {
          this.snackService.snack(result);

          if(result.status == 1)
            this.room.isArchieved = newArchivationStatus;
            this.headerClass = (this.room.isArchieved) ? 'text-muted' : '';
  
          this.archivationStatusChanged.emit(this.room.isArchieved);
        })
      )
  }

  edit(){
    this.dialogService.openDialog(
      AddRoomComponent,
      [{label: "roomId", value: this.roomId}],
      () => {
         this.fetchRoom();
      }
    )
  }
}
