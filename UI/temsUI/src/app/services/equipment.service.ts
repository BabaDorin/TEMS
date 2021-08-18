import { EquipmentFilter } from './../helpers/filters/equipment.filter';
import { ViewPropertySimplified } from './../models/equipment/view-property-simplified.model';
import { AddEquipment } from 'src/app/models/equipment/add-equipment.model';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ViewDefinitionSimplified } from 'src/app/models/equipment/view-definition-simplified.model';
import { Property } from 'src/app/models/equipment/view-property.model';
import { AttachEquipment } from 'src/app/models/equipment/attach-equipment.model';
import { ViewTypeSimplified } from 'src/app/models/equipment/view-type-simplified.model';
import { TEMSService } from './tems.service';
import { API_ALL_URL, API_EQDEF_URL, API_EQTYPE_URL, API_EQ_URL, API_PROP_URL, IEntityCollection } from '../models/backend.config';
import { AddType } from '../models/equipment/add-type.model';
import { AddDefinition, Definition } from '../models/equipment/add-definition.model';
import { AddProperty } from '../models/equipment/add-property.model';
import { IOption } from '../models/option.model';

@Injectable({
  providedIn: 'root'
})
export class EquipmentService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  removeEquipment(equipmentId: string): Observable<any>{
    return this.http.delete(
      API_EQ_URL + '/remove/' + equipmentId,
      this.httpOptions
    );
  }

  removeProperty(propertyId: string): Observable<any>{
    return this.http.delete(
      API_PROP_URL + '/remove/' + propertyId,
      this.httpOptions
    );
  }

  removeAllocation(allocationId: string): Observable<any>{
    return this.http.delete(
      API_ALL_URL + '/remove/' + allocationId,
      this.httpOptions
    );
  }

  getTypesSimplified(): Observable<ViewTypeSimplified[]>{
    return this.http.get<ViewTypeSimplified[]>(
      API_EQTYPE_URL + '/getsimplified',
      this.httpOptions
    );
  }

  getTypeSimplifiedById(typeId: string): Observable<ViewTypeSimplified>{
    return this.http.get<ViewTypeSimplified>(
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
    return this.http.put(
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
    return this.http.put<AddProperty>(
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
    return this.http.put(
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

  changeWorkingState(equipmentId: string, isWorking?: boolean): Observable<any>{
    let endPoint = API_EQ_URL + '/changeworkingstate/' + equipmentId;
    if(isWorking != undefined) 
      endPoint +='/' + isWorking;
    
    return this.http.get(
      endPoint,
      this.httpOptions
    );
  }

  changeUsingState(equipmentId: string, isUsed?: boolean): Observable<any>{
    let endPoint = API_EQ_URL + '/changeusingstate/' + equipmentId;
    if(isUsed != undefined) 
      endPoint +='/' + isUsed;

    return this.http.get(
      endPoint,
      this.httpOptions
    );
  }

  updateEquipment(addEquipment: AddEquipment): Observable<any>{
    return this.http.put(
      API_EQ_URL + '/update',
      JSON.stringify(addEquipment),
      this.httpOptions
    );
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

  getPropertiesOfType(typeId: string): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_EQTYPE_URL + '/getpropertiesoftype/' + typeId,
      this.httpOptions
    );
  }

  getEquipmentToUpdate(equipmentId: string): Observable<AddEquipment>{
    return this.http.get<AddEquipment>(
      API_EQ_URL + '/getequipmenttoupdate/' + equipmentId,
      this.httpOptions
    );
  } 

  getEquipmentSimplified(filter: EquipmentFilter): Observable<any>{
    console.log(filter);

    return this.http.post(
      API_EQ_URL + '/GetSimplified',
      JSON.stringify(filter),
      this.httpOptions 
    );      
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
  
  detach(childId: string): Observable<any> {
    return this.http.get(
      API_EQ_URL + '/detach/' + childId,
      this.httpOptions
    );
  }
  
  attach(model: AttachEquipment): Observable<any>{
    return this.http.post(
      API_EQ_URL + '/attach',
      JSON.stringify(model),
      this.httpOptions
    );
  }

  getEquipmentOfDefinitions(definitionIds: string[]): Observable<IOption[]>{
    let params = new HttpParams();
    definitionIds.forEach(id => {
      params = params.append('definitionIds', id);
    })

    params = params.append('onlyDeatachedEquipment', "true");

    return this.http.get<IOption[]>(
      API_EQ_URL + '/getEquipmentOfDefinitions',
      {params: params}
    );
  }

  getUploadOptions = (): HttpHeaders => {
    const headers = new HttpHeaders();
    headers.set('Accept', 'application/json');
    headers.delete('Content-Type');
    return headers;
  }

  bulkUpload(formData): Observable<any>{
    

    return this.http.post(
      API_EQ_URL + '/bulkupload',
      formData,
      { headers: this.getUploadOptions() }
    );
  }
}
