import { LightDefinition } from './../../contracts/equipment/viewlight-definition.model';
import { Property } from './../../contracts/equipment/view-property.model';
import { Type } from './../../contracts/equipment/view-type.model';
import { AddType } from './../../contracts/equipment/add-type.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class EquipmentService {

  constructor() { }


  // What it does
  //  returns the list of types
  //  returns the list of properties for a specific type
  //  returns definitions of a specific type

  getTypes(){
    // fake service
    let types: Type[] = [
      { id: '1', name: 'printer'},
      { id: '2', name: 'laptop'},
      { id: '3', name: 'scanner'},
    ]
    return types;
  }

  getPropertiesOfType(type: Type){
    // send type to API, it will return the list of properties

    // fake service
    let pcProperties: Property[] = [
      { id: '1', displayName: 'Model', description: 'The model', dataType: {id: '1', name: 'string'}},
      { id: '3', displayName: 'refreshRate', description: 'Monitors refresh rate', dataType: {id: '2', name: 'bool'}},
    ];

    let printerProperties: Property[] = [
      { id: '1', displayName: 'Model', description: 'The model', dataType: {id: '1', name: 'string'}},
      { id: '2', displayName: 'Color', description: 'Color or black and white?', dataType: {id: '2', name: 'bool'}},
    ];

    return (type.name == 'printer') ? printerProperties : pcProperties;
  }

  getDefinitionsOfType(type: Type){
    // send type to API, it will return the list of properties

    // fake service
    let printerDefinitions: LightDefinition[] = [
      {id: '1', name: 'HP LaserJet'},
      {id: '2', name: 'Lenovo M7000'}
    ];

    let pcDefinitions: LightDefinition[] = [
      {id: '3', name: 'Hantol'},
      {id: '4', name: 'HPC'},
      {id: '5', name: 'Sohoo'},
    ];

    if(type.name == 'printer')
      return printerDefinitions;

    if(type.name == 'pc')
      return pcDefinitions;
  }
}
