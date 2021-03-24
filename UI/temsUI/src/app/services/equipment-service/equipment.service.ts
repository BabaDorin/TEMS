import { ViewPropertySimplified } from './../../models/equipment/view-property-simplified.model';
import { ViewTypeSiplified } from './../../models/equipment/view-type-simplified.model';
import { TEMSService } from './../tems-service/tems.service';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { AddType } from './../../models/equipment/add-type.model';
import { EquipmentType } from './../../models/equipment/view-type.model';
import { Definition, AddDefinition } from './../../models/equipment/add-definition.model';
import { API_PROP_URL, API_EQTYPE_URL, API_EQDEF_URL, API_EQ_URL } from './../../models/backend.config';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { IOption } from './../../models/option.model';
import { CheckboxItem } from '../../models/checkboxItem.model';
import { ViewAllocationSimplified } from './../../models/equipment/view-equipment-allocation.model';
import { ViewEquipment } from './../../models/equipment/view-equipment.model';
import { ViewEquipmentSimplified } from './../../models/equipment/view-equipment-simplified.model';
import { AddProperty } from './../../models/equipment/add-property.model';
import { Injectable, Type } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ViewDefinitionSimplified } from 'src/app/models/equipment/view-definition-simplified.model';

@Injectable({
  providedIn: 'root'
})
export class EquipmentService extends TEMSService {

  
  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  getTypes(): Observable<IOption[]> {
    return this.http.get<IOption[]>(
      API_EQTYPE_URL + '/get',
      this.httpOptions
      );
  }

  getTypesSimplified(): Observable<ViewTypeSiplified[]>{
    return this.http.get<ViewTypeSiplified[]>(
      API_EQTYPE_URL + '/getsimplified',
      this.httpOptions
    );
  }

  getPropertiesSimplified(): Observable<ViewPropertySimplified[]>{
    return this.http.get<ViewPropertySimplified[]>(
      API_PROP_URL + '/getsimplified',
      this.httpOptions
    );
  }

  getDefinitionsSimplified(): Observable<ViewDefinitionSimplified[]>{
    return this.http.get<ViewDefinitionSimplified[]>(
      API_EQDEF_URL + '/getsimplified',
      this.httpOptions
    );
  }

  postType(addType: AddType): Observable<any> {
    return this.http.post<AddType>(
      API_EQTYPE_URL + '/insert', 
      JSON.stringify(addType), 
      this.httpOptions);
  }

  postProperty(addProperty: AddProperty): Observable<any>{
    return this.http.post<AddProperty>(
      API_PROP_URL + '/insert', 
      JSON.stringify(addProperty), 
      this.httpOptions);
  }

  createDefinition(addDefinition: AddDefinition): Observable<any>{
    return this.http.post<AddDefinition>(
      API_EQDEF_URL + '/insert', 
      JSON.stringify(addDefinition), 
      this.httpOptions);
  }

  createEquipment(addEquipment: AddEquipment): Observable<any>{
    return this.http.post(
      API_EQ_URL + '/create',
      JSON.stringify(addEquipment),
      this.httpOptions
    );
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

  getEquipmentSimplified(
    pageNumber: number, 
    recordsPerPage: number,
    onlyParent?: boolean): Observable<any>{
      if(onlyParent == undefined) onlyParent = true;
      return this.http.get(API_EQ_URL + '/getsimplified/'+pageNumber+'/'+recordsPerPage+'/'+onlyParent,
      this.httpOptions);      
  }

  getEquipmentSimplifiedById(id: string): Observable<any>{
    return this.http.get(
      API_EQ_URL + '/getsimplified/' + id,
      this.httpOptions
      );
  }

  getEquipmentByID(id: string): Observable<any>{
    return this.http.get(
      API_EQ_URL + '/getbyid/' + id,
      this.httpOptions
    );
  }
  
  getAllAutocompleteOptions(onlyParent?: boolean): Observable<any> {
    if(onlyParent == undefined) 
      onlyParent = true;
    return this.http.get(
      API_EQ_URL + '/getallautocompleteoptions/' + onlyParent, 
      this.httpOptions);
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

  getDefinitionsOfType(typeId: string): Observable<any> {
    console.log(typeId);
    return this.http.post(API_EQDEF_URL + '/getdefinitionsoftype', JSON.stringify(typeId), this.httpOptions)

    // let printerDefinitions: IOption[] = [
    //   { value: '1', label: 'HP LaserJet' },
    //   { value: '2', label: 'Lenovo M7000' }
    // ];

    // let pcDefinitions: IOption[] = [
    //   { value: '3', label: 'Hantol' },
    //   { value: '4', label: 'HPC' },
    //   { value: '5', label: 'Sohoo' },
    // ];

    // return printerDefinitions;
  }

  getFullDefinition(definitionId: string): Observable<any> {

    return this.http.post(
      API_EQDEF_URL + '/getfulldefinition', 
      JSON.stringify(definitionId),
      this.httpOptions);
    // return new Definition();
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
    return this.http.post(API_EQTYPE_URL + '/fulltype', JSON.stringify(typeId), this.httpOptions);
  }

  generateAddEquipmentOfDefinition(definition: Definition): AddEquipment {
    return new AddEquipment(definition);
  }
} 
