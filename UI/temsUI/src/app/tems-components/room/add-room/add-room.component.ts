import { IOption } from '../../../models/option.model';
import { RoomsService } from '../../../services/rooms-service/rooms.service';
import { FormlyParserService } from '../../../services/formly-parser-service/formly-parser.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddRoom } from 'src/app/models/room/add-room.model';

@Component({
  selector: 'app-add-room',
  templateUrl: './add-room.component.html',
  styleUrls: ['./add-room.component.scss']
})
export class AddRoomComponent implements OnInit {

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  roomLabels: IOption[];

  constructor(
    private formlyParserService: FormlyParserService,
    private roomService: RoomsService,
  ) { }


  
  ngOnInit(): void {
    this.formlyData.model ={};
    this.roomLabels = this.roomService.getRoomLabels();
    this.formlyData.fields = this.formlyParserService.parseAddRoom(
      new AddRoom(), 
      this.roomLabels.map(q => q.value)
      );
  }


  onSubmit(model) {
    console.log(model);
  }
}
