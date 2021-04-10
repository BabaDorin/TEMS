import { Observable } from 'rxjs';
import { IOption } from './../../models/option.model';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './../tems-service/tems.service';
import { Injectable } from '@angular/core';
import { API_EQDEF_URL } from 'src/app/models/backend.config';

@Injectable({
  providedIn: 'root'
})
export class DefinitionService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  getAllAutocompleteOptions(filter?: string, types?: string[]): Observable<IOption[]>{

    return this.http.post<IOption[]>(
      API_EQDEF_URL + '/getdefinitionsoftypes',
      JSON.stringify({filter: filter, typeIds: types}),
      this.httpOptions
    );
  }

  getDefinitionsOfType(typeId: string): Observable<any> {
    return this.http.get(
      API_EQDEF_URL + '/getdefinitionsoftype/' + typeId, 
      this.httpOptions
    );
  }

  getDefinitionsOfTypes(typeIds: string[]): Observable<IOption[]>{
    return this.http.post<IOption[]>(
      API_EQDEF_URL + '/getdefinitionsoftypes',
      JSON.stringify(typeIds),
      this.httpOptions
    );
  }
}
