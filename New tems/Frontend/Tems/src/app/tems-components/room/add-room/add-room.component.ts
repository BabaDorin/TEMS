import { Component, Inject, OnInit, Optional, ViewChild, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TEMS_FORMS_IMPORTS } from 'src/app/shared/constants/tems-forms-imports.const';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { FormlyData } from 'src/app/models/formly/formly-data.model';
import { AddRoom } from 'src/app/models/room/add-room.model';
import { IOption } from '../../../models/option.model';
import { FormlyParserService } from '../../../services/formly-parser.service';
import { PersonnelService } from '../../../services/personnel.service';
import { RoomsService } from '../../../services/rooms.service';
import { SnackService } from '../../../services/snack.service';
import { ChipsAutocompleteComponent } from './../../../public/formly/chips-autocomplete/chips-autocomplete.component';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-add-room',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    TranslateModule,
    ChipsAutocompleteComponent,
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './add-room.component.html',
  styleUrls: ['./add-room.component.scss']
})

export class AddRoomComponent extends TEMSComponent implements OnInit, OnDestroy {

  roomId: string;
  @ViewChild('labels') labels: ChipsAutocompleteComponent;
  @ViewChild('supervisories') supervisories: ChipsAutocompleteComponent;

  public formlyData = new FormlyData();

  roomLabels: IOption[] = [];

  constructor(
    private formlyParserService: FormlyParserService,
    public roomService: RoomsService,
    private snackService: SnackService,
    public personnelService: PersonnelService,
    public translate: TranslateService,
    @Optional() public dialogRef: MatDialogRef<AddRoomComponent>,
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

    this.subscriptions.push(
      this.roomService.getLabels()
      .subscribe(result => {
        this.roomLabels = result.map(q => ({ 
          label: this.translate.instant('room.labelOptions.' + q.label), 
          value: q.value 
        } as IOption));
      })
    )

    if(this.roomId == undefined)
      return;
    
    this.subscriptions.push(
      this.roomService.getRoomToUpdate(this.roomId)
      .subscribe(result => {
        let roomToUpdate: AddRoom = result;
        this.formlyData.model = {};

        this.formlyData.model = {
          id: roomToUpdate.id,
          identifier:  roomToUpdate.identifier,
          floor: roomToUpdate.floor,
          description: roomToUpdate.description,
          labels: roomToUpdate.labels,
        }

        this.labels.selectOptions = roomToUpdate.labels.map(q => ({ 
          label: this.translate.instant('room.labelOptions.' + q.label), 
          value: q.value 
        } as IOption));
        this.supervisories.selectOptions = roomToUpdate.supervisories;
      })
    )
  }

  onSubmit(model) {
    let addRoomModel: AddRoom = {
      id: model.id,
      identifier: model.identifier,
      description: model.description,
      floor: model.floor,
      labels: this.labels.selectOptions,
      supervisories: this.supervisories.selectOptions
    }

    let endPoint = this.roomService.createRoom(addRoomModel as AddRoom);
    if(addRoomModel.id != undefined)
      endPoint = this.roomService.updateRoom(addRoomModel as AddRoom)
    
    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1){
          this.formlyData.model = {};
          this.labels.selectOptions = [];
          this.supervisories.selectOptions = [];

          if(this.dialogRef != undefined)
            this.dialogRef.close();
        }
      })
    )
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
