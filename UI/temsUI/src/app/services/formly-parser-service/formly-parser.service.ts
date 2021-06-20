import { DefinitionService } from 'src/app/services/definition-service/definition.service';
import { TEMSComponent } from './../../tems/tems.component';
import { SnackService } from './../snack/snack.service';
import { IOption } from 'src/app/models/option.model';
import { LogsService } from 'src/app/services/logs-service/logs.service';
import { Property } from 'src/app/models/equipment/view-property.model';
import { AddEquipment } from './../../models/equipment/add-equipment.model';
import { AddIssue } from '../../models/communication/issues/add-issue.model';
import { EquipmentService } from './../equipment-service/equipment.service';
import { Definition } from './../../models/equipment/add-definition.model';
import { FormlyField, FormlyFieldConfig } from '@ngx-formly/core';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { AddRoom } from 'src/app/models/room/add-room.model';
import { AddPersonnel } from 'src/app/models/personnel/add-personnel.model';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FormlyParserService extends TEMSComponent {

  // documentation: https://formly.dev/guide/properties-options
  // This service is used for parsing an object of type AddEquipment to a FormlyFieldCOnfig array.
  // Which is used by formly to render forms.

  constructor(
    private equipmentService: EquipmentService,
    private logsService: LogsService,
    private definitionService: DefinitionService
  ) {
    super();
  }

  parseAddRoom() {
    let fields: FormlyFieldConfig[] =
      [
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
      ];

    return fields;
  }

  parseSendEmail() {
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'from',
          type: 'input-tooltip',
          defaultValue: 'TEMS CIH Cahul',
          templateOptions: {
            required: true,
            label: 'Email sender',
          }
        },
        {
          key: 'subject',
          type: 'input',
          templateOptions: {
            required: true,
            label: 'Email subject',
          }
        },
        {
          key: 'text',
          type: 'textarea',
          templateOptions: {
            required: true,
            label: 'Email text',
            rows: 5,
          },
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
              defaultValue: statuses[0].value,
              templateOptions: {
                label: 'Priority',
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

  parseAddEquipment(addEquipment: AddEquipment, updating = false, formlyFields?: FormlyFieldConfig[]) {
    let formlyFieldsAddEquipment =
      (formlyFields == undefined) ? [] as FormlyFieldConfig[] : formlyFields;


    formlyFieldsAddEquipment.push(
      {
        wrappers: ['formly-wrapper'],
        fieldGroup: this.generateAddEquipmentFields(addEquipment)
      }
    )

    if (addEquipment.children == undefined || addEquipment.children.length == 0 || updating)
      return formlyFieldsAddEquipment;

    let index = 0;
    addEquipment.children.forEach(childAddEquipment => {
      formlyFieldsAddEquipment[formlyFieldsAddEquipment.length - 1].fieldGroup.push({
        key: childAddEquipment.definition.id + '---' + index++, // in reality - the index of child definition
        type: 'eq-repeat',
        wrappers: ['formly-wrapper'],
        fieldArray: {
          templateOptions: {
            btnText: childAddEquipment.definition.identifier,
          },
          fieldGroup: this.generateAddEquipmentFields(childAddEquipment)
        }
      })
    });

    return formlyFieldsAddEquipment;
  }

  generateAddEquipmentFields(addEquipment: AddEquipment) {
    return [
      {
        className: 'section-label',
        template: '<h3 class="alert alert-primary">' + addEquipment.definition.equipmentType.label + '</h3>'
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
        type: 'input-tooltip',
        templateOptions: {
          label: 'TEMSID',
          description: "The unique identifier you've given to the item, like LPB002"
        }
      },
      {
        key: 'serialNumber',
        type: 'input-tooltip',
        templateOptions: {
          label: 'Serial Number',
          description: "The serial number assigned to the item by it's manufacturer"
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
            templateOptions: {
              type: 'date',
              label: 'Purchase Date',
            },
          }
        ]
      }
    ]
  }

  parseAddProperty() {
    let fields: FormlyFieldConfig[] =
      [

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
      ];

    return fields;
  }

  parseAddKey() {
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

  parseAddAnnouncement() {
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
          key: 'identifier',
          type: 'input-tooltip',
          templateOptions: {
            description: 'Name associated with this definition',
            required: true,
            label: 'Identifier'
          }
        },
      ];

    // Add inputs for parent's properties
    addDefinition.properties.forEach(property => {
      fields.push(this.generatePropertyFieldGroup(property))
    });

    fields.push(
      {
        key: 'description',
        type: 'textarea',
        templateOptions: {
          label: 'Description',
        }
      },
      this.generatePriceFields(),
    );

    if (addDefinition.children.length == 0)
      return fields;

    // Adding children with 'repeat' type
    let tempKey = 0;
    addDefinition.children.forEach(childDefinition => {
      fields.push(
        {
          template: '<h4>' + childDefinition.equipmentType.label + ' definitions</h4>',
        },
        {
          key: childDefinition.equipmentType.value,
          type: 'repeat',
          wrappers: ['formly-wrapper'],
          fieldArray: {
            templateOptions: {
              btnText: childDefinition.equipmentType.label,
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
                key: 'identifierSelect',
                type: 'select',
                templateOptions: {
                  description: 'Choose an existing definition',
                  label: 'Choose existing one',
                  options: this.definitionService.getDefinitionsOfType(childDefinition.equipmentType.value).pipe(tap(defs => {
                    defs.unshift({value: "new", label: "new"});
                  })),
                  change: (field, $event)=>{ 
                    this.setChildDefinition(field.parent, $event.value);
                },
                },
              }
            ]
          }
        }
      );

      let destination = fields[fields.length - 1].fieldArray.fieldGroup;

      childDefinition.properties.forEach(property => {
        destination.push(this.generatePropertyFieldGroup(property, property.value));
      });

      // price and descriptions => mandatory properties
      destination.push(
        {
          key: 'description',
          type: 'textarea',
          templateOptions: {
            label: 'Description',
          }
        },
        this.generatePriceFields(childDefinition.price, childDefinition.currency),
      );
    });
    return fields;
  }

  resetDefinitionProperties(childFieldGroup: FormlyFieldConfig){
    childFieldGroup.fieldGroup.forEach(e => {
      if(e.key != "identifierSelect"){
        e.defaultValue = null;
      }
    })

    childFieldGroup.fieldGroup.find(q => q.key == "identifier").defaultValue = "";
    childFieldGroup.fieldGroup[childFieldGroup.fieldGroup.length-1].fieldGroup[0].defaultValue = "0";
    childFieldGroup.fieldGroup[childFieldGroup.fieldGroup.length-1].fieldGroup[1].defaultValue = "lei";
  }

  setChildDefinition(childFieldGroup: FormlyFieldConfig, definitionId){
    let priceField = childFieldGroup.fieldGroup[childFieldGroup.fieldGroup.length-1].fieldGroup[0]; 
    let currencyField = childFieldGroup.fieldGroup[childFieldGroup.fieldGroup.length-1].fieldGroup[1];

    this.resetDefinitionProperties(childFieldGroup);

    if(definitionId == "new"){
      childFieldGroup.fieldGroup.forEach(e => {
        if(e.key != "identifierSelect"){
          e.templateOptions.disabled = false
        }
      })

      priceField.templateOptions.disabled = false;
      currencyField.templateOptions.disabled = false;
      return;
    }

    this.subscriptions.push(
      this.equipmentService.getFullDefinition(definitionId)
      .subscribe(result => {
        
        childFieldGroup.fieldGroup.forEach(e => {
          if(e.key != "identifierSelect")
            e.templateOptions.disabled = true
        })
        
        let definition: Definition = result;
        definition.properties.forEach(q => {
          console.log('property:');
          console.log(q);

          let propertyField = childFieldGroup.fieldGroup.find(e => e.key == q.name); 
          propertyField.defaultValue = q.value;
        });

        priceField.defaultValue = definition.price;
        priceField.templateOptions.disabled = true;

        currencyField.defaultValue = definition.currency;
        currencyField.templateOptions.disabled = true;
      })
    )
  }

  generatePriceFields(defaultPrice?: number, defaultCurrency?: string) {
    return {
      fieldGroupClassName: 'row',
      fieldGroup: [
        {
          className: 'col-4',
          key: 'price',
          defaultValue: defaultPrice ?? 0,
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
          defaultValue: defaultCurrency ?? 'lei',
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

  generatePropertyFieldGroup(addProperty: Property, value?): FormlyFieldConfig {
    // When the formly form is used for updating,
    let propertyFieldGroup: FormlyFieldConfig;
  
    if(value != undefined)
      propertyFieldGroup.defaultValue = value;

    switch (addProperty.dataType.toLowerCase()) {
      case 'text':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'input-tooltip',
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

    return propertyFieldGroup;
  }

  parseAddPersonnel(addPersonnel: AddPersonnel) {
    let fields: FormlyFieldConfig[] =
      [
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
      ];

    return fields;
  }

  parseLogin() {
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'login',
          fieldGroup: [
            {
              key: 'username',
              className: "my-2",
              type: 'input',
              templateOptions: {
                required: true,
                label: 'Username',
              }
            },
            {
              key: 'password',
              className: "my-2",
              type: 'input',
              templateOptions: {
                required: true,
                type: 'password',
                label: 'Password',
              },
            },
          ]
        }
      ];

    return fields;
  }

  parseAddUser(update?: boolean) {
    if (update == undefined) update = false;
    let fields: FormlyFieldConfig[] =
      [
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
            required: update ? false : true,
            label: update ? 'Set new password' : 'Password',
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
      ];

    return fields;
  }

  parseChangePassword() {
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'oldPass',
          type: 'input',
          templateOptions: {
            type: "password",
            required: true,
            label: 'Old password',
          }
        },
        {
          validators: {
            validation: [
              { name: 'fieldMatch', options: { errorPath: 'confirmNewPass' } },
            ],
          },
          fieldGroup: [

            {
              key: 'newPass',
              type: 'input',
              templateOptions: {
                type: "password",
                required: true,
                label: 'New password',
              }
            },
            {
              key: 'confirmNewPass',
              type: 'input',
              templateOptions: {
                type: "password",
                required: true,
                label: 'Confirm new password',
              }
            },
          ]
        }];

    return fields;
  }

  parseChangeEmailPreferences() {
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'email',
          type: 'input-tooltip',
          templateOptions: {
            type: "email",
            description: 'The email address is primarily used for account recovering. If you desire, you can recieve notifications too.',
            label: 'Email address',
          }
        },
        {
          key: 'getNotifications',
          type: 'checkbox',
          templateOptions: {
            label: 'Get notifications via Email',
          }
        },
      ];

    return fields;
  }

  parseAccountGeneralInfo() {
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'fullName',
          type: 'input',
          templateOptions: {
            required: true,
            label: 'Full Name',
          }
        },
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
      ];

    return fields;
  }
}
