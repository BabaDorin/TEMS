import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
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

  getDefinitionsOfType(typeId: string, includeArchived: boolean = false): Observable<IOption[]> {
    const url = `${API_ASSET_DEFINITION_URL}?includeArchived=${includeArchived}`;

    return this.http
      .get<{ assetDefinitions: Array<{ id: string; name: string; assetTypeId: string; model?: string; manufacturer?: string; isArchived: boolean }> }>(url, this.httpOptions)
      .pipe(
        map(response => {
          const defs = response.assetDefinitions ?? [];
          const byType = defs.filter(d => d.assetTypeId === typeId);
          const active = includeArchived ? byType : byType.filter(d => !d.isArchived);
          return active.map(d => ({
            value: d.id,
            label: d.name,
            description: d.model || d.manufacturer || ''
          } as IOption));
        })
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
