import { AddType } from './../../models/equipment/add-type.model';
import { EquipmentType } from './../../models/equipment/view-type.model';
import { Definition } from './../../models/equipment/add-definition.model';
import { API_PROP_URL, API_EQTYPE_URL } from './../../models/backend.config';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { IOption } from './../../models/option.model';
import { CheckboxItem } from '../../models/checkboxItem.model';
import { ViewEquipmentAllocation } from './../../models/equipment/view-equipment-allocation.model';
import { ViewEquipment } from './../../models/equipment/view-equipment.model';
import { ViewEquipmentSimplified } from './../../models/equipment/view-equipment-simplified.model';
import { AddProperty } from './../../models/equipment/add-property.model';
import { AddEquipment } from '../../models/equipment/add-equipment.model';
import { Injectable, Type } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EquipmentService {

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':'application/json; charset=utf-8'
    })
  }
  constructor(
    private http: HttpClient
  ) { }

  getTypes(): Observable<any> {
    return this.http.get(API_EQTYPE_URL + '/get');
  }

  postType(addType: AddType): Observable<any> {
    return this.http.post<AddType>(API_EQTYPE_URL + '/insert', JSON.stringify(addType), this.httpOptions);
  }

  postProperty(addProperty: AddProperty): Observable<any>{
    return this.http.post<AddProperty>(API_PROP_URL + '/insert', JSON.stringify(addProperty), this.httpOptions);
  }

  getTypesAutocomplete(): IOption[]{
    return [
      { value: '1', label: 'printer' },
      { value: '2', label: 'laptop' },
      { value: '3', label: 'scanner' },
    ] 
  }

  getDefinitionsAutocomplete(ofTypes: IOption[]): IOption[]{
    return [
      { value: '1', label: 'printer' },
      { value: '2', label: 'laptop' },
      { value: '3', label: 'scanner' },
    ]
  }

  getProperties(): Observable<any>{
    return this.http.get(API_PROP_URL + '/get');
  }

  getCommonProperties(): CheckboxItem[]{
    return [
      new CheckboxItem('temsid', 'Tems ID'),
      new CheckboxItem('serialNumber', 'Serial Number'),
    ]
  }

  getTypeSpecificProperties(type: IOption): CheckboxItem[]{
    return [
      new CheckboxItem('temsid', 'Tems ID'),
      new CheckboxItem('serialNumber', 'Serial Number'),
    ]
  }

  getEquipment(): ViewEquipmentSimplified[]{
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

  getEquipmentSimplified(id: string): ViewEquipmentSimplified{
    return new ViewEquipmentSimplified();
  }

  getEquipmentByID(id: string): ViewEquipment{
    return new ViewEquipment();
  }

  getEquipmentAllocations(id: string): ViewEquipmentAllocation[]{
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

  getAllAutocompleteOptions(): IOption[] {
    return [
      {value: '1', label: 'LPB301', },
      {value: '2', label: 'LPB301', },
      {value: '3', label: 'LPB301', },
      {value: '4', label: 'LPB01', },
      {value: '5', label: 'A2B301', },
      {value: '6', label: 'WQE2N3II4I4220001', },
      {value: '7', label: 'L04322', },
      {value: '8', label: 'PC001', },
      {value: '9', label: 'PC002', },
      {value: '10', label: 'OC2332', },
    ]
  }

  getFileterdAutocompleteOptions(filter: string){
    // might be unusable..
  }

  getPropertiesOfType(typeId: string): AddProperty[] {
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

  getDefinitionsOfType(typeId: string): IOption[] {
    let printerDefinitions: IOption[] = [
      { value: '1', label: 'HP LaserJet' },
      { value: '2', label: 'Lenovo M7000' }
    ];

    let pcDefinitions: IOption[] = [
      { value: '3', label: 'Hantol' },
      { value: '4', label: 'HPC' },
      { value: '5', label: 'Sohoo' },
    ];

    return printerDefinitions;
  }

  getFullDefinition(definitionId: string): Definition {

    return new Definition();
    // // returns the full definition, including children definitions and so on...
    // let fullDefinitions: Definition[] = [
    //   {
    //     type: new Type(),
    //     id: '1',
    //     identifier: 'HP LaserJet',
    //     equipmentType: { value: '1', label: 'printer'},
    //     properties: [
    //       {
    //         id: '1',
    //         name: 'Model',
    //         displayName: 'Model',
    //         description: 'the model',
    //         dataType: { id: '1', name: 'string' },
    //         value: 'HP LaserJet',
    //         required: true
    //       },
    //       {
    //         id: '2',
    //         name: 'color',
    //         displayName: 'Color',
    //         description: 'Color = true, B&W = false',
    //         dataType: { id: '2', name: 'radiobutton' },
    //         options: [{ value: 'color', label: 'color' }, { value: 'black and white', label: 'b&w' }],
    //         required: true
    //       },
    //     ],
    //     children: [
    //     ],
    //   },
    //   {
    //     type: new Type(),
    //     id: '2',
    //     identifier: 'Lenovo M700',
    //     equipmentType: { value: '1', label: 'printer'},
    //     properties: [
    //       {
    //         id: '1',
    //         name: 'Model',
    //         displayName: 'Model',
    //         description: 'the model',
    //         dataType: { id: '1', name: 'string' },
    //         value: 'HP LaserJet',
    //         required: true
    //       },
    //       {
    //         id: '2',
    //         name: 'Color',
    //         displayName: 'Color',
    //         description: 'Color = true, B&W = false',
    //         dataType: { id: '2', name: 'bool' },
    //         value: 'false',
    //         required: true
    //       },
    //     ],
    //     children: [{
    //       type: new Type(),
    //       id: '2',
    //       identifier: 'Lenovo M700',
    //       equipmentType: { value: '1', label: 'printer'},
    //       properties: [
    //         {
    //           id: '1',
    //           name: 'Model',
    //           displayName: 'Model',
    //           description: 'the model',
    //           dataType: { id: '1', name: 'string' },
    //           value: 'HP LaserJet',
    //           required: true
    //         },
    //         {
    //           id: '2',
    //           name: 'Color',
    //           displayName: 'Color',
    //           description: 'Color = true, B&W = false',
    //           dataType: { id: '2', name: 'bool' },
    //           value: 'false',
    //           required: true
    //         },
    //       ],
    //       children: []
    //     },
    //     {
    //       type: new Type(),
    //       id: '2',
    //       identifier: 'not lenovo M700',
    //       equipmentType: { value: '1', label: 'printer'},
    //       properties: [
    //         {
    //           id: '1',
    //           name: 'Model',
    //           displayName: 'Model',
    //           description: 'the model',
    //           dataType: { id: '1', name: 'string' },
    //           value: 'HP LaserJet',
    //           required: true
    //         },
    //         {
    //           id: '2',
    //           name: 'Color',
    //           displayName: 'Color',
    //           description: 'Color = true, B&W = false',
    //           dataType: { id: '2', name: 'bool' },
    //           value: 'false',
    //           required: true
    //         },
    //       ],
    //       children: []
    //     }
    //   ],
    //   }
    // ];
    // return fullDefinitions.find(q => q.id == definitionId);
  }

  getFullType(typeId: string): Observable<any> {
    return this.http.post(API_EQTYPE_URL + '/getfulltype', JSON.stringify(typeId), this.httpOptions);
    // return new EquipmentType();
    // let fullType: AddType = {
    //   id: typeId,
    //   name: (typeId == "1") ? 'printer' : (typeId == "2") ? 'laptop' : 'scanner',
    //   properties: this.getPropertiesOfType(typeId),
    //   children: [
    //     {
    //       id: '1',
    //       name: 'cartrige',
    //       properties: this.getPropertiesOfType(typeId),
    //     },
    //     {
    //       id: '2',
    //       name: 'microprocessor',
    //       properties: this.getPropertiesOfType(typeId),
    //     },
    //   ]
    // }
    // return fullType;
    // 
  }

  generateAddEquipmentOfDefinition(definition: Definition): AddEquipment {
    return new AddEquipment(definition);
  }
} 
