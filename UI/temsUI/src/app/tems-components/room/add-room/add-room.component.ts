import { TEMSComponent } from './../../../tems/tems.component';
import { ChipsAutocompleteComponent } from './../../../public/formly/chips-autocomplete/chips-autocomplete.component';
import { IOption } from '../../../models/option.model';
import { RoomsService } from '../../../services/rooms-service/rooms.service';
import { FormlyParserService } from '../../../services/formly-parser-service/formly-parser.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddRoom } from 'src/app/models/room/add-room.model';

@Component({
  selector: 'app-add-room',
  templateUrl: './add-room.component.html',
  styleUrls: ['./add-room.component.scss']
})
export class AddRoomComponent extends TEMSComponent implements OnInit {

  @ViewChild('labels') labels: ChipsAutocompleteComponent;

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  roomLabels: IOption[];

  constructor(
    private formlyParserService: FormlyParserService,
    private roomService: RoomsService,
  ) {
    super();
  }
  
  ngOnInit(): void {
    this.formlyData.model ={};
    
    this.subscriptions.push(this.roomService.getRoomLabels()
      .subscribe(result => {
        console.log(result);
        this.roomLabels = result;
      }))

    this.formlyData.fields = this.formlyParserService.parseAddRoom();
  }

  onSubmit(model) {
    model.room.labels = this.labels.options;
    this.subscriptions.push(this.roomService.createRoom(model.room as AddRoom)
      .subscribe(result => {
        console.log(result);
      }))
  }
}
