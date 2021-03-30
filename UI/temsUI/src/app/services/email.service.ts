import { API_EMAIL_URL } from './../models/backend.config';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { TEMSService } from './tems-service/tems.service';
import { Injectable } from '@angular/core';
import { ISendEmail } from '../models/email/send-email.model';

@Injectable({
  providedIn: 'root'
})
export class EmailService extends TEMSService {

  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  sendEmail(model: ISendEmail): Observable<any>{
    return this.http.post(
      API_EMAIL_URL + '/sendEmail',
      JSON.stringify(model),
      this.httpOptions
    );
  }
}
