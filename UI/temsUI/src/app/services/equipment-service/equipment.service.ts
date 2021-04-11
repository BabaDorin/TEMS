import { ViewPropertySimplified } from './../../models/equipment/view-property-simplified.model';
import { ViewTypeSiplified } from './../../models/equipment/view-type-simplified.model';
import { TEMSService } from './../tems-service/tems.service';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { AddType } from './../../models/equipment/add-type.model';
import { Definition, AddDefinition } from './../../models/equipment/add-definition.model';
import { API_PROP_URL, API_EQTYPE_URL, API_EQDEF_URL, API_EQ_URL, IEntityCollection } from './../../models/backend.config';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IOption } from './../../models/option.model';
import { CheckboxItem } from '../../models/checkboxItem.model';
import { ViewEquipmentSimplified } from './../../models/equipment/view-equipment-simplified.model';
import { AddProperty } from './../../models/equipment/add-property.model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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

  getDefinitionSimplifiedById(definitionId: string): Observable<ViewDefinitionSimplified>{
    return this.http.get<ViewDefinitionSimplified>(
      API_EQDEF_URL + '/getsimplifiedbyid/' + definitionId,
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

  archieveType(typeId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved

    return this.http.get(
      API_EQTYPE_URL + '/archieve/' + typeId + '/' + archivationStatus,
      this.httpOptions
    );
  }

  archieveDefinition(definitionId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved
    
    return this.http.get(
      API_EQDEF_URL + '/archieve/' + definitionId + '/' + archivationStatus,
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

  archieveProperty(propertyId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved
    
    return this.http.get(
      API_PROP_URL + '/archieve/' + propertyId + '/' + archivationStatus,
      this.httpOptions
    );
  }

  updateProperty(addProperty: AddProperty): Observable<any>{
    return this.http.post<AddProperty>(
      API_PROP_URL + '/update', 
      JSON.stringify(addProperty), 
      this.httpOptions);
  }

  addDefinition(addDefinition: AddDefinition): Observable<any>{
    return this.http.post(
      API_EQDEF_URL + '/add', 
      JSON.stringify(addDefinition), 
      this.httpOptions);
  }

  updateDefinition(addDefinition: AddDefinition): Observable<any>{
    return this.http.post(
      API_EQDEF_URL + '/update',
      JSON.stringify(addDefinition),
      this.httpOptions
    );
  }

  addEquipment(addEquipment: AddEquipment): Observable<any>{
    return this.http.post(
      API_EQ_URL + '/add',
      JSON.stringify(addEquipment),
      this.httpOptions
    );
  }

  archieveEquipment(equipmentId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined)  archivationStatus = true;

    return this.http.get(
      API_EQ_URL + '/archieve/' + equipmentId + '/' + archivationStatus,
      this.httpOptions
    );
  }

  changeState(attribute: string, equipmentId: string): Observable<any>{
    return this.http.get(
      API_EQ_URL + '/changeworkingstate/' + attribute + '/' + equipmentId,
      this.httpOptions
    );
  }

  updateEquipment(addEquipment: AddEquipment): Observable<any>{
    return this.http.post(
      API_EQ_URL + '/update',
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

  getPropertiesOfType(typeId: string): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_PROP_URL + '/getpropertiesoftype/' + typeId,
      this.httpOptions
    );
  }

  getEquipmentToUpdate(equipmentId: string): Observable<AddEquipment>{
    return this.http.get<AddEquipment>(
      API_EQ_URL + '/getequipmenttoupdate/' + equipmentId,
      this.httpOptions
    );
  } 

  getEquipmentSimplified(
    pageNumber: number, 
    recordsPerPage: number,
    onlyParent: boolean,
    entityCollection: IEntityCollection): Observable<any>{
      if(onlyParent == undefined) onlyParent = true;
      if(entityCollection.roomIds == undefined) entityCollection.roomIds = [];
      if(entityCollection.personnelIds == undefined) entityCollection.personnelIds = [];

      console.log(entityCollection.roomIds);

      let params = new HttpParams();
      entityCollection.roomIds.forEach(id => {
        params = params.append('rooms', id);
      })

      entityCollection.personnelIds.forEach(id => {
        params = params.append('personnel', id);
      })
      
      return this.http.get(API_EQ_URL + '/getsimplified/'+pageNumber+'/'+recordsPerPage+'/'+onlyParent,
      {params: params});      
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
  
  getAllAutocompleteOptions(filter?: string, onlyParent?: boolean): Observable<any> {
    if(onlyParent == undefined) 
      onlyParent = true;
    return this.http.get(
      API_EQ_URL + '/getallautocompleteoptions/' + onlyParent + '/' + filter, 
      this.httpOptions);
  }

  getFullDefinition(definitionId: string): Observable<Definition> {
    return this.http.get<Definition>(
      API_EQDEF_URL + '/getfulldefinition/' + definitionId, 
      this.httpOptions
      );
  }

  getFullType(typeId: string): Observable<any> {
    return this.http.post(API_EQTYPE_URL + '/fulltype', JSON.stringify(typeId), this.httpOptions);
  }

  generateAddEquipmentOfDefinition(definition: Definition): AddEquipment {
    return new AddEquipment(definition);
  }
  
  detach(equipmentId: string): Observable<any> {
    return this.http.get(
      API_EQ_URL + '/detach/' + equipmentId,
      this.httpOptions
    );
  }
}
