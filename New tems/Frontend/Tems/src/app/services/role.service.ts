import { ViewRole } from '../models/roles/role.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TEMSService } from './tems.service';
import { Observable } from 'rxjs';
import { API_ROLE_URL } from 'src/app/models/backend.config';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  getRoles(): Observable<ViewRole[]>
  {
    return this.http.get<ViewRole[]>(
      API_ROLE_URL + '/getRoles',
      this.httpOptions
    );
  }
}
