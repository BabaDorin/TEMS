import { IOption } from 'src/app/models/option.model';
import { ViewEquipmentAllocation } from './../../models/equipment/view-equipment-allocation.model';
import { ViewEquipment } from './../../models/equipment/view-equipment.model';
import { ViewEquipmentSimplified } from './../../models/equipment/view-equipment-simplified.model';
import { AddProperty } from './../../models/equipment/add-property.model';
import { AddEquipment } from '../../models/equipment/add-equipment.model';
import { AddDefinition } from '../../models/equipment/add-definition.model';
import { LightDefinition } from '../../models/equipment/viewlight-definition.model';
import { Property } from '../../models/equipment/view-property.model';
import { Type } from '../../models/equipment/view-type.model';
import { AddType } from '../../models/equipment/add-type.model';
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

  getTypes() {
    // fake service
    let types = [
      { id: '1', name: 'printer' },
      { id: '2', name: 'laptop' },
      { id: '3', name: 'scanner' },
    ]
    return types;
  }

  getTypesAutocomplete(){
    return [
      { id: '1', value: 'printer' },
      { id: '2', value: 'laptop' },
      { id: '3', value: 'scanner' },
    ] 
  }

  getDefinitionsAutocomplete(ofTypes: IOption[]){
    // if ofTypes = nul => all of the definitions
    return [
      { id: '1', value: 'printer' },
      { id: '2', value: 'laptop' },
      { id: '3', value: 'scanner' },
    ]
  }

  getProperties() {

    // id: string,
    // name: string,
    // displayName: string,
    // description: string,
    // dataType: DataType,
    // value?: string

    let properties: AddProperty[] = [
      {
        id: '1',
        name: 'model',
        displayName: 'Model',
        description: 'Model name',
        dataType: { id: '1', name: 'string' },
        required: true,
      },
      {
        id: '2',
        name: 'Frequency',
        displayName: 'Frequency',
        description: 'Frequency in GHz',
        dataType: { id: '2', name: 'number' },
        required: true,
      },
      {
        id: '3',
        name: 'color',
        displayName: 'Color',
        description: 'Black and White or Color',
        dataType: { id: '3', name: 'radiobutton' },
        options: [{ value: 'color', label: 'color' }, { value: 'bw', label: 'black and white' }],
        required: true,
      },
      {
        id: '4',
        name: 'Resolution',
        displayName: 'Resolution',
        description: 'Resolution',
        dataType: { id: '1', name: 'string' },
        required: true,
      },
    ];

    return properties;
  }

  getEquipment(){
    // returns the list of all equipment records
    let equipments: ViewEquipmentSimplified[] = [
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
      new ViewEquipmentSimplified(), new ViewEquipmentSimplified(), new ViewEquipmentSimplified(),
    ];

    return equipments;
  }

  getEquipmentSimplified(id: string){
    return new ViewEquipmentSimplified();
  }

  getEquipmentByID(id: string){
    return new ViewEquipment();
  }

  getEquipmentAllocations(id: string){
    return [
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
    ]
  }

  getAllAutocompleteOptions(){
    return [
      {id: '1', value: 'LPB301', },
      {id: '2', value: 'LPB301', },
      {id: '3', value: 'LPB301', },
      {id: '4', value: 'LPB01', },
      {id: '5', value: 'A2B301', },
      {id: '6', value: 'WQE2N3II4I4220001', },
      {id: '7', value: 'L04322', },
      {id: '8', value: 'PC001', },
      {id: '9', value: 'PC002', },
      {id: '10', value: 'OC2332', },
    ]
  }

  getFileterdAutocompleteOptions(filter: string){
    // might be unusable..
  }

  getPropertiesOfType(typeId: string) {
    // send type to API, it will return the list of properties

    // fake service
    let pcProperties: AddProperty[] = [
      {
        id: '1',
        name: 'model',
        displayName: 'Model',
        description: 'Model name',
        dataType: { id: '1', name: 'string' },
        required: true,
      },
      {
        id: '2',
        name: 'Frequency',
        displayName: 'Frequency',
        description: 'Frequency in GHz',
        dataType: { id: '2', name: 'number' },
        required: true,
      },
      {
        id: '3',
        name: 'color',
        displayName: 'Color',
        description: 'Black and White or Color',
        dataType: { id: '3', name: 'radiobutton' },
        options: [{ value: 'color', label: 'color' }, { value: 'bw', label: 'black and white' }],
        required: true,
      },
    ];

    let printerProperties: AddProperty[] = [
      {
        id: '1',
        name: 'model',
        displayName: 'Model',
        description: 'Model name',
        dataType: { id: '1', name: 'string' },
        required: true,
      },
      {
        id: '2',
        name: 'Frequency',
        displayName: 'Frequency',
        description: 'Frequency in GHz',
        dataType: { id: '2', name: 'number' },
        required: true,
      },
      {
        id: '3',
        name: 'color',
        displayName: 'Color',
        description: 'Black and White or Color',
        dataType: { id: '3', name: 'radiobutton' },
        options: [{ value: 'color', label: 'color' }, { value: 'bw', label: 'black and white' }],
        required: true,
      },
    ];

    return (typeId == '1') ? printerProperties : pcProperties;
  }

  getDefinitionsOfType(type: Type) {
    // send type to API, it will return the list of properties

    // fake service
    let printerDefinitions: LightDefinition[] = [
      { id: '1', name: 'HP LaserJet' },
      { id: '2', name: 'Lenovo M7000' }
    ];

    let pcDefinitions: LightDefinition[] = [
      { id: '3', name: 'Hantol' },
      { id: '4', name: 'HPC' },
      { id: '5', name: 'Sohoo' },
    ];

    if (type.name == 'printer')
      return printerDefinitions;

    if (type.name == 'pc')
      return pcDefinitions;
  }

  getFullDefinition(definitionId: string) {
    // returns the full definition, including children definitions and so on...

    let fullDefinitions: AddDefinition[] = [
      {
        id: '1',
        identifier: 'HP LaserJet',
        equipmentType: { id: '1', name: 'printer', parents: [], properties: [] },
        properties: [
          {
            id: '1',
            name: 'Model',
            displayName: 'Model',
            description: 'the model',
            dataType: { id: '1', name: 'string' },
            value: 'HP LaserJet',
            required: true
          },
          {
            id: '2',
            name: 'color',
            displayName: 'Color',
            description: 'Color = true, B&W = false',
            dataType: { id: '2', name: 'radiobutton' },
            options: [{ value: 'color', label: 'color' }, { value: 'black and white', label: 'b&w' }],
            required: true
          },
        ],
        children: [
        ],
      },

      {
        id: '2',
        identifier: 'Lenovo M700',
        equipmentType: { id: '1', name: 'printer', parents: [], properties: [] },
        properties: [
          {
            id: '1',
            name: 'Model',
            displayName: 'Model',
            description: 'the model',
            dataType: { id: '1', name: 'string' },
            value: 'HP LaserJet',
            required: true
          },
          {
            id: '2',
            name: 'Color',
            displayName: 'Color',
            description: 'Color = true, B&W = false',
            dataType: { id: '2', name: 'bool' },
            value: 'false',
            required: true
          },
        ],
        children: [{
          id: '2',
          identifier: 'Lenovo M700',
          equipmentType: { id: '1', name: 'printer', parents: [], properties: [] },
          properties: [
            {
              id: '1',
              name: 'Model',
              displayName: 'Model',
              description: 'the model',
              dataType: { id: '1', name: 'string' },
              value: 'HP LaserJet',
              required: true
            },
            {
              id: '2',
              name: 'Color',
              displayName: 'Color',
              description: 'Color = true, B&W = false',
              dataType: { id: '2', name: 'bool' },
              value: 'false',
              required: true
            },
          ],
          children: []
        },
        {
          id: '2',
          identifier: 'not lenovo M700',
          equipmentType: { id: '1', name: 'printer', parents: [], properties: [] },
          properties: [
            {
              id: '1',
              name: 'Model',
              displayName: 'Model',
              description: 'the model',
              dataType: { id: '1', name: 'string' },
              value: 'HP LaserJet',
              required: true
            },
            {
              id: '2',
              name: 'Color',
              displayName: 'Color',
              description: 'Color = true, B&W = false',
              dataType: { id: '2', name: 'bool' },
              value: 'false',
              required: true
            },
          ],
          children: []
        }
      ],
      }
    ];



    // BELOW - Defintion with children
    // let hpLaserJet: AddDefinition = {
    //   id: '1',
    //   identifier: 'HP LaserJet',
    //   equipmentType: { id: '1', name: 'printer', children: [] },
    //   properties: [
    //     {
    //       id: '1',
    //       name: 'Model',
    //       displayName: 'Model',
    //       description: 'the model',
    //       dataType: { id: '1', name: 'string' },
    //       value: 'HP LaserJet'
    //     },
    //     {
    //       id: '2',
    //       name: 'Color',
    //       displayName: 'Color',
    //       description: 'Color = true, B&W = false',
    //       dataType: { id: '2', name: 'bool' },
    //       value: 'false'
    //     },
    //   ],
    //   children: [
    //     {
    //       id: '1',
    //       identifier: 'CB285A',
    //       equipmentType: { id: '4', name: 'cartrige', children: [] },
    //       properties: [
    //         {
    //           id: '1',
    //           name: 'Model',
    //           displayName: 'Model',
    //           description: 'the model',
    //           dataType: { id: '1', name: 'string' },
    //           value: 'CB285A'
    //         },
    //         {
    //           id: '2',
    //           name: 'Color',
    //           displayName: 'Color',
    //           description: 'Color = true, B&W = false',
    //           dataType: { id: '2', name: 'bool' },
    //           value: 'false'
    //         },
    //       ],
    //       children: [
    //         {
    //           id: '1',
    //           identifier: 'CB285A',
    //           equipmentType: { id: '4', name: 'cartrige', children: [] },
    //           properties: [
    //             {
    //               id: '1',
    //               name: 'Model',
    //               displayName: 'Model',
    //               description: 'the model',
    //               dataType: { id: '1', name: 'string' },
    //               value: 'CB285A'
    //             },
    //             {
    //               id: '2',
    //               name: 'Color',
    //               displayName: 'Color',
    //               description: 'Color = true, B&W = false',
    //               dataType: { id: '2', name: 'bool' },
    //               value: 'false'
    //             },
    //           ],
    //           children: [
    //             {
    //               id: '1',
    //               identifier: 'CB285A',
    //               equipmentType: { id: '4', name: 'cartrige', children: [] },
    //               properties: [
    //                 {
    //                   id: '1',
    //                   name: 'Model',
    //                   displayName: 'Model',
    //                   description: 'the model',
    //                   dataType: { id: '1', name: 'string' },
    //                   value: 'CB285A'
    //                 },
    //                 {
    //                   id: '2',
    //                   name: 'Color',
    //                   displayName: 'Color',
    //                   description: 'Color = true, B&W = false',
    //                   dataType: { id: '2', name: 'bool' },
    //                   value: 'false'
    //                 },
    //               ],
    //               children: [
    //                 {
    //                   id: '1',
    //                   identifier: 'CB285A',
    //                   equipmentType: { id: '4', name: 'cartrige', children: [] },
    //                   properties: [
    //                     {
    //                       id: '1',
    //                       name: 'Model',
    //                       displayName: 'Model',
    //                       description: 'the model',
    //                       dataType: { id: '1', name: 'string' },
    //                       value: 'CB285A'
    //                     },
    //                     {
    //                       id: '2',
    //                       name: 'Color',
    //                       displayName: 'Color',
    //                       description: 'Color = true, B&W = false',
    //                       dataType: { id: '2', name: 'bool' },
    //                       value: 'false'
    //                     },
    //                   ],
    //                   children: []
    //                 }
    //               ]
    //             },
    //             {
    //               id: '1',
    //               identifier: 'CB285A',
    //               equipmentType: { id: '4', name: 'cartrige', children: [] },
    //               properties: [
    //                 {
    //                   id: '1',
    //                   name: 'Model',
    //                   displayName: 'Model',
    //                   description: 'the model',
    //                   dataType: { id: '1', name: 'string' },
    //                   value: 'CB285A'
    //                 },
    //                 {
    //                   id: '2',
    //                   name: 'Color',
    //                   displayName: 'Color',
    //                   description: 'Color = true, B&W = false',
    //                   dataType: { id: '2', name: 'bool' },
    //                   value: 'false'
    //                 },
    //               ],
    //               children: []
    //             }
    //           ]
    //         }
    //       ]
    //     }
    //   ]
    // }

    return fullDefinitions.find(q => q.id == definitionId);
  }

  getFullType(typeId: string) {
    let fullType: AddType = {
      id: typeId,
      name: (typeId == "1") ? 'printer' : (typeId == "2") ? 'laptop' : 'scanner',
      properties: this.getPropertiesOfType(typeId),
      children: [
        {
          id: '1',
          name: 'cartrige',
          properties: this.getPropertiesOfType(typeId),
        },
        {
          id: '2',
          name: 'microprocessor',
          properties: this.getPropertiesOfType(typeId),
        },
      ]
    }

    return fullType;
  }

  generateAddEquipmentOfDefinition(definition: AddDefinition) {
    return new AddEquipment(definition);
  }
} 
