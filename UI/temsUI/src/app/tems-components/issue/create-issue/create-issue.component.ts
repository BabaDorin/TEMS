import { IssuesService } from './../../../services/issues-service/issues.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { IOption } from './../../../models/option.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddIssue } from 'src/app/models/communication/issues/add-issue.model';

@Component({
  selector: 'app-create-issue',
  templateUrl: './create-issue.component.html',
  styleUrls: ['./create-issue.component.scss']
})
export class CreateIssueComponent extends TEMSComponent implements OnInit {

  frequentProblems = ['Echipament Defect', 'Incarcare Cartus', 'Interventia unui tehnician'];
  isRegistered: boolean; // user, logged in
  
  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  roomsAutoCompleteOptions;
  @Input() roomsAlreadySelected: IOption[] = [];

  equipmentAutoCompleteOptions;
  @Input() equipmentAlreadySelected: IOption[] = [];

  personnelAutocompleteOptions;
  @Input() personnelAlreadySelected: IOption[] = [];

  @ViewChild('assignees') assignees;
  @ViewChild('rooms') rooms;
  @ViewChild('personnel') personnel;
  @ViewChild('equipment') equipment;

  constructor(
    private formlyParserService: FormlyParserService,
    private roomService: RoomsService,
    private personnelService: PersonnelService,
    private equipmentService: EquipmentService,
    private issueService: IssuesService
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.issueService.getStatuses()
      .subscribe(result => {
        console.log(result);
        this.formlyData.fields = this.formlyParserService.parseAddIssue(new AddIssue, this.frequentProblems, result);
      }))

    this.subscriptions.push(this.equipmentService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log(result);
        this.equipmentAutoCompleteOptions = result;
      }));

    this.subscriptions.push(this.roomService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log(result);
        this.roomsAutoCompleteOptions = result;
      }));
    
    this.subscriptions.push(this.personnelService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log(result);
        this.personnelAutocompleteOptions = result;
      }));
    
    this.isRegistered = true;
  }

  onSubmit(model){
    model.issue.personnel = this.personnel.options as IOption[];
    model.issue.rooms = this.rooms.options as IOption[];
    model.issue.equipments = this.equipment.options as IOption[];
    model.issue.assignees = this.assignees.options as IOption[];

    let addIssue = model.issue as AddIssue;
    
    console.log(addIssue);
    this.subscriptions.push(this.issueService.createIssue(addIssue)
      .subscribe(response => {
        console.log(response)
      }))
  }
}
