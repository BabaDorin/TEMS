import { AddDefinition } from './../../../contracts/equipment/add-definition.model';
import { LightDefinition } from './../../../contracts/equipment/viewlight-definition.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Type } from './../../../contracts/equipment/view-type.model';
import { Component, OnInit } from '@angular/core';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

@Component({
  selector: 'app-add-equipment',
  templateUrl: './add-equipment.component.html',
  styleUrls: ['./add-equipment.component.scss'],
  providers: [ EquipmentService ]
})
export class AddEquipmentComponent implements OnInit {
  // general methods
  constructor(private equipmentService: EquipmentService) { }

  ngOnInit(): void {
    this.types = this.equipmentService.getTypes();
  }

  // type related ------------------------------------------
  types: Type[];
  selectedType: Type = {id: '', name: ''};
  private typeSelected: boolean = false;
  private chosenEquipmentType: string = "Choose an equipment type"

  typeHasBeenSelected(type: Type){
    this.selectedType = type;

    // here we check if the selected type is valid and user hasn't done any manipulation
    if(this.types.find( type => type === this.selectedType ) != undefined){
      this.typeSelected = true;
      this.chosenEquipmentType = this.selectedType.name;
      this.definitionsOfType = this.equipmentService.getDefinitionsOfType(this.selectedType);
      console.log('type found');
    } 
    else{
      this.typeSelected = false;
      this.definitionsOfType = undefined;
      this.chosenEquipmentType = "Choose an equipment type";
    }
  }

  // definition related ------------------------------------------
  definitionsOfType: LightDefinition[];
  selectedDefinition: LightDefinition = {id: '', name: ''};
  selectedFullDefinition: AddDefinition;
  private definitionSelected: boolean = false;
  private chosenEquipmentDefinition: string = "Choose a " + this.selectedType.name + " definition";

  definitionHasBeenSelected(definition: LightDefinition){
    // here we check if the selected definition is valid and user hasn't done any manipulation
    this.selectedDefinition = definition;
    if(this.definitionsOfType.find( def => def === this.selectedDefinition ) != undefined){
      this.chosenEquipmentDefinition = this.selectedDefinition.name;

      // fetch all the data about the selected definition
      this.selectedFullDefinition = this.equipmentService.getFullDefinition(this.selectedDefinition.id);

      this.definitionSelected = true;
    } 
    else{
      this.chosenEquipmentDefinition = "Choose a " + this.selectedType.name + " definition";
      this.definitionSelected = false;
    }
  }

  // general ------------------------------------------
  private readyToBeSaved: boolean = false;
}
