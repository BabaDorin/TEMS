import { UserService } from './../../../services/user-service/user.service';
import { SnackService } from './../../../services/snack/snack.service';
import { ChipsAutocompleteComponent } from './../../../public/formly/chips-autocomplete/chips-autocomplete.component';
import { IOption } from './../../../models/option.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { AddPersonnel } from 'src/app/models/personnel/add-personnel.model';

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

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    private formlyParserService: FormlyParserService,
    private personnelService: PersonnelService,
    private userService: UserService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.formlyData.model = {};
    this.formlyData.fields = this.formlyParserService.parseAddPersonnel(new AddPersonnel());

    this.subscriptions.push(this.personnelService.getPersonnelPositions()
      .subscribe(result => {
        this.personnelPositions = result;
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
          positions: personnelToUpdate.positions
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
      }))
  }
}
