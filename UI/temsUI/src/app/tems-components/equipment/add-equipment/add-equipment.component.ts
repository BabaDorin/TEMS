import { LightDefinition } from './../../../contracts/equipment/viewlight-definition.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Type } from './../../../contracts/equipment/view-type.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-add-equipment',
  templateUrl: './add-equipment.component.html',
  styleUrls: ['./add-equipment.component.scss'],
  providers: [ EquipmentService ]
})
export class AddEquipmentComponent implements OnInit {
  types: Type[];
  definitionsOfType: LightDefinition[];

  selectedType: Type = {id: '', name: ''};
  selectedDefinition: LightDefinition = {id: '', name: ''};
  // selectedDefinition: 


  private typeSelected: boolean = false;
  private definitionSelected: boolean = false;
  private readyToBeSaved: boolean = false;
  private chosenEquipmentType: string = "Choose an equipment type"
  private chosenEquipmentDefinition: string = "Choose a " + this.selectedType.name + " definition";

  constructor(private equipmentService: EquipmentService) { }

  ngOnInit(): void {
    this.types = this.equipmentService.getTypes();
  }

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

  definitionHasBeenSelected(definition: LightDefinition){
    // here we check if the selected type is valid and user hasn't done any manipulation
    this.selectedDefinition = definition;
    if(this.definitionsOfType.find( def => def === this.selectedDefinition ) != undefined){
      this.definitionSelected = true;
      this.chosenEquipmentDefinition = this.selectedDefinition.name;
    } 
    else{
      this.definitionSelected = false;
      this.chosenEquipmentDefinition = "Choose a " + this.selectedType.name + " definition";
    }
  }
}
