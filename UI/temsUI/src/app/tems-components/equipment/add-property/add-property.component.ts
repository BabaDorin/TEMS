import { TranslateService } from '@ngx-translate/core';
import { Component, Inject, OnInit, Optional } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { EquipmentService } from 'src/app/services/equipment.service';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { AddProperty } from './../../../models/equipment/add-property.model';

@Component({
  selector: 'app-add-property',
  templateUrl: './add-property.component.html',
  styleUrls: ['./add-property.component.scss']
})
export class AddPropertyComponent extends TEMSComponent implements OnInit {

  propertyId: string;

  public formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  };
  
  dialogRef;

  constructor(
    private equipmentService: EquipmentService,
    private formlyParser: FormlyParserService,
    private snackService: SnackService,
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    super();
    this.propertyId = this.propertyId ?? this.dialogData?.propertyId; 
  }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParser.parseAddProperty();

    if(this.propertyId == undefined)
      return;

    this.subscriptions.push(
      this.equipmentService.getPropertyById(this.propertyId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        let model = this.formlyData.model; 

        this.formlyData.model = {
          name: result.name,
        }
        this.formlyData.model.name = result.name;
        this.formlyData.model.displayName = result.displayName;
        this.formlyData.model.dataType = result.dataType;
        this.formlyData.model.description = result.description;
        this.formlyData.model.required = result.required;
      })
    )
  }

  onSubmit(){
    let model = this.formlyData.model; 
    
    let addProperty: AddProperty = {
      id: this.propertyId,
      name: model.name,
      displayName: model.displayName,
      dataType: model.dataType,
      description: model.description,
      required: model.required
    }
    
    let endPoint = this.equipmentService.addProperty(addProperty);
    if(addProperty.id != undefined)
      endPoint = this.equipmentService.updateProperty(addProperty);

    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.snackService.snack(result);
        if(result.status == 1)
          this.dialogRef.close();
      })
    )
  }
}
