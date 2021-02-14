import { AddProperty } from './../../models/equipment/add-property.model';
import { EquipmentService } from './../equipment-service/equipment.service';
import { AddDefinition } from './../../models/equipment/add-definition.model';
import { AddType } from './../../models/equipment/add-type.model';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { Injectable } from '@angular/core';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { Observable } from 'rxjs';

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
    let fields : FormlyFieldConfig[] =
    [
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

    if(addDefinition.children.length == 0)
      return fields;

    // Adding children with 'repeat' type
    // 1) preparing
      
    let tempKey = 0;
    addDefinition.children.forEach(childDefinition => {

      fields[fields.length - 1].fieldGroup.push(
        {
          key: ' '+tempKey++, // in realilty - this will be the child definition ID
          type: 'repeat',
          fieldArray:{
            templateOptions: {
              btnText: 'Add Another'
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
   
    console.log(fields);

    return fields;

    //       {
    //         key: 'investments',
    //         type: 'repeat',
    //         fieldArray: {
    //           fieldGroupClassName: 'row',
    //           templateOptions: {
    //             btnText: 'Add another investment',
    //           },
    //           fieldGroup: [
    //             {
    //               className: 'col-sm-4',
    //               type: 'input',
    //               key: 'customerId',
    //               templateOptions: {
    //                 label: 'Customer Id:',
    //                 required: true,
    //               },
    //               expressionProperties: {
    //                 'model.customerId': 'formState.model.customer.id',
    //               },
    //             },
    //             {
    //               className: 'col-sm-4',
    //               type: 'input',
    //               key: 'investmentName',
    //               defaultValue: 'My name',
    //               templateOptions: {
    //                 label: 'Name of Investment:',
    //                 required: true,
    //               },
    //             },
    //             {
    //               type: 'input',
    //               key: 'investmentDate',
    //               className: 'col-sm-3',
    //               templateOptions: {
    //                 type: 'date',
    //                 label: 'Date of Investment:',
    //               },
    //             },
    //             {
    //               type: 'input',
    //               key: 'stockIdentifier',
    //               className: 'col-sm-3',
    //               defaultValue: 'test',
    //               templateOptions: {
    //                 label: 'Stock Identifier:',
    //                 addonRight: {
    //                   class: 'fa fa-code',
    //                   onClick: (to, fieldType, $event) => console.log(to, fieldType, $event),
    //                 },
    //               },
    //             },
    //           ],
    //         },
    //       },  
    //     ]
    //   }
    // ];  






    // let formlyFieldsAddDefinition =
    //   (formlyFields == undefined) ? [] as FormlyFieldConfig[] : formlyFields;

    // formlyFieldsAddDefinition.push({
    //   wrappers: ['formly-wrapper'],
    //   type: 'repeat',
    //   fieldGroup: [
    //     {
    //       className: 'section-label',
    //       template: '<h5>' + addDefinition.equipmentType.name + '</h5>'
    //     },
    //     {
    //       key: 'identifier',
    //       type: 'input',
    //       defaultValue: addDefinition.identifier,
    //       templateOptions: {
    //         label: 'Identifier',
    //         required: true
    //       },
    //     },
    //   ]
    // });

    // addDefinition.properties.forEach(property => {
    //   formlyFieldsAddDefinition[formlyFieldsAddDefinition.length - 1].fieldGroup.push(
    //     this.generatePropertyFieldGroup(property)
    //   )
    // });

    // addDefinition.children.forEach(childDefininition => {
    //   this.parseAddDefinition(
    //     childDefininition, 
    //     formlyFieldsAddDefinition[formlyFieldsAddDefinition.length - 1].fieldGroup);

    //     formlyFieldsAddDefinition[formlyFieldsAddDefinition.length - 1].fieldGroup.push(
    //       {
    //         type: 'button',
    //         templateOptions: {
    //           text: 'Click this guy',
    //           btnType: 'info',
    //           onClick: () => {

    //             console.log(formlyFieldsAddDefinition[formlyFieldsAddDefinition.length - 1].fieldGroup);
    //              this.parseAddDefinition(
    //               childDefininition, 
    //               formlyFieldsAddDefinition[formlyFieldsAddDefinition.length - 1].fieldGroup);

    //             console.log(formlyFieldsAddDefinition[formlyFieldsAddDefinition.length - 1].fieldGroup);

    //           },
    //         },
    //       }
    //     )
    // });

    // return formlyFieldsAddDefinition;
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
