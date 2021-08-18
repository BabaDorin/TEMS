import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { TEMSService } from './tems.service';
import { API_EQTYPE_URL } from 'src/app/models/backend.config';

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
      API_EQTYPE_URL + '/remove/' + typeId,
      this.httpOptions
    );
  }

  getAllAutocompleteOptions(filter?: string, includeChildren: boolean = true): Observable<IOption[]> {
    let endPoint = API_EQTYPE_URL + '/getallautocompleteoptions';
    filter == undefined || filter == '' ? endPoint += '/ ' : endPoint += '/' + filter;

    endPoint += '/' + includeChildren;
     
    return this.http.get<IOption[]>(
      endPoint,
      this.httpOptions
      );
  }

  getFullType(typeId: string): Observable<any> {
    return this.http.post(API_EQTYPE_URL + '/fulltype', JSON.stringify(typeId), this.httpOptions);
  }
}
