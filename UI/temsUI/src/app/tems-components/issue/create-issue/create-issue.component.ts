import { UserService } from 'src/app/services/user.service';
import { TokenService } from '../../../services/token.service';
import { SnackService } from '../../../services/snack.service';
import { IssuesService } from '../../../services/issues.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { IOption } from './../../../models/option.model';
import { EquipmentService } from 'src/app/services/equipment.service';
import { PersonnelService } from '../../../services/personnel.service';
import { RoomsService } from '../../../services/rooms.service';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { Component, Inject, Input, OnInit, Optional, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddIssue } from 'src/app/models/communication/issues/add-issue.model';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-create-issue',
  templateUrl: './create-issue.component.html',
  styleUrls: ['./create-issue.component.scss']
})
export class CreateIssueComponent extends TEMSComponent implements OnInit {

  frequentProblems = ['Echipament Defect', 'Incarcare Cartus', 'Interventia unui tehnician'];
  isRegistered: boolean;
  sent = false;
  
  public formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  @Input() roomsAlreadySelected: IOption[] = [];
  @Input() equipmentAlreadySelected: IOption[] = [];
  @Input() personnelAlreadySelected: IOption[] = [];
  @Input() assigneesAlreadySelected: IOption[] = [];

  @ViewChild('assignees') assignees;
  @ViewChild('rooms') rooms;
  @ViewChild('personnel') personnel;
  @ViewChild('equipment') equipment;

  dialogRef;

  constructor(
    private formlyParserService: FormlyParserService,
    public roomService: RoomsService,
    public userService: UserService,
    private tokenService: TokenService,
    public personnelService: PersonnelService,
    public equipmentService: EquipmentService,
    private issueService: IssuesService,
    private snackService: SnackService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(dialogData != undefined){
      this.equipmentAlreadySelected = dialogData.equipmentAlreadySelected;
      this.personnelAlreadySelected = dialogData.personnelAlreadySelected;
      this.roomsAlreadySelected = dialogData.roomsAlreadySelected;
      this.assigneesAlreadySelected = dialogData.assigneesAlreadySelected;
    }
  }

  ngOnInit(): void {
    this.isRegistered = this.tokenService.hasClaim('UserID');
    this.subscriptions.push(this.issueService.getStatuses()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
          
        this.formlyData.fields = this.formlyParserService.parseAddIssue(new AddIssue, this.frequentProblems, result);
        this.formlyData.isVisible = true;
      }))
  }

  onSubmit(model){
    model.issue.personnel = this.personnel.options as IOption[];
    model.issue.rooms = this.rooms.options as IOption[];
    model.issue.equipments = this.equipment.options as IOption[];
    model.issue.assignees = (this.isRegistered) ? this.assignees.options : [];

    let addIssue = model.issue as AddIssue;
    
    console.log(addIssue);
    this.subscriptions.push(this.issueService.createIssue(addIssue)
      .subscribe(result => {
        this.snackService.snack(result);
        
        if(result.status == 1){
          (this.dialogRef != undefined) ? this.dialogRef.close() : this.sent = true; 
        }
      }))
  }
}
