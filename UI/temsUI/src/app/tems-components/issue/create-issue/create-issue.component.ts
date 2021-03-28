import { UserService } from 'src/app/services/user-service/user.service';
import { TokenService } from './../../../services/token-service/token.service';
import { SnackService } from './../../../services/snack/snack.service';
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
  isRegistered: boolean;
  sent = false;
  
  private formlyData = {
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
    private roomService: RoomsService,
    private userService: UserService,
    private tokenService: TokenService,
    private personnelService: PersonnelService,
    private equipmentService: EquipmentService,
    private issueService: IssuesService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.isRegistered = this.tokenService.hasClaim('UserID');
    this.subscriptions.push(this.issueService.getStatuses()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
          
        this.formlyData.fields = this.formlyParserService.parseAddIssue(new AddIssue, this.frequentProblems, result);
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
