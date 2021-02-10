import { AddTypeComponent } from '.././add-type/add-type.component';
import { AddType } from './../../../models/equipment/add-type.model';
import { FormlyParserService } from './../../../services/formly-parser-service/formly-parser.service';
import { AddDefinition } from '../../../models/equipment/add-definition.model';
import { LightDefinition } from '../../../models/equipment/viewlight-definition.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Type } from '../../../models/equipment/view-type.model';
import { Component, OnInit, Inject} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-add-equipment',
  templateUrl: './add-equipment.component.html',
  styleUrls: ['./add-equipment.component.scss'],
  providers: []
})
export class AddEquipmentComponent implements OnInit {
  // general methods
  constructor(
    private equipmentService: EquipmentService,
    private formlyParserService: FormlyParserService,
    public dialog: MatDialog) { }

  ngOnInit(): void {
    this.types = this.equipmentService.getTypes();
  }

  // type related ------------------------------------------
  types: Type[];
  selectedType: Type = { id: '', name: '' };
  private typeSelected: boolean = false;
  private chosenEquipmentType: string = "Choose an equipment type"

  typeHasBeenSelected(type: Type) {
    this.selectedType = type;

    // here we check if the selected type is valid and user hasn't done any manipulation
    if (this.types.find(type => type === this.selectedType) != undefined) {
      this.typeSelected = true;
      this.chosenEquipmentType = this.selectedType.name;
      this.definitionsOfType = this.equipmentService.getDefinitionsOfType(this.selectedType);
      console.log('type found');
    }
    else {
      this.typeSelected = false;
      this.definitionsOfType = undefined;
      this.chosenEquipmentType = "Choose an equipment type";
    }
  }

  // definition related ------------------------------------------
  definitionsOfType: LightDefinition[];
  selectedDefinition: LightDefinition = { id: '', name: '' };
  selectedFullDefinition: AddDefinition = undefined;
  private definitionSelected: boolean = false;
  private chosenEquipmentDefinition: string = "Choose a " + this.selectedType.name + " definition";

  
  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }


  definitionHasBeenSelected(definition: LightDefinition) {
    // here we check if the selected definition is valid and user hasn't done any manipulation
    this.selectedDefinition = definition;
    if (this.definitionsOfType.find(def => def === this.selectedDefinition) != undefined) {
      this.chosenEquipmentDefinition = this.selectedDefinition.name;

      // fetch all the data about the selected definition
      this.selectedFullDefinition = this.equipmentService.getFullDefinition(this.selectedDefinition.id);

      this.definitionSelected = true;

      let addEq = this.equipmentService.generateAddEquipmentOfDefinition(this.selectedFullDefinition);
      let formlyFields = this.formlyParserService.parseAddEquipment(addEq);

      this.formlyData.model = new AddEquipment(this.selectedFullDefinition);
      this.formlyData.fields = formlyFields;
      this.formlyData.isVisible = true;
    }
    else {
      this.chosenEquipmentDefinition = "Choose a " + this.selectedType.name + " definition";
      this.definitionSelected = false;
    }
  }


  onSubmit(model) {
    model.id = this.selectedFullDefinition.id,
    console.log(model);
  }


  openDialog(): void {
    const dialogRef = this.dialog.open(AddTypeComponent,{
      width: '250px',
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  private readyToBeSaved: boolean = false;
}