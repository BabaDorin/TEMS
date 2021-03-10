import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { IOption } from 'src/app/models/option.model';
import { AddRoom } from 'src/app/models/room/add-room.model';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { RoomsService } from 'src/app/services/rooms-service/rooms.service';
import { AddPersonnel } from 'src/app/models/personnel/add-personnel.model';

@Component({
  selector: 'app-add-personnel',
  templateUrl: './add-personnel.component.html',
  styleUrls: ['./add-personnel.component.scss']
})
export class AddPersonnelComponent extends TEMSComponent implements OnInit {

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    private formlyParserService: FormlyParserService,
    private personnelService: PersonnelService
  ) {
    super();
  }

  ngOnInit(): void {
    this.formlyData.model = {};
    this.formlyData.fields = this.formlyParserService.parseAddPersonnel(new AddPersonnel());
  }

  onSubmit(model) {
    console.log(model);
    this.subscriptions.push(this.personnelService.createPersonnel(model.personnel as AddPersonnel)
      .subscribe(result => {
        console.log(result);
      }))
  }
}
