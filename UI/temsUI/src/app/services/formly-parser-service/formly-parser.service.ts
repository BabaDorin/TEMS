import { IOption } from 'src/app/models/option.model';
import { LogsService } from 'src/app/services/logs-service/logs.service';
import { Property } from 'src/app/models/equipment/view-property.model';
import { AddEquipment } from './../../models/equipment/add-equipment.model';
import { AddIssue } from '../../models/communication/issues/add-issue.model';
import { EquipmentService } from './../equipment-service/equipment.service';
import { Definition } from './../../models/equipment/add-definition.model';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { AddRoom } from 'src/app/models/room/add-room.model';
import { AddPersonnel } from 'src/app/models/personnel/add-personnel.model';

@Injectable({
  providedIn: 'root'
})
export class FormlyParserService {

  // documentation: https://formly.dev/guide/properties-options
  // This service is used for parsing an object of type AddEquipment to a FormlyFieldCOnfig array.
  // Which is used by formly to render forms.

  constructor(
    private equipmentService: EquipmentService,
    private logsService: LogsService) { }

  parseAddRoom(){
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'room',
          fieldGroup: [
            {
              key: 'identifier',
              type: 'input',
              templateOptions: {
                required: true,
                label: 'Room Identifier',
                placeholder: '214'
              }
            },
            {
              key: 'floor',
              type: 'input',
              templateOptions: {
                type: 'number',
                label: 'Floor',
                placeholder: '1',
                min: 1,
              }
            },
            {
              key: 'description',
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

  parseAddIssue(addIssue: AddIssue, frequentProblems: string[], statuses: IOption[]) {
    if (addIssue == undefined)
      addIssue = new AddIssue();

    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'issue',
          fieldGroup: [
            {
              key: 'problem',
              type: 'autocomplete',
              templateOptions: {
                required: true,
                label: 'What is the problem?',
                placeholder: 'Incarcare cartus...',
                filter: (term) => of(term ? this.filterAutocomplete(term, frequentProblems) : frequentProblems.slice()),
              },
            },
            {
              key: 'problemDescription',
              type: 'textarea',
              templateOptions: {
                label: 'Problem description - Helps a lot!',
              },
            },
            {
              key: 'status',
              type: 'radio',
              templateOptions: {
                label: 'Radio',
                required: true,
                options: statuses
              },
            }
          ]
        }
      ];

    return fields;
  }

  private filterAutocomplete(name: string, autocomplete: string[]) {
    return autocomplete.filter(criteria =>
      criteria.toLowerCase().indexOf(name.toLowerCase()) === 0);
  }

  parseAddEquipment(addEquipment: AddEquipment, formlyFields?: FormlyFieldConfig[]) {
    let formlyFieldsAddEquipment =
      (formlyFields == undefined) ? [] as FormlyFieldConfig[] : formlyFields;


    formlyFieldsAddEquipment.push(
      {
        wrappers: ['formly-wrapper'],
        fieldGroup: this.generateAddEquipmentFields(addEquipment)
      }
    )

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
          fieldGroup: this.generateAddEquipmentFields(childAddEquipment)
        }
      })
    });

    return formlyFieldsAddEquipment;
  }

  generateAddEquipmentFields(addEquipment: AddEquipment){
    return [
      {
        className: 'section-label',
        template: '<h5><pre>' + addEquipment.definition.equipmentType.value + '</pre></h5>'
      },
      {
        key: 'identifier',
        type: 'input',
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
        templateOptions: {
          label: 'TEMSID',
        }
      },
      {
        key: 'serialNumber',
        type: 'input',
        templateOptions: {
          label: 'Serial Number',              
        }
      },
      {
        key: 'isDefect',
        type: 'checkbox',
        defaultValue: false,
        templateOptions: {
          label: 'Is Defect',
        }
      },
      {
        key: 'isUsed',
        type: 'checkbox',
        defaultValue: true,
        templateOptions: {
          label: 'Is Used',
        }
      },
      {
        key: 'description',
        type: 'textarea',
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
            defaultValue: addEquipment.definition.price,
            type: 'input',
            templateOptions: {
              label: 'Price',
            },
          },
          {
            className: 'col-4',
            key: 'currency',
            type: 'select',
            defaultValue: addEquipment.definition.currency,
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
            defaultValue: new Date(),
            templateOptions: {
              type: 'date',
              label: 'Purchase Date',
            },
          }
        ]
      }
    ]
  }

  parseAddProperty(){
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'addProperty',
          fieldGroup: [
            {
              key: 'name',
              type: 'input-tooltip',
              templateOptions: {
                label: 'Name',
                placeholder: 'model',
                description: 'The name that will be used by the system for building objects. No spaces or other special charaters allowed!',
                required: true
              },
              validators: {
                validation: ['specCharValidator']
              }
            },
            {
              key: 'displayName',
              type: 'input-tooltip',
              templateOptions: {
                label: 'Display Name',
                placeholder: 'Model',
                required: true,
                description: 'The name that will be displayed'
              },
            },
            {
              key: 'description',
              type: 'input-tooltip',
              templateOptions: {
                label: 'Property description',
                description: 'Description of this property will appear like this',
              },
            },
            
            {
              key: 'dataType',
              type: 'select',
              templateOptions: {
                required: true,
                label: 'DataType',
                options: [
                  { value: 'text', label: 'Text' },
                  { value: 'number', label: 'Number' },
                  { value: 'bool', label: 'Boolean' }, // Other will appear soon
                ]
              }
            },
            {
              key: 'required',
              type: 'checkbox',
              defaultValue: false,
              templateOptions: {
                label: 'Required',
              },
            },
          ]
        }
      ];

    return fields;
  }

  parseAddKey(roomOptions: IOption[]){
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'key',
          fieldGroup: [
            {
              key: 'identifier',
              type: 'input-tooltip',
              templateOptions: {
                required: true,
                label: 'Indetifier',
                description: 'A short name for this key, like "214"'
              }
            },
            {
              key: 'numberOfCopies',
              type: 'input-tooltip',
              templateOptions: {
                type: 'number',
                min: 0,
                label: 'Number of copies',
                description: "Number of available options"
              },
            },
            {
              key: 'roomId',
              type: 'select',
              templateOptions: {
                required: true,
                label: 'Room',
                options: roomOptions
              }
            },
            {
              key: 'description',
              type: 'input',
              templateOptions: {
                label: 'Description',
              }
            },
          ]
        }
      ];

    return fields;
  }

  parseAddAnnouncement(){
    let fields: FormlyFieldConfig[] =
    [
      {
        key: 'announcement',
        fieldGroup: [
          {
            key: 'title',
            type: 'input',
            templateOptions: {
              required: true,
              label: 'Title',
            }
          },
          {
            key: 'text',
            type: 'textarea',
            templateOptions: {
              rows: 3,
              required: true,
              label: 'Message',
            },
          },
        ]
      }
    ];

    return fields;
  }

  parseAddDefinition(addDefinition: Definition, formlyFields?: FormlyFieldConfig[]) {

    let fields: FormlyFieldConfig[] =
      [
        {
          template: '<h2> Add new ' + addDefinition.equipmentType.label + ' definition</h2><hr><br>'
        },
        {
          key: 'addDefinition',
          wrappers: ['formly-wrapper'],
          fieldGroup: [
            {
              key: 'identifier',
              type: 'input-tooltip',
              templateOptions: {
                description: 'Name associated with this definition',
                required: true,
                label: 'Identifier'
              }
            },
          ]
        }
      ];
    console.log('1');
    // Adding inputs for parent's properties
    addDefinition.properties.forEach(property => {
      fields[fields.length - 1].fieldGroup.push(this.generatePropertyFieldGroup(property))
    });
    console.log('back');

    console.log('price and description');
    fields[fields.length-1].fieldGroup.push(
      {
        key: 'description',
        type: 'textarea',
        templateOptions: {
          label: 'Description',
        }
      },
      this.generatePriceFields(),
    );

    console.log('children');
    if (addDefinition.children.length == 0)
      return fields;

    console.log('continue');
    
    // Adding children with 'repeat' type
    let tempKey = 0;
    addDefinition.children.forEach(childDefinition => {

      fields[fields.length - 1].fieldGroup.push(
        {
          template: '<h4>' + childDefinition.equipmentType.value + ' definitions</h4>',
        },
        {
          key: ''+tempKey++, // in realilty - this will be the child definition ID
          type: 'repeat',
          wrappers: ['formly-wrapper'],
          fieldArray: {
            templateOptions: {
              btnText: '+ ' + childDefinition.equipmentType.value,
            },
            fieldGroup: [
              {
                className: 'col-6',
                key: 'identifier',
                type: 'input-tooltip',
                templateOptions: {
                  description: 'Name associated with this definition (If the desired definition already exists, select it from dropdown)',
                  required: true,
                  label: 'Identifier'
                }
              },
              {
                className: 'col-6',
                key: 'identifier',
                type: 'select',
                templateOptions: {
                  description: 'Choose an existing definition',
                  label: 'Choose existing one',
                  clickEvent: console.log('hello'),
                }
              },
            ]
          }
        }
      )

      let lastFieldGroup = fields[fields.length - 1].fieldGroup;
      let destination = lastFieldGroup[lastFieldGroup.length - 1].fieldArray.fieldGroup;

      childDefinition.properties.forEach(property => {
        destination.push(this.generatePropertyFieldGroup(property));
      });
      destination.push(
        {
          key: 'description',
          type: 'textarea',
          templateOptions: {
            label: 'Description',
          }
        },
        this.generatePriceFields(),
      );
    });
    return fields;
  }

  generatePriceFields(){
    return {
      fieldGroupClassName: 'row',
      fieldGroup: [
        {
          className: 'col-4',
          key: 'price',
          defaultValue: 0,
          type: 'input-tooltip',
          templateOptions: {
            type: 'number',
            min: 0,
            description: 'The price can be overwritten equipments having this definition',
            label: 'Price',
          },
        },
        {
          className: 'col-4',
          key: 'currency',
          type: 'select',
          defaultValue: 'lei',
          templateOptions: {
            label: 'Currency',
            options: [
              { label: 'LEI', value: 'lei' },
              { label: 'EUR', value: 'eur' },
              { label: 'USD', value: 'usd' },
            ],
          },
        },
      ]
    }
  }

  parseAddLog(logTypes: IOption[]) {
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'log',
          fieldGroup: [
            {
              key: 'logTypeId',
              type: 'select',
              templateOptions: {
                required: true,
                label: 'Log Type',
                options: logTypes
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
            },
            {
              className: 'mb-0',
              key: 'isImportant',
              type: 'checkbox',
              defaultValue: false,
              templateOptions: {
                label: 'Is important',
                description: 'Unimportant logs will get deleted after a certain period of time',
              },
            }
          ]
        }
      ];

    return fields;
  }

  generatePropertyFieldGroup(addProperty: Property): FormlyFieldConfig {
    let propertyFieldGroup: FormlyFieldConfig;
    console.log('generatepropertyfieldgrop');

    switch (addProperty.dataType.name.toLowerCase()) {
      case 'text':
        console.log('add ' + addProperty.name);
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'input-tooltip',
          defaultValue: "default value",
          templateOptions: {
            description: addProperty.description,
            label: addProperty.displayName,
            required: addProperty.required,
          },
        }
        break;

      case 'number':
        console.log('add ' + addProperty.name);

        propertyFieldGroup = {
          key: addProperty.name,
          type: 'input-tooltip',
          defaultValue: "default value",
          templateOptions: {
            type: 'number',
            description: addProperty.description,
            label: addProperty.displayName,
            required: addProperty.required,
            min: (addProperty.min == 0 && addProperty.max == 0) ? undefined : addProperty.min,
            max: (addProperty.min == 0 && addProperty.max == 0) ? undefined : addProperty.max
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

      case 'bool':
        console.log('add ' + addProperty.name);
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

    console.log('return ' + addProperty.name);
    return propertyFieldGroup;

  }

  parseAddPersonnel(addPersonnel: AddPersonnel){
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'personnel',
          fieldGroup: [
            {
              key: 'name',
              type: 'input',
              templateOptions: {
                required: true,
                label: 'Name',
              }
            },
            {
              key: 'phoneNumber',
              type: 'input-tooltip',
              templateOptions: {
                label: 'Phone Number (Without the leading 0)',
                description: 'It will be used to send SMS!'
              },
            },
            {
              key: 'email',
              type: 'input-tooltip',
              templateOptions: {
                type: 'email',
                label: 'Email',
                description: 'It will be used to send mails!'
              },
            },
          ]
        }
      ];

    return fields;
  }

  parseAddUser(){
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'addUser',
          fieldGroup: [
            {
              key: 'username',
              type: 'input-tooltip',
              templateOptions: {
                minLength: 4,
                required: true,
                label: 'Username',
                description: 'Minimum 4 alphanumerics or ".", "_"'
              },
              validators: {
                validation: ['usernameValidator']
              }
            },
            {
              key: 'password',
              type: 'input-tooltip',
              templateOptions: {
                type: "password",
                minLength: 5,
                required: true,
                label: 'Password',
                description: "At least 5 characters long"
              },
            },
            {
              key: 'fullName',
              type: 'input',
              templateOptions: {
                label: 'Full Name',
              },
            },
            {
              key: 'phoneNumber',
              type: 'input-tooltip',
              templateOptions: {
                label: 'Phone Number (Without the leading 0)',
                description: 'It will be used to send SMS!'
              },
            },
            {
              key: 'email',
              type: 'input-tooltip',
              templateOptions: {
                type: 'email',
                label: 'Email',
                description: 'It will be used to send mails!'
              },
            },
            {
              key: 'roles',
            },
            {
              key: 'personnel',
            },
          ]
        }
      ];

    return fields;
  }
}
