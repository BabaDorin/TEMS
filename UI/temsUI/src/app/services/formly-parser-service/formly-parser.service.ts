import { FormlyFieldConfig } from '@ngx-formly/core';
import { Injectable } from '@angular/core';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';

@Injectable({
  providedIn: 'root'
})
export class FormlyParserService {

  // documentation: https://formly.dev/guide/properties-options
  constructor() { }

  parseAddEquipment(addEquipment: AddEquipment, formlyFields?: FormlyFieldConfig[]){
    let formlyFieldsAddEquipment = 
          (formlyFields == undefined) ? [] as FormlyFieldConfig[] : formlyFields; 
    
    formlyFieldsAddEquipment.push({
      wrappers: ['formly-wrapper'],
      fieldGroup: [
        {
          key: 'identifier',
          type: 'input',
          defaultValue: addEquipment.definition.identifier,
          templateOptions: {
            label: 'Identifier',
          },
          expressionProperties: {
            'templateOptions.disabled': 'true',
          },
        },
        {
          key: 'temsid',
          type: 'input',
          defaultValue: addEquipment.temsid,
          templateOptions: {
            label: 'TEMSID',
          }
        },
        {
          key: 'serialNumber',
          type: 'input',
          defaultValue: addEquipment.serialNumber,
          templateOptions: {
            label: 'Serial Number',
          }
        },
      ]
    }
    );

    addEquipment.children.forEach(childAddEquipment => {
      this.parseAddEquipment(childAddEquipment, formlyFieldsAddEquipment[formlyFieldsAddEquipment.length-1].fieldGroup);
    });

    return formlyFieldsAddEquipment;
  }
}
