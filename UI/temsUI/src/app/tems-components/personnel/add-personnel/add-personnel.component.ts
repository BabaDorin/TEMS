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

  personnelPositions: IOption[];
  @ViewChild('personnelPositionsInput') personnelPositionsInput: ChipsAutocompleteComponent;
  personnelId: string;
  dialogRef;

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    private formlyParserService: FormlyParserService,
    private personnelService: PersonnelService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.formlyData.model = {};
    this.formlyData.fields = this.formlyParserService.parseAddPersonnel(new AddPersonnel());
    this.subscriptions.push(this.personnelService.getPersonnelPositions()
      .subscribe(result => {
        console.log(result);
        this.personnelPositions = result;
      }))

    if(this.personnelId == undefined) return;

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

        this.personnelPositionsInput.options = personnelToUpdate.positions;
      })
    )
  }

  onSubmit(model) {
    let addPersonnel = new AddPersonnel();
    
    addPersonnel.id = this.personnelId;
    addPersonnel.name = model.name;
    addPersonnel.phoneNumber = model.phoneNumber;
    addPersonnel.email = model.email;
    addPersonnel.positions = this.personnelPositionsInput.options;
    console.log(addPersonnel);
    
    let endPoint = this.personnelService.createPersonnel(addPersonnel);
    if(addPersonnel.id != undefined)
      endPoint = this.personnelService.updatePersonnel(addPersonnel);

    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.snackService.snack(result);
        if(this.dialogRef != undefined)
          this.dialogRef.close();
      }))
  }
}
