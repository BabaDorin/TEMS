import { AddDefinitionComponent } from './../add-definition/add-definition.component';
import { AddTypeComponent } from '.././add-type/add-type.component';
import { FormlyParserService } from './../../../services/formly-parser-service/formly-parser.service';
import { AddDefinition } from '../../../models/equipment/add-definition.model';
import { LightDefinition } from '../../../models/equipment/viewlight-definition.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Type } from '../../../models/equipment/view-type.model';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ComponentType } from '@angular/cdk/portal';
// import { Overlay } from '@angular/cdk/overlay';

@Component({
  selector: 'app-add-equipment',
  templateUrl: './add-equipment.component.html',
  styleUrls: ['./add-equipment.component.scss'],
  providers: []
})
export class AddEquipmentComponent implements OnInit {

  constructor(
    private equipmentService: EquipmentService,
    private formlyParserService: FormlyParserService,
    // private overlay: Overlay,
    public dialog: MatDialog) { }

  ngOnInit(): void {
    this.types = this.equipmentService.getTypes();
  }

  // type related -------------------------------------------------------------------------------------------------

  types: Type[];
  selectedType: Type = { id: '', name: '' };
  private typeSelected: boolean = false;
  private chosenEquipmentType: string = "Choose an equipment type"

  typeHasBeenSelected(type: Type) {
    // clean the interface from previously selected definition (if it exists)
    let isUnsavedModel = false; // true if the user has inserted some data into the 
    // formly and now he wants to switch to another type, therefore the formly model will 
    // get completely wiped out
    if (!isUnsavedModel ||
      isUnsavedModel && confirm('There are unsaved changes, do you still want to continue?')) {
      this.wipeAddEquipmentFormly();
    }

    this.selectedType = type;
    // here we check if the selected type is valid and user hasn't done any manipulation
    if (this.types.find(type => type === this.selectedType) != undefined) {
      this.typeSelected = true;
      this.chosenEquipmentType = this.selectedType.name;
      this.definitionsOfType = this.equipmentService.getDefinitionsOfType(this.selectedType);
    }
    else {
      this.typeSelected = false;
      this.definitionsOfType = undefined;
      this.chosenEquipmentType = "Choose an equipment type";
    }
  }

  // definition related -------------------------------------------------------------------------------------------
  
  definitionsOfType: LightDefinition[];
  selectedDefinition: LightDefinition = { id: '', name: '' };
  selectedFullDefinition: AddDefinition = undefined;
  private definitionSelected: boolean = false;
  private chosenEquipmentDefinition: string = "Choose a definition";

  definitionHasBeenSelected(definition: LightDefinition) {

    // here we check if the selected definition is valid and user hasn't done any manipulation
    this.selectedDefinition = definition;
    if (this.definitionsOfType.find(def => def === this.selectedDefinition) != undefined) {

      this.definitionSelected = true;
      this.chosenEquipmentDefinition = this.selectedDefinition.name;
      this.selectedFullDefinition = this.equipmentService.getFullDefinition(this.selectedDefinition.id);

      this.createAddEquipmentFormly();
    }
    else {
      this.chosenEquipmentDefinition = "Choose a " + this.selectedType.name + " definition";
      this.definitionSelected = false;
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

  createAddEquipmentFormly(){
    let addEq = this.equipmentService.generateAddEquipmentOfDefinition(this.selectedFullDefinition);
    let formlyFields = this.formlyParserService.parseAddEquipment(addEq);

    this.formlyData.model = new AddEquipment(this.selectedFullDefinition);
    this.formlyData.fields = formlyFields;
    this.formlyData.isVisible = true;
  }

  wipeAddEquipmentFormly(){
    this.chosenEquipmentDefinition = "Select a definition";
    this.selectedDefinition = { id: '', name: '' };
    this.selectedFullDefinition = undefined;
    this.formlyData.isVisible = false;
    this.formlyData.fields = [];
    this.formlyData.model = {};
  }

  // Validate & send data to API
  onSubmit(model) {
    model.id = this.selectedFullDefinition.id,
    console.log(model);
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
          // const scrollStrategy = this.overlay.scrollStrategies.reposition();
          dialogRef = this.dialog.open(AddDefinitionComponent, {
          data: {
            selectedType: this.selectedType,
          },
          maxHeight: '80vh',
          autoFocus: false
          // autoFocus: false,
          // scrollStrategy: scrollStrategy,
        });
        break;
    }

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
}