import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { TEMSService } from '../tems-service/tems.service';
import { API_EQTYPE_URL } from 'src/app/models/backend.config';

@Injectable({
  providedIn: 'root'
})
export class TypeService extends TEMSService{

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  getAllAutocompleteOptions(filter?: string): Observable<IOption[]> {
    let endPoint = API_EQTYPE_URL + '/getallautocompleteoptions';
    if(filter != undefined)
      endPoint += '/' + filter;
     
    return this.http.get<IOption[]>(
      endPoint,
      this.httpOptions
      );
  }
}
