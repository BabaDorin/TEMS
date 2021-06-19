import { EmailSenderCredentials } from './../models/system-configuration/emai-sender.model';
import { API_SYSCONF_URL } from './../models/backend.config';
import { Observable } from 'rxjs';
import { TEMSService } from './tems-service/tems.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_EQ_URL } from '../models/backend.config';
import { AppSettings } from '../models/system-configuration/app-settings.model';

@Injectable({
  providedIn: 'root'
})
export class SystemConfigurationService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  integrateSIC(): Observable<any>{
    return this.http.get(
      API_SYSCONF_URL + '/integratesic',
      this.httpOptions
    );
  }

  setLibraryAllocateStorageSpace(gb: number): Observable<any>{
    return this.http.get(
      API_SYSCONF_URL + "/setlibraryallocatedstoragespace/" + gb,
      this.httpOptions
    );
  }

  setEmailSender(emailSenderCredentials: EmailSenderCredentials): Observable<any>{
    return this.http.post(
      API_SYSCONF_URL + '/setemailsender',
      emailSenderCredentials,
      this.httpOptions
    );
  }

  setRoutineCheckInterval(hours:  number): Observable<any>{
    return this.http.get(
      API_SYSCONF_URL + '/setroutinecheckinterval/' + hours,
      this.httpOptions
    );
  }

  setArchieveInterval(hours:  number): Observable<any>{
    return this.http.get(
      API_SYSCONF_URL + '/setarchieveinterval/' + hours,
      this.httpOptions
    );
  }

  getAppSettingsModel(): Observable<AppSettings>{
    return this.http.get<AppSettings>(
      API_SYSCONF_URL + '/getappsettingsmodel',
      this.httpOptions
    );
  }

  setLibraryPassword(newPass: string): Observable<any>{
    return this.http.get(
      API_SYSCONF_URL + '/setlibrarypassword/' + newPass, // library pass isn't something very very confidential
      this.httpOptions
    );
  }

  guestTicketCreationAllowanceChanged(flag: boolean): Observable<any>{
    return this.http.get(
      API_SYSCONF_URL + '/SetGuestTicketCreationAllowance/' + flag,
      this.httpOptions
    );
  }
}
