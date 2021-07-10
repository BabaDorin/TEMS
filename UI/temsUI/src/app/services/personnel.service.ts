import { IOption } from '../models/option.model';
import { TEMSService } from './tems.service';
import { API_PERS_URL } from '../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddPersonnel } from 'src/app/models/personnel/add-personnel.model';
import { ViewPersonnel } from '../models/personnel/view-personnel.model';
import { Injectable } from '@angular/core';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';

@Injectable({
  providedIn: 'root'
})
export class PersonnelService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  remove(personnelId: string): Observable<any>{
    return this.http.delete(
      API_PERS_URL + '/remove/' + personnelId,
      this.httpOptions
    );
  }

  getAllAutocompleteOptions(filter?: string): Observable<IOption[]>{
    let endPoint = API_PERS_URL + '/getallautocompleteoptions';
    if(filter != undefined)
      endPoint += '/' + filter;

    return this.http.get<IOption[]>(
      endPoint,
      this.httpOptions
    );
  } 

  getPersonnelToUpdate(personnelId: string): Observable<AddPersonnel>{
    return this.http.get<AddPersonnel>(
      API_PERS_URL + '/getpersonneltoupdate/' + personnelId,
      this.httpOptions
    );
  }

  updatePersonnel(personnel: AddPersonnel): Observable<any>{
    return this.http.put(
      API_PERS_URL + '/update',
      JSON.stringify(personnel),
      this.httpOptions
    );
  }

  archievePersonnel(personnelId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved

    return this.http.get(
      API_PERS_URL + '/archieve/' + personnelId + '/' + archivationStatus,
      this.httpOptions
    );
  }

  getPersonnelSimplifiedById(personnelId: string): ViewPersonnelSimplified{
    return new ViewPersonnelSimplified();
  }

  getPersonnelSimplified(pageNumber: number, recordsPerPage: number): Observable<any>{
    return this.http.get(
      API_PERS_URL + '/getsimplified' + '/' + pageNumber + '/' + recordsPerPage,
      this.httpOptions
    );
  }

  getPersonnelById(personnelId: string): Observable<any> {
    return this.http.get(
      API_PERS_URL + '/getbyid/' + personnelId,
      this.httpOptions
    );
  }

  createPersonnel(addPersonnel: AddPersonnel): Observable<any>{
    return this.http.post(
      API_PERS_URL + '/create',
      addPersonnel,
      this.httpOptions
    )
  }

  getPersonnelPositions(): Observable<any>{
    return this.http.get(
      API_PERS_URL + '/getpositions',
      this.httpOptions
    );
  }

  getPersonnelSimplifiedFromPersonnel(personnel: ViewPersonnel): ViewPersonnelSimplified{
    return {
      id: personnel.id,
      name: personnel.name,
      isArchieved: personnel.isArchieved
    }
  }
}
