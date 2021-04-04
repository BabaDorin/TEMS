import { HttpClient } from '@angular/common/http';
import { TEMSService } from './tems-service/tems.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_AUTH_URL } from '../models/backend.config';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  isLoggedIn(): Observable<any>{
    return this.http.get(
      API_AUTH_URL + '/isauthenticated',
      this.httpOptions
    );
  }

  logIn(loginModel): Observable<any>{
    return this.http.post(
      API_AUTH_URL + '/login',
      JSON.stringify(loginModel),
      this.httpOptions
    );
  }

  signOut(): Observable<any>{
    let token = localStorage.getItem('token');
    localStorage.removeItem('token');
    return this.http.post(
      API_AUTH_URL + "/signout/",
      JSON.stringify(token),
      this.httpOptions
    );
  }
}
