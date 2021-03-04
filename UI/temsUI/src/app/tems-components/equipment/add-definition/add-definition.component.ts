import { map } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { IOption } from './../../../models/option.model';
import { Definition } from './../../../models/equipment/add-definition.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Input, Inject, OnDestroy } from '@angular/core';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { FormGroup } from '@angular/forms';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-add-definition',
  templateUrl: './add-definition.component.html',
  styleUrls: ['./add-definition.component.scss']
})

export class AddDefinitionComponent extends TEMSComponent implements OnInit {

  constructor(
    private formlyParserService: FormlyParserService,
    private equipmentService: EquipmentService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef?: MatDialogRef<AddDefinitionComponent>) {
      super();
  }

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  addDefinition: Definition;
  equipmentTypes: IOption[];

  ngOnInit(): void {
    // There are two ways of getting the type:
    // 1) The type was sent via matDialog data
    // 2) User chooses the type via select input
    console.log(this.data);
    if (this.data != undefined)
      this.setDefinitionType(this.data.selectedType);
    else
      this.subscriptions.push(this.equipmentService.getTypes().subscribe(response => {
        this.equipmentTypes = response.map(r => ({value: r.id, label: r.name} as IOption));
      }));
  }

  setDefinitionType(typeId: string) {
    // this.addDefinition = new Definition();
    // let parentFullType = this.equipmentService.getFullType(typeId);

    // this.addDefinition.equipmentType = { value: parentFullType.id, label: parentFullType.name } as IOption;
    
    // // Properties are copied from the definition types because we don't want
    // // definition properties to be tight coupled to equipment properties.
    // // We may add / remove properties from a definition (this support will be added soon)
    // this.addDefinition.properties = parentFullType.properties;

    // if (parentFullType.children != undefined)
    //   parentFullType.children.forEach(childType => {
    //     let childDefinition = new Definition();
    //     childDefinition.equipmentType = { value: childType.id, label: childType.name } as IOption;
    //     childDefinition.properties = childType.properties;

    //     this.addDefinition.children.push(childDefinition);
    //   });

    // this.formlyData.model = {
    //   typeId: typeId
    // };
    // this.formlyData.fields = this.formlyParserService.parseAddDefinition(this.addDefinition);
    // this.formlyData.isVisible = true;
    // this.edit();
  }

  edit() {
    let objectFromServer = {
      typeId: 'typeid from server',
      identifier: 'identifier from server',
      description: 'description from server',
      price: "40",
      currency: 'eur',
      color: 'bw',
      model: 'model from server',
      frequency: '20',
      0: [
        {
          identifier: 'identifier from server',
          description: 'description from server',
          price: "40",
          currency: 'eur',
          color: 'bw',
          model: 'model from server',
          frequency: '20',
        }
      ]
    }

    this.formlyData.model.addDefinition = objectFromServer;
  }

  onSubmit(model) {
    console.log(model);
  }
}
