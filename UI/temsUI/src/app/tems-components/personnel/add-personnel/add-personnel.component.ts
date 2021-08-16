import { IOption } from 'src/app/models/option.model';
import { Component, Inject, OnInit, Optional, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AddPersonnel } from 'src/app/models/personnel/add-personnel.model';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { PersonnelService } from 'src/app/services/personnel.service';
import { SnackService } from '../../../services/snack.service';
import { UserService } from '../../../services/user.service';
import { FormlyData } from './../../../models/formly/formly-data.model';
import { ChipsAutocompleteComponent } from './../../../public/formly/chips-autocomplete/chips-autocomplete.component';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-add-personnel',
  templateUrl: './add-personnel.component.html',
  styleUrls: ['./add-personnel.component.scss']
})
export class AddPersonnelComponent extends TEMSComponent implements OnInit {

  // The personnel to be edited
  personnelId: string;

  // Aldready selected:
  personnelPositions: IOption[];
  user: IOption[];

  // ChipsAutocomplete references
  @ViewChild('personnelPositionsChips') personnelPositionsChips: ChipsAutocompleteComponent;
  @ViewChild('userChips') userChips: ChipsAutocompleteComponent;
  
  dialogRef;

  public formlyData = new FormlyData();

  constructor(
    private formlyParserService: FormlyParserService,
    private personnelService: PersonnelService,
    public userService: UserService,
    public translate: TranslateService,
    private snackService: SnackService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(dialogData != undefined){
      this.personnelId = dialogData.personnelId;
    }
  }

  ngOnInit(): void {
    this.formlyData.model = {};
    this.formlyData.fields = this.formlyParserService.parseAddPersonnel(new AddPersonnel());

    this.subscriptions.push(this.personnelService.getPersonnelPositions()
      .subscribe(result => {
        this.personnelPositions = result.map(q => ({ 
          label: this.translate.instant('personnel.positionOptions.' + q.label), 
          value: q.value 
        } as IOption));
      }))

    if(this.personnelId == undefined) 
      return;

    this.subscriptions.push(
      this.personnelService.getPersonnelToUpdate(this.personnelId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result)) return;

        let personnelToUpdate: AddPersonnel = result;
        this.formlyData.model = {
          name: personnelToUpdate.name,
          phoneNumber: personnelToUpdate.phoneNumber,
          email: personnelToUpdate.email,
          positions: personnelToUpdate.positions.map(q => ({ 
            label: this.translate.instant('personnel.positionOptions.' + q.label), 
            value: q.value 
          } as IOption))
        };

        this.personnelPositionsChips.options = personnelToUpdate.positions;
        this.user = (personnelToUpdate.user == undefined) ? [] : [personnelToUpdate.user];
      })
    )
  }

  onSubmit(model) {
    let addPersonnel = new AddPersonnel();
    
    addPersonnel.id = this.personnelId;
    addPersonnel.name = model.name;
    addPersonnel.phoneNumber = model.phoneNumber;
    addPersonnel.email = model.email;
    addPersonnel.positions = this.personnelPositionsChips.options;
    
    if(this.userChips.options != undefined)
      addPersonnel.user = this.userChips.options[0];

    let endPoint = this.personnelService.createPersonnel(addPersonnel);

    if(addPersonnel.id != undefined)
      endPoint = this.personnelService.updatePersonnel(addPersonnel);

    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.snackService.snack(result);
        
        if(result.status == 1)
          if(this.dialogRef != undefined)
            this.dialogRef.close();

          this.clearForm();
      }))
  }

  clearForm(){
    this.formlyData.model = {};
    this.personnelPositionsChips.options = [];
    this.userChips.options = [];
  }
}
