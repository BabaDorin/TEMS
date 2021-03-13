import { API_ANN_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './../tems-service/tems.service';
import { AddAnnouncement } from './../../models/communication/announcement/add-announcement.model';
import { Observable } from 'rxjs';
import { ViewAnnouncement } from './../../models/communication/announcement/view-announcement.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommunicationService extends TEMSService {
  
  constructor(
    private http: HttpClient
  ) { 
    super();
  }

  getAnnouncements(): Observable<ViewAnnouncement[]>{
    return this.http.get<ViewAnnouncement[]>(
      API_ANN_URL + '/get',
      this.httpOptions
    );
  }

  createAnnouncement(addAnnouncement: AddAnnouncement): Observable<any>{
    return this.http.post(
      API_ANN_URL + '/create',
      JSON.stringify(addAnnouncement),
      this.httpOptions
    );
  }
}
