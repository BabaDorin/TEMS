import { TEMSComponent } from 'src/app/tems/tems.component';
import { AddProperty } from './../../../models/equipment/add-property.model';
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
export class AddPropertyComponent extends TEMSComponent implements OnInit {

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    private equipmentService: EquipmentService,
    private formlyParser: FormlyParserService
  ) { super(); }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParser.parseAddProperty();
  }

  onSubmit(){
    let addProperty: AddProperty = {
      name: this.formlyData.model.addProperty.name,
      displayName: this.formlyData.model.addProperty.displayName,
      dataType: this.formlyData.model.addProperty.dataType,
      description: this.formlyData.model.addProperty.description,
      required: this.formlyData.model.addProperty.required
    }
    console.log(addProperty);
    this.subscriptions.push(this.equipmentService.postProperty(addProperty)
      .subscribe(response => console.log(response)));
  }
}
