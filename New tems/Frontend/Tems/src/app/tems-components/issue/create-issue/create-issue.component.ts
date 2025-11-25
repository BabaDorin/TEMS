import { Component, Inject, Input, OnInit, Optional, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddIssue } from 'src/app/models/communication/issues/add-issue.model';
import { EquipmentService } from 'src/app/services/equipment.service';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { UserService } from 'src/app/services/user.service';
import { IssuesService } from '../../../services/issues.service';
import { PersonnelService } from '../../../services/personnel.service';
import { RoomsService } from '../../../services/rooms.service';
import { SnackService } from '../../../services/snack.service';
import { TokenService } from '../../../services/token.service';
import { IOption } from './../../../models/option.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { TEMS_FORMS_IMPORTS } from 'src/app/shared/constants/tems-forms-imports.const';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ChipsAutocompleteComponent } from '../../../public/formly/chips-autocomplete/chips-autocomplete.component';

@Component({
  selector: 'app-create-issue',
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

  constructor(
    private formlyParserService: FormlyParserService,
    public roomService: RoomsService,
    public userService: UserService,
    private tokenService: TokenService,
    public personnelService: PersonnelService,
    public equipmentService: EquipmentService,
    private issueService: IssuesService,
    private snackService: SnackService,
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any,
    @Optional() public dialogRef: MatDialogRef<CreateIssueComponent>
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
    model.issue.personnel = this.personnel.selectOptions as IOption[];
    model.issue.rooms = this.rooms.selectOptions as IOption[];
    model.issue.equipments = this.equipment.selectOptions as IOption[];
    model.issue.assignees = (this.isRegistered) ? this.assignees.selectOptions : [];

    let addIssue = model.issue as AddIssue;
    
    this.subscriptions.push(this.issueService.createIssue(addIssue)
      .subscribe(result => {
        this.snackService.snack(result);
        
        if(result.status == 1){
          (this.dialogRef != undefined) ? this.dialogRef.close() : this.sent = true; 
        }
      }))
  }
}
