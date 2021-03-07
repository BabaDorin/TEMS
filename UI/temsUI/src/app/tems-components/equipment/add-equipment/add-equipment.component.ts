import { AddDefinition } from './../../../models/equipment/add-definition.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Subscription } from 'rxjs';
import { IOption } from './../../../models/option.model';
import { AddDefinitionComponent } from './../add-definition/add-definition.component';
import { AddTypeComponent } from '.././add-type/add-type.component';
import { FormlyParserService } from './../../../services/formly-parser-service/formly-parser.service';
import { Definition } from '../../../models/equipment/add-definition.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, ControlValueAccessor } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ComponentType } from '@angular/cdk/portal';

@Component({
  selector: 'app-add-equipment',
  templateUrl: './add-equipment.component.html',
  styleUrls: ['./add-equipment.component.scss'],
  providers: []
})
export class AddEquipmentComponent extends TEMSComponent implements OnInit {

  constructor(
    private equipmentService: EquipmentService,
    private formlyParserService: FormlyParserService,
    public dialog: MatDialog) {
      super();
    }
  
  formGroup = new FormGroup({
    equipmentType: new FormControl(),
    equipmentDefinition: new FormControl(),
    properties: new FormControl(),
  });

  types: IOption[] = [];
  ngOnInit(): void {
    this.subscriptions.push(this.equipmentService.getTypes().subscribe(response => {
      this.types = response.map(r => ({value: r.id, label: r.name} as IOption));
    }));
  }

  // type related -------------------------------------------------------------------------------------------------
  selectedType: IOption;

  onTypeChanged(eventData) {
    if(eventData.value == undefined)
      return;

    // true if the user has inserted some data into the 
    // formly and now he wants to switch to another type, therefore the formly model will 
    // get completely wiped out
    let isUnsavedModel = false; 

    if (!isUnsavedModel ||
      isUnsavedModel && confirm('There are unsaved changes, do you still want to continue?')) {
      this.wipeAddEquipmentFormly();
    }

    console.log('event data:');
    console.log(eventData);
    if (this.types.find(q => q.value == eventData.value) != undefined)
      this.subscriptions.push(this.equipmentService.getDefinitionsOfType(eventData.value)
        .subscribe(response => {
          if(response.status == undefined)
            this.definitionsOfType = response;
          else
            console.log(response); // error
        }))
    else
      this.definitionsOfType = undefined;
  }

  // definition related -------------------------------------------------------------------------------------------

  definitionsOfType: IOption[];
  selectedDefinition: IOption;
  selectedFullDefinition: Definition = undefined;

  onDefinitionChanged(eventData){
    if(eventData.value == undefined)
      return;
    
    if (this.definitionsOfType.find(q => q.value == eventData.value) != undefined)
    {
      this.subscriptions.push(this.equipmentService.getFullDefinition(eventData.value)
        .subscribe(response => {
          console.log(response);
          this.selectedFullDefinition = response;
          this.createAddEquipmentFormly();
        }))
    }
  }

  // formly related -----------------------------------------------------------------------------------------------

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  // True if for the parent equipment has been assigned a TEMSID or SerialNumber.
  private readyToBeSaved: boolean = false;

  createAddEquipmentFormly() {
    let addEq = this.equipmentService.generateAddEquipmentOfDefinition(this.selectedFullDefinition);
    let formlyFields = this.formlyParserService.parseAddEquipment(addEq);

    // this.formlyData.model = new AddEquipment(this.selectedFullDefinition);
    this.formlyData.model = { equipmentDefinitionID: addEq.definition.id };
    this.formlyData.fields = formlyFields;
    this.formlyData.isVisible = true;
  }

  wipeAddEquipmentFormly() {
    this.selectedDefinition = { value: '', label: '' } as IOption;
    this.selectedFullDefinition = undefined;
    this.formlyData.isVisible = false;
    this.formlyData.fields = [];
    this.formlyData.model = {};
  }

  onSubmit(model) {
    // Validate & send data to API
    let submitAddEquipment = new AddEquipment();
    
    console.log(model)
    submitAddEquipment = model;
    console.log(submitAddEquipment)

    this.subscriptions.push(this.equipmentService.createEquipment(submitAddEquipment)
      .subscribe(response => {
        console.log(response);
      }));
  }

  // Angular Material Dialog --------------------------------------------------------------------------------------
  openDialog(componentName: string): void {
    let componentType: ComponentType<any>;

    let dialogRef: MatDialogRef<any>;
    switch (componentName) {
      case 'add-type':
        dialogRef = this.dialog.open(AddTypeComponent);
        break;

      case 'add-definition':
        dialogRef = this.dialog.open(AddDefinitionComponent, {
          data: {
            selectedType: this.selectedType,
          },
          maxHeight: '80vh',
          autoFocus: false
        });
        break;
    }

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
}