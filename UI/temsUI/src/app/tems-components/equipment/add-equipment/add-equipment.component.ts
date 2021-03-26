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

@Component({
  selector: 'app-add-equipment',
  templateUrl: './add-equipment.component.html',
  styleUrls: ['./add-equipment.component.scss'],
  providers: []
})
export class AddEquipmentComponent extends TEMSComponent implements OnInit {

  header = "Add Equipment";

  @Input() updateEquipmentId: string;
  @Input() updateEquipmentDefinitionId: string;
  updateEquipment: AddEquipment;

  @Output() done = new EventEmitter();

  formGroup = new FormGroup({
    equipmentType: new FormControl(),
    equipmentDefinition: new FormControl(),
    properties: new FormControl(),
  });

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  types: IOption[] = [];
  selectedType: IOption;

  definitionsOfType: IOption[];
  selectedDefinition: IOption;
  selectedFullDefinition: Definition = undefined;

  constructor(
    private equipmentService: EquipmentService,
    private formlyParserService: FormlyParserService,
    private dialogService: DialogService) {
    super();
  }

  ngOnInit(): void {

    if(this.updateEquipmentId != undefined){
      this.header = "Update Equipment";
      this.edit();
      return;
    }

    this.subscriptions.push(
      this.equipmentService.getTypes()
      .subscribe(response => {
      this.types = response;
    }));
  }

  edit(){
    // 1) get equipment's full definition
    if(this.updateEquipmentDefinitionId != undefined){
      this.subscriptions.push(
        this.equipmentService.getFullDefinition(this.updateEquipmentDefinitionId)
        .subscribe(result => {

          this.selectedFullDefinition = result; 
          console.log('fulldefinition: ');
          console.log(this.selectedFullDefinition);

           // 2) Generate formly fields
           this.createAddEquipmentFormly();

            // 3) Get equipment from db and complete the formly model
           this.fetchEquipmentData();
        })
      )
    }
  }

  fetchEquipmentData(){
    this.subscriptions.push(
      this.equipmentService.getEquipmentToUpdate(this.updateEquipmentId)
      .subscribe(result => {
        console.log('this is what i got');
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
        this.selectedFullDefinition = result; 
      })
    )
  }

  onTypeChanged(eventData) {
    if (eventData.value == undefined)
      return;

    // true if the user has inserted some data into the 
    // formly and now he wants to switch to another type, therefore the formly model will 
    // get completely wiped out
    let isUnsavedModel = false;

    if (!isUnsavedModel ||
      isUnsavedModel && confirm('There are unsaved changes, do you still want to continue?')) {
      this.wipeAddEquipmentFormly();
    }

    if (this.types.find(q => q.value == eventData.value) != undefined)
      this.subscriptions.push(this.equipmentService.getDefinitionsOfType(eventData.value)
        .subscribe(response => {
          console.log(response);
          this.definitionsOfType = response;
        }))
    else
      this.definitionsOfType = undefined;
  }

  onDefinitionChanged(eventData) {
    if (eventData.value == undefined)
      return;

    if (this.definitionsOfType.find(q => q.value == eventData.value) != undefined) {
      this.subscriptions.push(this.equipmentService.getFullDefinition(eventData.value)
        .subscribe(response => {
          console.log(response);
          this.selectedFullDefinition = response;
          this.createAddEquipmentFormly();
        }))
    }
  }

  createAddEquipmentFormly() {
    let addEq = this.equipmentService.generateAddEquipmentOfDefinition(this.selectedFullDefinition);
    let formlyFields = this.formlyParserService.parseAddEquipment(addEq);
    
    this.formlyData.fields = formlyFields;
    this.formlyData.isVisible = true;
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

    let endPoint = this.equipmentService.addEquipment(submitAddEquipment);
    if(this.updateEquipmentId != undefined)
      endPoint = this.equipmentService.updateEquipment(submitAddEquipment);
    
    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        console.log(result);
        if(result.status == 1)
          this.done.emit();
      })
    )
  }

  openDialog(componentName: string): void {
    switch(componentName){
      case 'add-type':
        this.dialogService.openDialog(AddTypeComponent);
        break;

      case 'add-definition':
        this.dialogService.openDialog(AddDefinitionComponent);
        break;
    }
  }
}