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
import { Property } from 'src/app/models/equipment/view-property.model';

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

  getTypeSimplifiedById(typeId: string): Observable<ViewTypeSiplified>{
    return this.http.get<ViewTypeSiplified>(
      API_EQTYPE_URL + '/getsimplifiedbyid/' + typeId,
      this.httpOptions
    );
  }

  getPropertiesSimplified(): Observable<ViewPropertySimplified[]>{
    return this.http.get<ViewPropertySimplified[]>(
      API_PROP_URL + '/getsimplified',
      this.httpOptions
    );
  }

  getPropertySimplifiedById(propertyId: string): Observable<ViewPropertySimplified>{
    return this.http.get<ViewPropertySimplified>(
      API_PROP_URL + '/getsimplifiedbyid/' + propertyId,
      this.httpOptions
    );
  }

  getDefinitionsSimplified(): Observable<ViewDefinitionSimplified[]>{
    return this.http.get<ViewDefinitionSimplified[]>(
      API_EQDEF_URL + '/getsimplified',
      this.httpOptions
    );
  }

  addType(addType: AddType): Observable<any> {
    return this.http.post(
      API_EQTYPE_URL + '/add', 
      JSON.stringify(addType), 
      this.httpOptions);
  }

  updateType(addType: AddType): Observable<any>{
    return this.http.post(
      API_EQTYPE_URL + '/update', 
      JSON.stringify(addType), 
      this.httpOptions);
  }

  removeType(typeId: string): Observable<any>{
    return this.http.get(
      API_EQTYPE_URL + '/remove/'+typeId,
      this.httpOptions
    );
  }

  removeDefinition(definitionId: string): Observable<any>{
    return this.http.get(
      API_EQDEF_URL + '/remove/' + definitionId,
      this.httpOptions
    );
  }

  getDefinitionToUpdate(definitionId: string): Observable<AddDefinition>{
    return this.http.get<AddDefinition>(
      API_EQDEF_URL + '/getdefinitiontoupdate/' + definitionId,
      this.httpOptions
    );
  }

  addProperty(addProperty: AddProperty): Observable<any>{
    return this.http.post<AddProperty>(
      API_PROP_URL + '/add', 
      JSON.stringify(addProperty), 
      this.httpOptions);
  }

  removeProperty(propertyId): Observable<any>{
    return this.http.get(
      API_PROP_URL + '/remove/' + propertyId,
      this.httpOptions
    );
  }

  updateProperty(addProperty: AddProperty): Observable<any>{
    return this.http.post<AddProperty>(
      API_PROP_URL + '/update', 
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

  getProperties(): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_PROP_URL + '/get',
      this.httpOptions
      );
  }

  getPropertyById(propertyId: string): Observable<Property>{
    return this.http.get<Property>(
      API_PROP_URL + '/getbyid/' + propertyId,
      this.httpOptions
    );
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

  getDefinitionsOfType(typeId: string): Observable<any> {
    console.log(typeId);
    return this.http.post(API_EQDEF_URL + '/getdefinitionsoftype', JSON.stringify(typeId), this.httpOptions)
  }

  getFullDefinition(definitionId: string): Observable<any> {

    return this.http.post(
      API_EQDEF_URL + '/getfulldefinition', 
      JSON.stringify(definitionId),
      this.httpOptions);
  }

  getFullType(typeId: string): Observable<any> {
    return this.http.post(API_EQTYPE_URL + '/fulltype', JSON.stringify(typeId), this.httpOptions);
  }

  generateAddEquipmentOfDefinition(definition: Definition): AddEquipment {
    return new AddEquipment(definition);
  }
} 
