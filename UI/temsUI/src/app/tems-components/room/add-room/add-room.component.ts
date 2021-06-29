import { TranslateService } from '@ngx-translate/core';
import { PersonnelService } from '../../../services/personnel.service';
import { RoomLabelService } from './../../../services/room-label.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { ChipsAutocompleteComponent } from './../../../public/formly/chips-autocomplete/chips-autocomplete.component';
import { IOption } from '../../../models/option.model';
import { RoomsService } from '../../../services/rooms.service';
import { FormlyParserService } from '../../../services/formly-parser.service';
import { Component, Inject, OnInit, Optional, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddRoom } from 'src/app/models/room/add-room.model';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormlyData } from 'src/app/models/formly/formly-data.model';

@Component({
  selector: 'app-add-room',
  templateUrl: './add-room.component.html',
  styleUrls: ['./add-room.component.scss']
})

export class AddRoomComponent extends TEMSComponent implements OnInit {

  roomId: string;
  @ViewChild('labels') labels: ChipsAutocompleteComponent;
  @ViewChild('supervisories') supervisories: ChipsAutocompleteComponent;

  public formlyData = new FormlyData();

  roomLabels: IOption[];
  dialogRef;

  constructor(
    private formlyParserService: FormlyParserService,
    public roomService: RoomsService,
    private snackService: SnackService,
    public roomLabelService: RoomLabelService,
    public personnelService: PersonnelService,
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(dialogData != undefined){
      this.roomId = dialogData.roomId;
    }
  }
  
  ngOnInit(): void {
    this.formlyData.model ={};
    this.formlyData.fields = this.formlyParserService.parseAddRoom();

    if(this.roomId == undefined)
      return;
    
    this.subscriptions.push(
      this.roomService.getRoomToUpdate(this.roomId)
      .subscribe(result => {
        let roomToUpdate: AddRoom = result;
        console.log('room to update');
        console.log(roomToUpdate);
        this.formlyData.model = {};

        this.formlyData.model = {
          id: roomToUpdate.id,
          identifier:  roomToUpdate.identifier,
          floor: roomToUpdate.floor,
          description: roomToUpdate.description,
          labels: roomToUpdate.labels,
        }

        this.labels.options = roomToUpdate.labels;
        this.supervisories.options = roomToUpdate.supervisories;
      })
    )
  }

  onSubmit(model) {
    let addRoomModel: AddRoom = {
      id: model.id,
      identifier: model.identifier,
      description: model.description,
      floor: model.floor,
      labels: this.labels.options,
      supervisories: this.supervisories.options
    }

    let endPoint = this.roomService.createRoom(addRoomModel as AddRoom);
    if(addRoomModel.id != undefined)
      endPoint = this.roomService.updateRoom(addRoomModel as AddRoom)
    
    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.snackService.snack(result);

        console.log(this.dialogRef);
        if(result.status == 1){
          this.formlyData.model = {};
          this.labels.options = [];
          this.supervisories.options = [];

          if(this.dialogRef != undefined)
            this.dialogRef.close();
        }
      })
    )
  }
}
