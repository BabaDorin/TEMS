import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { TEMSService } from './tems.service';
import { API_ASSET_TYPE_URL } from 'src/app/models/backend.config';

@Injectable({
  providedIn: 'root'
})
export class TypeService extends TEMSService {
  typeService: Observable<IOption[]>;

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  remove(typeId: string): Observable<any>{
    return this.http.delete(
      API_ASSET_TYPE_URL + '/remove/' + typeId,
      this.httpOptions
    );
  }

  getAllAutocompleteOptions(filter?: string, includeArchived: boolean = false): Observable<IOption[]> {
    const url = `${API_ASSET_TYPE_URL}?includeArchived=${includeArchived}`;
    const filterText = (filter ?? '').trim().toLowerCase();

    return this.http
      .get<{ assetTypes: Array<{ id: string; name: string; description: string; isArchived: boolean }> }>(url, this.httpOptions)
      .pipe(
        map(response => {
          const types = response.assetTypes ?? [];
          const activeTypes = includeArchived ? types : types.filter(t => !t.isArchived);
          const options = activeTypes.map(t => ({ value: t.id, label: t.name, description: t.description } as IOption));
          if (!filterText) return options;
          return options.filter(o => o.label.toLowerCase().includes(filterText));
        })
      );
  }

  getFullType(typeId: string): Observable<any> {
    return this.http.post(API_ASSET_TYPE_URL + '/fulltype', JSON.stringify(typeId), this.httpOptions);
  }

  getPropertiesOfType(typeId: string): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_ASSET_TYPE_URL + '/getpropertiesoftype/' + typeId,
      this.httpOptions
    );
  }
}
