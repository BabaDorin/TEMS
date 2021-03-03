import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';

@Component({
  selector: 'app-add-property',
  templateUrl: './add-property.component.html',
  styleUrls: ['./add-property.component.scss']
})
export class AddPropertyComponent implements OnInit {

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    private equipmentService: EquipmentService,
    private formlyParser: FormlyParserService
  ) { }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParser.parseAddProperty();
  }

  onSubmit(){
    // It's already validated.
    // Send data to API.
    console.log(this.formlyData.model);
  }
}
