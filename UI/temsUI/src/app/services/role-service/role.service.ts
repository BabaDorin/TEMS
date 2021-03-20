import { API_ROLE_URL } from './../../models/backend.config';
import { IOption } from './../../models/option.model';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './../tems-service/tems.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  getAllAutocompleteOptions(): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_ROLE_URL + '/getallautocompleteoptions', 
      this.httpOptions
    );
  }
}
