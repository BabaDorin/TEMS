import { TEMSService } from './../tems-service/tems.service';
import { API_PERS_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddPersonnel } from 'src/app/models/personnel/add-personnel.model';
import { IOption } from 'src/app/models/option.model';
import { ViewPersonnel } from './../../models/personnel/view-personnel.model';
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

  getAllAutocompleteOptions(): Observable<any>{
    return this.http.get(
      API_PERS_URL + '/getallautocompleteoptions',
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

  getPersonnelById(personnelId: string): ViewPersonnel{
    return new ViewPersonnel();
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
}
