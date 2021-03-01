import { IOption } from './../../../models/option.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddIssue } from 'src/app/models/communication/issues/add-issue';

@Component({
  selector: 'app-create-issue',
  templateUrl: './create-issue.component.html',
  styleUrls: ['./create-issue.component.scss']
})
export class CreateIssueComponent implements OnInit {

  frequentProblems = ['Echipament Defect', 'Incarcare Cartus', 'Interventia unui tehnician'];
  isRegistered: boolean;
  
  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  roomsAutoCompleteOptions;
  @Input() roomsAlreadySelected: IOption[];

  equipmentAutoCompleteOptions;
  @Input() equipmentAlreadySelected: IOption[];

  personnelAutocompleteOptions;
  @Input() personnelAlreadySelected: IOption[];

  @ViewChild('assignees') assignees;
  @ViewChild('rooms') rooms;
  @ViewChild('personnel') personnel;
  @ViewChild('equipment') equipment;

  constructor(
    private formlyParserService: FormlyParserService,
    private roomService: RoomsService,
    private personnelService: PersonnelService,
    private equipmentService: EquipmentService,
  ) {

  }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParserService.parseAddIssue(new AddIssue, this.frequentProblems);

    this.roomsAutoCompleteOptions = this.roomService.getAllAutocompleteOptions();
    this.equipmentAutoCompleteOptions = this.equipmentService.getAllAutocompleteOptions();
    this.personnelAutocompleteOptions = this.personnelService.getAllAutocompleteOptions();

    if(this.roomsAlreadySelected == undefined)
      this.roomsAlreadySelected = [];

    if(this.equipmentAlreadySelected == undefined)
      this.equipmentAlreadySelected = [];

    if(this.personnelAlreadySelected == undefined)
      this.personnelAlreadySelected = [];
    
    this.isRegistered = true;

    console.log(this.equipmentAlreadySelected);
  }

  onSubmit(model){
    // What to do next =>
    // check if registered or not
    // send data to API
    // Validate data.
    
    model.personnel = this.personnel.options;
    model.rooms = this.rooms.options;
    model.equipment = this.equipment.options;
    model.assignees = this.assignees.options;
    console.log(model);
  }
}
