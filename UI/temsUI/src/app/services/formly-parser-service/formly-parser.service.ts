import { AddProperty } from './../../models/equipment/add-property.model';
import { EquipmentService } from './../equipment-service/equipment.service';
import { AddDefinition } from './../../models/equipment/add-definition.model';
import { AddType } from './../../models/equipment/add-type.model';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { Injectable } from '@angular/core';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { Observable } from 'rxjs';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';

@Injectable({
  providedIn: 'root'
})
export class FormlyParserService {

  // documentation: https://formly.dev/guide/properties-options
  // This service is used for parsing an object of type AddEquipment to a FormlyFieldCOnfig array.
  // Which is used by formly to render forms.

  constructor(private equipmentService: EquipmentService) { }

  parseAddEquipment(addEquipment: AddEquipment, formlyFields?: FormlyFieldConfig[]) {
    let formlyFieldsAddEquipment =
      (formlyFields == undefined) ? [] as FormlyFieldConfig[] : formlyFields;


    formlyFieldsAddEquipment.push(
      {
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
    )


    // key: 'conditions',
    //   type: 'condition-repeat',
    //   hideExpression: (model, formState, field) => field.parent.model.type !== 'LOGICAL',
    //   templateOptions: {
    //     label: 'Conditions',
    //   },
    //   fieldArray: {
    //     fieldGroup: [],
    //   },

    if (addEquipment.children == undefined || addEquipment.children.length == 0)
      return formlyFieldsAddEquipment;

    let index = 0;
    addEquipment.children.forEach(childAddEquipment => {
      formlyFieldsAddEquipment[formlyFieldsAddEquipment.length - 1].fieldGroup.push({
        key: '' + index++, // in reality - the index of child definition
        type: 'eq-repeat',
        wrappers: ['formly-wrapper'],
        fieldArray: {
          templateOptions: {
            btnText: '+ ' + childAddEquipment.definition.identifier,
          },
          fieldGroup: [
            {
              className: 'section-label',
              template: '<h5><pre>' + childAddEquipment.definition.equipmentType.name + '</pre></h5>'
            },
            {
              key: 'identifier',
              type: 'input',
              defaultValue: childAddEquipment.definition.identifier,
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
              defaultValue: childAddEquipment.temsid,
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
              defaultValue: childAddEquipment.isDefect,
              templateOptions: {
                label: 'Is Defect',
              }
            },
            {
              key: 'isUsed',
              type: 'checkbox',
              defaultValue: childAddEquipment.isUsed,
              templateOptions: {
                label: 'Is Used',
              }
            },
            {
              key: 'description',
              type: 'textarea',
              defaultValue: childAddEquipment.description,
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
      })
    });

    // Children data will be inserted into the last fieldGroup of parent.
    // The result will be smth like this
    // parent
    //  |serial Number
    //  |temsid
    //  | |Child
    //  | |child Serial Number
    //  ....

    // addEquipment.children.forEach(childAddEquipment => {
    //   this.parseAddEquipment(childAddEquipment, formlyFieldsAddEquipment[formlyFieldsAddEquipment.length - 1].fieldGroup);
    // });

    console.log(formlyFieldsAddEquipment);
    return formlyFieldsAddEquipment;
  }

  parseAddType(addType: AddType) {
    let parents = [];
    addType.parents.forEach(parent => {

      parents.push({
        value: parent.id,
        label: parent.name
      })
    });

    let temsProperties = this.equipmentService.getProperties();
    let properties = [];

    temsProperties.forEach(property => {
      properties.push({
        value: property.id,
        label: property.name
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

  parseAddDefinition(addDefinition: AddDefinition, formlyFields?: FormlyFieldConfig[]) {

    // initilizing fields
    let fields: FormlyFieldConfig[] =
      [
        {
          template: '<h3>' + addDefinition.equipmentType.name + ' definition</h3>'
        },
        {
          key: 'customer',
          wrappers: ['formly-wrapper'],
          fieldGroup: [
            {
              key: 'identifier',
              type: 'input',
              templateOptions: {
                description: 'Name associated with this definition',
                required: true,
                label: 'Identifier'
              }
            },
          ]
        }
      ];

    // Adding inputs for parent's properties
    addDefinition.properties.forEach(property => {
      fields[fields.length - 1].fieldGroup.push(this.generatePropertyFieldGroup(property))
    });

    if (addDefinition.children.length == 0)
      return fields;

    // Adding children with 'repeat' type
    let tempKey = 0;
    addDefinition.children.forEach(childDefinition => {

      fields[fields.length - 1].fieldGroup.push(
        {
          template: '<h4>' + childDefinition.equipmentType.name + ' definitions</h4>',
        },
        {
          key: ' ' + tempKey++, // in realilty - this will be the child definition ID
          type: 'repeat',
          wrappers: ['formly-wrapper'],
          fieldArray: {
            templateOptions: {
              btnText: '+ ' + childDefinition.equipmentType.name,
            },
            fieldGroup: []
          }
        }
      )

      let lastFieldGroup = fields[fields.length - 1].fieldGroup;
      let destination = lastFieldGroup[lastFieldGroup.length - 1].fieldArray.fieldGroup;

      childDefinition.properties.forEach(property => {
        destination.push(this.generatePropertyFieldGroup(property))
      });
    });

    return fields;
  }

  parseAddLog(addLog: ViewLog) {
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'log',
          fieldGroup: [
            {
              key: 'logType',
              type: 'select',
              templateOptions: {
                required: true,
                label: 'Log Type',
                options: [
                  { value: '1', label: 'Simple' },
                  { value: '2', label: 'Repair' },
                ]
              }
            },
            {
              key: 'text',
              type: 'textarea',
              templateOptions: {
                label: 'Description',
                placeholder: '...',
                rows: 5,
              },
            }
          ]
        }
      ];

    return fields;
  }


  generatePropertyFieldGroup(addProperty: AddProperty): FormlyFieldConfig {
    let propertyFieldGroup: FormlyFieldConfig;

    switch (addProperty.dataType.name.toLowerCase()) {
      case 'string':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'input-tooltip',
          defaultValue: "default value",
          templateOptions: {
            description: 'cf',
            label: addProperty.displayName,
            required: addProperty.required,
          },
        }
        break;

      case 'number':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'input-tooltip',
          defaultValue: "default value",
          templateOptions: {
            type: 'number',
            description: 'FrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequencyFrequency',
            label: addProperty.displayName,
            required: addProperty.required,
            min: addProperty.min,
            max: addProperty.max,
          },
        }
        break;

      case 'select':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'select',
          templateOptions: {
            label: addProperty.displayName,
            required: addProperty.required,
            options: addProperty.options
          },
        }
        break;

      case 'multicheckbox':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'multiCheckbox',
          templateOptions: {
            label: addProperty.displayName,
            required: addProperty.required,
            options: addProperty.options
          },
        }
        break;

      case 'checkbox':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'checkbox',
          templateOptions: {
            label: addProperty.displayName,
            required: addProperty.required,
          },
        }
        break;

      case 'radiobutton':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'radio',
          templateOptions: {
            label: addProperty.displayName,
            required: addProperty.required,
            options: addProperty.options
          },
        }
        break;
    }

    return propertyFieldGroup;

  }
}
