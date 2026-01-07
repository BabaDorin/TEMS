import { Observable } from 'rxjs';
import { IOption } from '../models/option.model';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './tems.service';
import { Injectable } from '@angular/core';
import { API_ASSET_DEFINITION_URL } from 'src/app/models/backend.config';

@Injectable({
  providedIn: 'root'
})
export class DefinitionService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  remove(definitionId: string): Observable<any>{
    return this.http.delete(
      API_ASSET_DEFINITION_URL + '/remove/' + definitionId,
      this.httpOptions    
    )
  }

  getAllAutocompleteOptions(filter?: string, types?: string[]): Observable<IOption[]>{

    return this.http.post<IOption[]>(
      API_ASSET_DEFINITION_URL + '/getdefinitionsoftypes',
      JSON.stringify({filter: filter, typeIds: types}),
      this.httpOptions
    );
  }

  getDefinitionsOfType(typeId: string): Observable<any> {
    return this.http.get(
      API_ASSET_DEFINITION_URL + '/getdefinitionsoftype/' + typeId, 
      this.httpOptions
    );
  }

  getDefinitionsOfTypes(typeIds: string[]): Observable<IOption[]>{
    return this.http.post<IOption[]>(
      API_ASSET_DEFINITION_URL + '/getdefinitionsoftypes',
      JSON.stringify(typeIds),
      this.httpOptions
    );
  }
}
