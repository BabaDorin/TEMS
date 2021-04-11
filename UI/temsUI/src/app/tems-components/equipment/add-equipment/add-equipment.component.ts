import { TypeService } from './../../../services/type-service/type.service';
import { SnackService } from './../../../services/snack/snack.service';
import { DialogService } from './../../../services/dialog-service/dialog.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { IOption } from './../../../models/option.model';
import { AddDefinitionComponent } from './../add-definition/add-definition.component';
import { AddTypeComponent } from '.././add-type/add-type.component';
import { FormlyParserService } from './../../../services/formly-parser-service/formly-parser.service';
import { Definition } from '../../../models/equipment/add-definition.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { DefinitionService } from 'src/app/services/definition-service/definition.service';

@Component({
  selector: 'app-add-equipment',
  templateUrl: './add-equipment.component.html',
  styleUrls: ['./add-equipment.component.scss'],
  providers: []
})
export class AddEquipmentComponent extends TEMSComponent implements OnInit {

  header = "Add Equipment";

  @Output() done = new EventEmitter();
  @Output() goBack = new EventEmitter();
  @Input() updateEquipmentId: string;
  @Input() updateEquipmentDefinitionId: string;
  updateEquipment: AddEquipment;

  // For selecting type and definition
  formGroup = new FormGroup({
    equipmentType: new FormControl(),
    equipmentDefinition: new FormControl(),
    properties: new FormControl(),
  });

  // For providing equipment data
  formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  types: IOption[] = [];
  selectedType: string;

  definitionsOfType: IOption[];
  selectedDefinition: IOption;
  selectedFullDefinition: Definition = undefined;

  constructor(
    private equipmentService: EquipmentService,
    private typeService: TypeService,
    private definitionService: DefinitionService,
    private formlyParserService: FormlyParserService,
    private dialogService: DialogService,
    private snackService: SnackService) {
    super();
  }

  ngOnInit(): void {
    if(this.updateEquipmentId != undefined){
      this.header = "Update Equipment";
      this.edit();
      return;
    }
  
    this.fetchTypes();
  }

  // Using this component for updating an equipment
  edit(){
    // 1) get equipment's full definition
    if(this.updateEquipmentDefinitionId != undefined){
      this.subscriptions.push(
        this.equipmentService.getFullDefinition(this.updateEquipmentDefinitionId)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;
          this.selectedFullDefinition = result; 

          // 2) Generate formly fields
          this.createAddEquipmentFormly();

          // 3) Get equipment from backend and complete the formly model
          this.fetchEquipmentData();
        })
      )
    }
  }

  // Used when updating an equipment.
  fetchEquipmentData(){
    this.subscriptions.push(
      this.equipmentService.getEquipmentToUpdate(this.updateEquipmentId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        console.log(result);
        this.formlyData.model = {};
        this.formlyData.model.equipmentDefinitionID = this.selectedFullDefinition.id;
        this.formlyData.model.identifier = this.selectedFullDefinition.identifier;
        this.formlyData.model.temsid = result.temsid;
        this.formlyData.model.serialNumber = result.serialNumber;
        this.formlyData.model.isDefect = result.isDefect;
        this.formlyData.model.isUsed = result.isUsed;
        this.formlyData.model.description = result.description;
        this.formlyData.model.price = result.price;
        this.formlyData.model.currency = (result.currency != undefined) ? result.currency : this.selectedFullDefinition.currency;
        this.formlyData.model.purchaseDate = result.purchaseDate.toString().split('T')[0];
      })
    )
  }

  fetchFullDefinition(){
    this.subscriptions.push(
      this.equipmentService.getFullDefinition(this.updateEquipmentDefinitionId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        console.log(result);
        this.selectedFullDefinition = result; 
      })
    )
  }

  fetchTypes(){
    this.subscriptions.push(
      this.typeService.getAllAutocompleteOptions()
      .subscribe(response => {
        if(this.snackService.snackIfError(response))
          return;
        
        console.log('types:');
        console.log(response);
        this.types = response;
    }));
  }

  fetchDefinitionsOfType(){
    console.log(this.selectedType);
    if (this.types.find(q => q.value == this.selectedType) != undefined)
      this.subscriptions.push(this.definitionService.getDefinitionsOfType(this.selectedType)
        .subscribe(response => {
          if(this.snackService.snackIfError(response))
            return;
          this.definitionsOfType = response;
        }))
    else
      this.definitionsOfType = undefined;
  }

  onTypeChanged(eventData) {
    if (eventData.value == undefined)
      return;

    this.wipeAddEquipmentFormly();
    this.fetchDefinitionsOfType();
  }

  onDefinitionChanged(eventData) {
    if (eventData.value == undefined)
      return;

    if (this.definitionsOfType.find(q => q.value == eventData.value) != undefined) {
      this.subscriptions.push(
        this.equipmentService.getFullDefinition(eventData.value)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;
          console.log(result);
          this.selectedFullDefinition = result;
          this.createAddEquipmentFormly();
        }))
    }
  }

  createAddEquipmentFormly() {
    let addEq: AddEquipment = this.equipmentService.generateAddEquipmentOfDefinition(this.selectedFullDefinition);
    console.log('add equipment:');
    console.log(addEq);
    let formlyFields = this.formlyParserService.parseAddEquipment(addEq);
    
    this.formlyData.fields = formlyFields;
    this.formlyData.isVisible = true;

    // BEFREE
    this.formlyData.model = { 
      equipmentDefinitionID: addEq.definition.id,
      identifier: this.selectedFullDefinition.identifier
    };
  }

  wipeAddEquipmentFormly() {
    this.selectedDefinition = { value: '', label: '' } as IOption;
    this.selectedFullDefinition = undefined;
    this.formlyData.isVisible = false;
    this.formlyData.fields = [];
    this.formlyData.model = {};
  }

  onSubmit(model) {
    let submitAddEquipment = new AddEquipment();
    submitAddEquipment = model;
    submitAddEquipment.id = this.updateEquipmentId;
    submitAddEquipment.children = [];

    console.log('model:');
    console.log(model);
    var propNames = Object.getOwnPropertyNames(model);
    propNames.forEach(propName => {
      if(Array.isArray(model[propName]) && propName.indexOf("---") > -1){
        console.log('array found: ');
        console.log(model[propName]);
          model[propName].forEach(child => {
            console.log('child:');
            console.log(child);
            let childEquipment = new AddEquipment();
            childEquipment = child;
            childEquipment.equipmentDefinitionID = propName.split('---')[0];
            submitAddEquipment.children.push(childEquipment);
          })
        }
    });

    let endPoint = this.equipmentService.addEquipment(submitAddEquipment);
    if(this.updateEquipmentId != undefined)
      endPoint = this.equipmentService.updateEquipment(submitAddEquipment);
    
    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1){
          this.createAddEquipmentFormly();
        }
      })
    )
  }

  openDialog(componentName: string): void {
    switch(componentName){
      case 'add-type':
        this.dialogService.openDialog(
          AddTypeComponent,
          undefined,
          () => {
            this.fetchTypes();
          });
        break;

      case 'add-definition':
        this.dialogService.openDialog(
          AddDefinitionComponent,
          [{label: "typeId", value: this.selectedType}],
          () => {
            this.fetchDefinitionsOfType();
          });
        break;
    }
  }

  back(){
    this.goBack.emit();
  }
}