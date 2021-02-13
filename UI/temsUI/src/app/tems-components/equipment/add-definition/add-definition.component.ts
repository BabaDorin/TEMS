import { AddDefinition } from './../../../models/equipment/add-definition.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Input, Inject } from '@angular/core';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-add-definition',
  templateUrl: './add-definition.component.html',
  styleUrls: ['./add-definition.component.scss']
})
export class AddDefinitionComponent implements OnInit {

  constructor(
    private formlyParserService : FormlyParserService,
    private equipmentService: EquipmentService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef?: MatDialogRef<AddDefinitionComponent>) {
  }


  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  ngOnInit(): void {
    let parentFullType = this.equipmentService.getFullType(this.data.selectedType.id);
    
    let addDefinition = new AddDefinition();
    addDefinition.equipmentType = parentFullType;
    
    // Properties are copied from the definition types because we don't want
    // definition properties to be tight coupled to equipment properties.
    // We may add / remove properties from a definition (this support will be added soon)
    addDefinition.properties = addDefinition.equipmentType.properties;

    parentFullType.children.forEach(childType => {
      let childDefinition = new AddDefinition();
      childDefinition.equipmentType = childType;
      childDefinition.properties = childType.properties;
      
      addDefinition.children.push(childDefinition);
    });

    this.formlyData.model = {};
    this.formlyData.fields = this.formlyParserService.parseAddDefinition(addDefinition);
    // this.formlyData.fields = [];
    this.formlyData.isVisible = true;
  }

  onSubmit(model){
    // Send data to API
    console.log('submitted');
    console.log(model);
  }
}
