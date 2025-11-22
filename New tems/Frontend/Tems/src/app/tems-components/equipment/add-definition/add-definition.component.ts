import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { EquipmentService } from 'src/app/services/equipment.service';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { TypeService } from '../../../services/type.service';
import { AddDefinition, Definition } from './../../../models/equipment/add-definition.model';
import { EquipmentType } from './../../../models/equipment/view-type.model';
import { IOption } from './../../../models/option.model';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { TEMS_FORMS_IMPORTS } from 'src/app/shared/constants/tems-forms-imports.const';

@Component({
  selector: 'app-add-definition',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatOptionModule,
    MatButtonModule,
    TranslateModule,
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './add-definition.component.html',
  styleUrls: ['./add-definition.component.scss']
})

export class AddDefinitionComponent extends TEMSComponent implements OnInit {

  // Provide a value for this in order to edit a definition instead of adding one.
  updateDefinitionId: string;
  @Input() typeId: string;

  formlyData = {
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

    this.subscriptions.forEach(s => s.unsubscribe);
    this.subscriptions.push(
      this.typeService.getFullType(typeId)
        .subscribe(
          response => {
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
        }));
  }

  onSubmit(model) {
    let addDefinition: AddDefinition = this.generateAddDefinitionModel(model, this.typeId);
    addDefinition.id = this.updateDefinitionId;

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
