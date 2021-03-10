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

  private formlyData = {
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
    this.subscriptions.push(this.personnelService.getPersonnelPositions()
      .subscribe(result => {
        console.log(result);
        this.personnelPositions = result;
      }))
  }

  onSubmit(model) {
    model.personnel.positions = this.personnelPositionsInput.options;
    console.log(model);
    
    this.subscriptions.push(this.personnelService.createPersonnel(model.personnel as AddPersonnel)
      .subscribe(result => {
        console.log(result);
      }))
  }
}
