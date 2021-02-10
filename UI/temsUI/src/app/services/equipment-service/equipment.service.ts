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
    let types: Type[] = [
      { id: '1', name: 'printer' },
      { id: '2', name: 'laptop' },
      { id: '3', name: 'scanner' },
    ]
    return types;
  }

  getPropertiesOfType(type: Type) {
    // send type to API, it will return the list of properties

    // fake service
    let pcProperties: Property[] = [
      { id: '1', displayName: 'Model', description: 'The model', dataType: { id: '1', name: 'string' } },
      { id: '3', displayName: 'refreshRate', description: 'Monitors refresh rate', dataType: { id: '2', name: 'bool' } },
    ];

    let printerProperties: Property[] = [
      { id: '1', displayName: 'Model', description: 'The model', dataType: { id: '1', name: 'string' } },
      { id: '2', displayName: 'Color', description: 'Color or black and white?', dataType: { id: '2', name: 'bool' } },
    ];

    return (type.name == 'printer') ? printerProperties : pcProperties;
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
        equipmentType: { id: '1', name: 'printer', parents: [] },
        properties: [
          {
            id: '1',
            name: 'Model',
            displayName: 'Model',
            description: 'the model',
            dataType: { id: '1', name: 'string' },
            value: 'HP LaserJet'
          },
          {
            id: '2',
            name: 'Color',
            displayName: 'Color',
            description: 'Color = true, B&W = false',
            dataType: { id: '2', name: 'bool' },
            value: 'false'
          },
        ],
        children: [],
      },

      {
      id: '2',
      identifier: 'Lenovo M700',
      equipmentType: { id: '1', name: 'printer', parents: [] },
      properties: [
        {
          id: '1',
          name: 'Model',
          displayName: 'Model',
          description: 'the model',
          dataType: { id: '1', name: 'string' },
          value: 'HP LaserJet'
        },
        {
          id: '2',
          name: 'Color',
          displayName: 'Color',
          description: 'Color = true, B&W = false',
          dataType: { id: '2', name: 'bool' },
          value: 'false'
        },
      ],
      children: [],
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

  generateAddEquipmentOfDefinition(definition: AddDefinition) {
    return new AddEquipment(definition);
  }
} 
