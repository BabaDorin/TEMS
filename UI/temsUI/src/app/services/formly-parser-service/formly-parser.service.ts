import { AddType } from './../../models/equipment/add-type.model';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { Injectable } from '@angular/core';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';

@Injectable({
  providedIn: 'root'
})
export class FormlyParserService {

  // documentation: https://formly.dev/guide/properties-options
  // This service is used for parsing an object of type AddEquipment to a FormlyFieldCOnfig array.
  // Which is used by formly to render forms.

  constructor() { }

  parseAddEquipment(addEquipment: AddEquipment, formlyFields?: FormlyFieldConfig[]) {
    let formlyFieldsAddEquipment =
      (formlyFields == undefined) ? [] as FormlyFieldConfig[] : formlyFields;

    formlyFieldsAddEquipment.push({
      wrappers: ['formly-wrapper'],
      fieldGroup: [
        {
          className: 'section-label',
          template: '<h5><pre>' + addEquipment.definition.equipmentType.name + '</pre></h5>'
        },
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
          // defaultValue: addEquipment.serialNumber,
          templateOptions: {
            label: 'Serial Number',
          }
        },
        {
          key: 'isDefect',
          type: 'checkbox',
          defaultValue: addEquipment.isDefect,
          templateOptions: {
            label: 'Is Defect',
          }
        },
        {
          key: 'isUsed',
          type: 'checkbox',
          defaultValue: addEquipment.isUsed,
          templateOptions: {
            label: 'Is Used',
          }
        },
        {
          key: 'description',
          type: 'textarea',
          defaultValue: addEquipment.description,
          templateOptions: {
            label: 'Description',
          }
        },
        {
          fieldGroupClassName: 'row',
          fieldGroup: [
            {
              className: 'col-4',
              key: 'price',
              type: 'input',
              templateOptions: {
                label: 'Price',
              },
            },
            {
              className: 'col-4',
              key: 'currency',
              type: 'select',
              templateOptions: {
                label: 'Currency',
                options: [
                  { label: 'LEI', value: 'lei' },
                  { label: 'EUR', value: 'eur' },
                  { label: 'USD', value: 'usd' },
                ],
              },
            },

            {
              className: 'col-4',
              type: 'input',
              key: 'purchaseDate',
              templateOptions: {
                type: 'date',
                label: 'Purchase Date',
              },
            }
          ]
        }
      ]
    }
    );

    // Children data will be inserted into the last fieldGroup of parent.
    // The result will be smth like this
    // parent
    //  |serial Number
    //  |temsid
    //  | |Child
    //  | |child Serial Number
    //  ....

    addEquipment.children.forEach(childAddEquipment => {
      this.parseAddEquipment(childAddEquipment, formlyFieldsAddEquipment[formlyFieldsAddEquipment.length - 1].fieldGroup);
    });

    return formlyFieldsAddEquipment;
  }

  parseAddType(addType: AddType){
    let parents = [];
    addType.parents.forEach(parent => {
      
      parents.push({
        value: parent.id,
        label: parent.name
      })
    });


    let formlyFieldsAddType: FormlyFieldConfig[] = [
      {
        template: '<h4>Add Type</h4>'
      },
      {
        key: 'parents',
        type: 'multicheckbox',
        templateOptions: {
          options: parents,
          label: "Select Type's parents"
        },
      },
      {
        key: 'name',
        type: 'input',
        defaultValue: addType.name,
        templateOptions: {
          label: 'Name',
        },
      },
    ];

    return formlyFieldsAddType;
  }
}
