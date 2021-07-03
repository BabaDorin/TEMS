import { EquipmentService } from 'src/app/services/equipment.service';
import { DefinitionService } from 'src/app/services/definition.service';
import { TEMSComponent } from '../tems/tems.component';
import { IOption } from 'src/app/models/option.model';
import { Property } from 'src/app/models/equipment/view-property.model';
import { AddEquipment } from '../models/equipment/add-equipment.model';
import { AddIssue } from '../models/communication/issues/add-issue.model';
import { Definition } from '../models/equipment/add-definition.model';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { AddPersonnel } from 'src/app/models/personnel/add-personnel.model';
import { tap } from 'rxjs/operators';
import { LogsService } from './logs.service';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class FormlyParserService extends TEMSComponent {

  // documentation: https://formly.dev/guide/properties-options
  // This service is used for parsing an object of type AddEquipment to a FormlyFieldCOnfig array.
  // Which is used by formly to render forms.

  constructor(
    private translate: TranslateService,
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
            placeholder: '214'
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('room.identifier'),
          },
        },
        {
          key: 'floor',
          type: 'input',
          templateOptions: {
            type: 'number',
            placeholder: '1',
            min: 1,
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('room.floor'),
          },
        },
        {
          key: 'description',
          type: 'textarea',
          templateOptions: {
            placeholder: '...',
            rows: 5,
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.description'),
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
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('email.sender'),
          },
        },
        {
          key: 'subject',
          type: 'input',
          templateOptions: {
            required: true,
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('email.subject'),
          },
        },
        {
          key: 'text',
          type: 'textarea',
          templateOptions: {
            required: true,
            rows: 5,
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('email.text'),
          },
        }
      ];

    return fields;
  }

  parseAddIssue(addIssue: AddIssue, frequentProblems: string[], statuses: IOption[]) {
    if (addIssue == undefined)
      addIssue = new AddIssue();

    statuses.map(q => q.label = this.translate.instant('ticket.' + q.label.toLowerCase()));

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
                placeholder: 'Incarcare cartus...',
                filter: (term) => of(term ? this.filterAutocomplete(term, frequentProblems) : frequentProblems.slice()),
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('ticket.problem'),
              },
            },
            {
              key: 'problemDescription',
              type: 'textarea',
              expressionProperties: {
                'templateOptions.label': this.translate.stream('ticket.description'),
              },
            },
            {
              key: 'status',
              type: 'radio',
              defaultValue: statuses[0].value,
              templateOptions: {
                required: true,
                options: statuses
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('ticket.priority'),
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
        expressionProperties: {
          'templateOptions.label': this.translate.stream('form.identifier'),
          'templateOptions.disabled': 'true',
        },
      },
      {
        key: 'temsid',
        type: 'input-tooltip',
        templateOptions: {
          label: 'TEMSID',
        },
        expressionProperties: {
          'templateOptions.description': this.translate.stream('form.temsIdDescription')
        }
      },
      {
        key: 'serialNumber',
        type: 'input-tooltip',
        templateOptions: {
          label: 'Serial Number',
        },
        expressionProperties: {
          'templateOptions.label': this.translate.stream('form.serialNumber'),
          'templateOptions.description': this.translate.stream('form.serialNumberDescription')
        }
      },
      {
        key: 'isDefect',
        type: 'checkbox',
        defaultValue: false,
        expressionProperties: {
          'templateOptions.label': this.translate.stream('form.isDefect'),
        }
      },
      {
        key: 'isUsed',
        type: 'checkbox',
        defaultValue: true,
        expressionProperties: {
          'templateOptions.label': this.translate.stream('form.isUsed'),
        }
      },
      {
        key: 'description',
        type: 'textarea',
        expressionProperties: {
          'templateOptions.label': this.translate.stream('form.description'),
        }
      },
      {
        fieldGroupClassName: 'row',
        fieldGroup: [
          {
            className: 'col-sm-4',
            key: 'price',
            defaultValue: addEquipment.definition.price,
            type: 'input',
            expressionProperties: {
              'templateOptions.label': this.translate.stream('form.price'),
            }
          },
          {
            className: 'col-sm-4',
            key: 'currency',
            type: 'select',
            defaultValue: addEquipment.definition.currency,
            templateOptions: {
              options: [
                { label: 'LEI', value: 'lei' },
                { label: 'EUR', value: 'eur' },
                { label: 'USD', value: 'usd' },
              ],
            },
            expressionProperties: {
              'templateOptions.label': this.translate.stream('form.currency'),
            }
          },
          {
            className: 'col-sm-4',
            type: 'input',
            key: 'purchaseDate',
            templateOptions: {
              type: 'date',
            },
            expressionProperties: {
              'templateOptions.label': this.translate.stream('form.purchaseDate'),
            }
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
            placeholder: 'model',
            required: true
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('property.name'),
            'templateOptions.description': this.translate.stream('property.nameDescription'),
          },
          validators: {
            validation: ['specCharValidator']
          }
        },
        {
          key: 'displayName',
          type: 'input-tooltip',
          templateOptions: {
            placeholder: 'Model',
            required: true,
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('property.displayName'),
            'templateOptions.description': this.translate.stream('property.displayNameDescription'),
          },
        },
        {
          key: 'description',
          type: 'input-tooltip',
          expressionProperties: {
            'templateOptions.label': this.translate.stream('property.description'),
            'templateOptions.description': this.translate.stream('property.descriptionDescription'),
          },
        },
        {
          key: 'dataType',
          type: 'select',
          templateOptions: {
            required: true,
            options: [
              { value: 'text', label: 'Text' },
              { value: 'number', label: 'Number' },
              { value: 'bool', label: 'Boolean' }, // Other will appear soon
            ]
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('property.dataType'),
          },
        },
        {
          key: 'required',
          type: 'checkbox',
          defaultValue: false,
          expressionProperties: {
            'templateOptions.label': this.translate.stream('property.required'),
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
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('form.identifier'),
                'templateOptions.description': this.translate.stream('key.identifierDescription'),
              },
            },
            {
              key: 'numberOfCopies',
              type: 'input-tooltip',
              templateOptions: {
                type: 'number',
                min: 0,
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('key.copies'),
                'templateOptions.description': this.translate.stream('key.copiesDescription'),
              },
            },
            {
              key: 'description',
              type: 'input',
              expressionProperties: {
                'templateOptions.label': this.translate.stream('form.description'),
              },
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
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('announcement.title'),
              },
            },
            {
              key: 'text',
              type: 'textarea',
              templateOptions: {
                rows: 3,
                required: true,
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('announcement.message'),
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
            required: true,
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.identifier'),
            'templateOptions.description': this.translate.stream('definition.identifierDescription'),
          },
        },
      ];

    addDefinition.properties.forEach(property => {
      fields.push(this.generatePropertyFieldGroup(property))
    });

    fields.push(
      {
        key: 'description',
        type: 'textarea',
        expressionProperties: {
          'templateOptions.label': this.translate.stream('form.description'),
        },
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
                  required: true,
                },
                expressionProperties: {
                  'templateOptions.label': this.translate.stream('form.identifier'),
                  'templateOptions.description': this.translate.stream('definition.childIdentifierDescription'),
                },
              },
                {
                  className: 'col-6',
                  key: 'identifierSelect',
                  type: 'select',
                  templateOptions: {
                    options: this.definitionService.getDefinitionsOfType(childDefinition.equipmentType.value).pipe(tap(defs => {
                      defs.unshift({value: "new", label: "new"});
                    })),
                    change: (field, $event)=>{ 
                      this.setChildDefinition(field.parent, $event.value);
                  },
                  expressionProperties: {
                    'templateOptions.label': this.translate.stream('definition.choseExistingLabel'),
                    'templateOptions.description': this.translate.stream('definition.choseExistingDescription'),
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
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.description'),
          },
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

          let propertyField = childFieldGroup.fieldGroup.find(e => e.key == q.name); 
          if(propertyField != undefined)
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
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.price'),
            'templateOptions.description': this.translate.stream('equipment.priceDescription'),
          },
        },
        {
          className: 'col-4',
          key: 'currency',
          type: 'select',
          defaultValue: defaultCurrency ?? 'lei',
          templateOptions: {
            options: [
              { label: 'LEI', value: 'lei' },
              { label: 'EUR', value: 'eur' },
              { label: 'USD', value: 'usd' },
            ],
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.currency'),
          },
        },
      ]
    }
  }

  parseAddLog() {
    let fields: FormlyFieldConfig[] =
      [
        {
          key: 'log',
          fieldGroup: [
            {
              key: 'description',
              type: 'textarea',
              templateOptions: {
                placeholder: '...',
                rows: 5,
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('form.description'),
              },
            },
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
            disabled: value != undefined,
          },
        }
        break;

      case 'number':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'input-tooltip',
          templateOptions: {
            type: 'number',
            description: addProperty.description,
            label: addProperty.displayName,
            required: addProperty.required,
            min: (addProperty.min == 0 && addProperty.max == 0) ? undefined : addProperty.min,
            max: (addProperty.min == 0 && addProperty.max == 0) ? undefined : addProperty.max,
            disabled: value != undefined,
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
            options: addProperty.options,
            disabled: value != undefined,
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
            options: addProperty.options,
            disabled: value != undefined,
          },
        }
        break;

      case 'bool':
        propertyFieldGroup = {
          key: addProperty.name,
          type: 'checkbox',
          templateOptions: {
            label: addProperty.displayName,
            required: addProperty.required,
            disabled: value != undefined,
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
            options: addProperty.options,
            disabled: value != undefined,
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
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('personnel.name'),
          },
        },
        {
          key: 'phoneNumber',
          type: 'input-tooltip',
          expressionProperties: {
            'templateOptions.label': this.translate.stream('personnel.phoneNumber'),
            'templateOptions.description': this.translate.stream('personnel.phoneNumberDescription'),
          },
        },
        {
          key: 'email',
          type: 'input-tooltip',
          templateOptions: {
            type: 'email',
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('personnel.email'),
            'templateOptions.description': this.translate.stream('personnel.emailDescription'),
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
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('form.username'),
              },
            },
            {
              key: 'password',
              className: "my-2",
              type: 'input',
              templateOptions: {
                required: true,
                type: 'password',
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('form.password'),
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
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.username'),
            'templateOptions.description': this.translate.stream('user.usernameDescription'),
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
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.password'),
            'templateOptions.description': this.translate.stream('user.passwordDescription'),
          },
        },
        {
          key: 'fullName',
          type: 'input',
          expressionProperties: {
            'templateOptions.label': this.translate.stream('user.fullName'),
          },
        },
        {
          key: 'phoneNumber',
          type: 'input-tooltip',
          expressionProperties: {
            'templateOptions.label': this.translate.stream('user.phoneNumber'),
          },
        },
        {
          key: 'email',
          type: 'input-tooltip',
          templateOptions: {
            type: 'email',
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('user.email'),
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
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('password.old'),
          },
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
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('password.new'),
              },
            },
            {
              key: 'confirmNewPass',
              type: 'input',
              templateOptions: {
                type: "password",
                required: true,
              },
              expressionProperties: {
                'templateOptions.label': this.translate.stream('password.confirm'),
              },
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
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.email'),
            'templateOptions.description': this.translate.stream('user.emailNotificationsDescription'),
          },
        },
        {
          key: 'getNotifications',
          type: 'checkbox',
          expressionProperties: {
            'templateOptions.label': this.translate.stream('user.emailNotifications'),
          },
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
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.fullName'),
          },
        },
        {
          key: 'username',
          type: 'input-tooltip',
          templateOptions: {
            minLength: 4,
            required: true,
          },
          expressionProperties: {
            'templateOptions.label': this.translate.stream('form.username'),
            'templateOptions.description': this.translate.stream('form.usernameDescription'),
          },
          validators: {
            validation: ['usernameValidator']
          }
        },
      ];

    return fields;
  }
}
