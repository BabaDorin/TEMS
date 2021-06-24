import { SnackService } from 'src/app/services/snack/snack.service';
import { TypeService } from './../../../services/type-service/type.service';
import { Observable } from 'rxjs';
import { EquipmentType } from './../../../models/equipment/view-type.model';
import { IOption } from './../../../models/option.model';
import { AddDefinition, Definition } from './../../../models/equipment/add-definition.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Input, Inject, OnDestroy, Optional, TemplateRef } from '@angular/core';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { FormGroup } from '@angular/forms';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Z } from '@angular/cdk/keycodes';

@Component({
  selector: 'app-add-definition',
  templateUrl: './add-definition.component.html',
  styleUrls: ['./add-definition.component.scss']
})

export class AddDefinitionComponent extends TEMSComponent implements OnInit {

  // Provide a value for this in order to edit a definition instead of adding one.
  updateDefinitionId: string;
  @Input() typeId: string;

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }
  dialogRef;

  addDefinition: Definition;
  equipmentTypes: IOption[];
  eqProperies: string[];

  constructor(
    private formlyParserService: FormlyParserService,
    private equipmentService: EquipmentService,
    private typeService: TypeService,
    private snackService: SnackService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any) {
    super();
    
    if(dialogData != undefined){
      this.updateDefinitionId = dialogData.updateDefinitionId;
      this.typeId = dialogData.typeId;
    }
  }

  ngOnInit(): void {
    // There are two ways of getting the type:
    // 1) The type was sent via matDialog data
    // 2) User chooses the type via select input

    if (this.updateDefinitionId != undefined) {
      this.update();
      return;
    }


    if (this.typeId != undefined)
      this.setDefinitionType(this.typeId);
    else
      this.subscriptions.push(
        this.typeService.getAllAutocompleteOptions()
          .subscribe(response => {
            this.equipmentTypes = response;
          }));
  }

  onSelectionChanged(eventData) {
    this.setDefinitionType(eventData.value);
  }

  setDefinitionType(typeId: string) {
    this.typeId = typeId;

    this.addDefinition = new Definition();
    let parentFullType: EquipmentType;

    console.log('typeId: ' + typeId);
    this.subscriptions.forEach(s => s.unsubscribe);
    this.subscriptions.push(
      this.equipmentService.getFullType(typeId)
        .subscribe(
          response => {
            console.log('type: ');
            console.log(response);

            parentFullType = response;
            this.addDefinition.equipmentType = { value: parentFullType.id, label: parentFullType.name } as IOption;

            // Properties are copied from the definition types because we don't want
            // definition properties to be tight coupled with equipment properties.
            // We may add / remove properties from a definition (this support will be added soon)
            this.addDefinition.properties = parentFullType.properties;

            if (parentFullType.children != undefined)
              parentFullType.children.forEach(childType => {
                let childDefinition = new Definition();
                childDefinition.equipmentType = { value: childType.id, label: childType.name } as IOption;
                childDefinition.properties = childType.properties;

                this.addDefinition.children.push(childDefinition);
              });

            this.formlyData.fields = this.formlyParserService.parseAddDefinition(this.addDefinition);
            this.formlyData.isVisible = true;
          }
        ))
  }

  update() {
    console.log(this.formlyData.model);
    this.subscriptions.push(
      this.equipmentService.getDefinitionToUpdate(this.updateDefinitionId)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;

          let resultDefinition: AddDefinition = result;
          this.setDefinitionType(resultDefinition.typeId);

          let updateDefinition = {
            typeId: result.typeId,
            identifier: result.identifier,
            description: result.description,
            price: result.price,
            currency: result.currency,
          };

          resultDefinition.properties.forEach(property => {
            updateDefinition[property.label] = property.value;
          });

          resultDefinition.children.forEach(child => {

            if(updateDefinition[child.typeId] == undefined)
              updateDefinition[child.typeId] = [];

            updateDefinition[child.typeId].push({
              typeId: child.typeId,
              // identifier: child.identifier,
              identifierSelect: child.id,
              description: child.description,
              price: child.price,
              currency: child.currency,
            });

            child.properties.forEach(property => {
              updateDefinition[child.typeId][updateDefinition[child.typeId].length-1][property.label] = property.value;
              updateDefinition[child.typeId][updateDefinition[child.typeId].length-1][property.label];
            });
          })

          this.formlyData.model = {};
          this.formlyData.model = updateDefinition;
          console.log(this.formlyData.model);
        }));

    // let objectFromServer = {
    //   typeId: 'typeid from server',
    //   identifier: 'identifier from server',
    //   description: 'description from server',
    //   price: "40",
    //   currency: 'eur',
    //   color: 'bw',
    //   model: 'model from server',
    //   frequency: '20',
    //   0: [
    //     {
    //       identifier: 'identifier from server',
    //       description: 'description from server',
    //       price: "40",
    //       currency: 'eur',
    //       color: 'bw',
    //       model: 'model from server',
    //       frequency: '20',
    //     }
    //   ]
    // }
  }

  onSubmit(model) {
    console.log('formly model');
    console.log(this.formlyData.model);

    let addDefinition: AddDefinition = this.generateAddDefinitionModel(model, this.typeId);
    addDefinition.id = this.updateDefinitionId;
    console.log('add definition');
    console.log(addDefinition);

    let endPoint = this.equipmentService.addDefinition(addDefinition);
    if (addDefinition.id != undefined)
      endPoint = this.equipmentService.updateDefinition(addDefinition);

    this.subscriptions.push(
      endPoint
        .subscribe(result => {
          this.snackService.snack(result);
          if (result.status == 1)
            this.dialogRef.close();
        }));
  }

  commonDefinitionProperties = ["Identifier", "IdentifierSelect", "Description", "Price", "Currency", "TypeId"];
  generateAddDefinitionModel(model, typeId): AddDefinition{
    let addDefinition = new AddDefinition();

    if(model.identifierSelect != undefined && model.identifierSelect != "new"){
      addDefinition.id = model.identifierSelect;
      return addDefinition;
    }
    
    addDefinition.typeId = typeId;
    addDefinition.identifier = model.identifier;
    addDefinition.price = model.price;
    addDefinition.description = model.description;
    addDefinition.currency = model.currency;

    var propNames = Object.getOwnPropertyNames(model);
    propNames.forEach(propName => {
      if(!Array.isArray(model[propName])
        && this.commonDefinitionProperties.findIndex(q => q.toLowerCase() == propName.toLowerCase()) == -1){
          addDefinition.properties.push({ value: propName, label: model[propName].toString() } as IOption)
        }

      if(Array.isArray(model[propName])){
        model[propName].forEach(def => {
          addDefinition.children.push(this.generateAddDefinitionModel(def, propName))
        })
      }
    });

    return addDefinition;
  }
}
