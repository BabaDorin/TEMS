import { API_ANN_URL } from '../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './tems.service';
import { AddAnnouncement } from '../models/communication/announcement/add-announcement.model';
import { Observable } from 'rxjs';
import { ViewAnnouncement } from '../models/communication/announcement/view-announcement.model';
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

  getAnnouncements(skip?: number, take?: number): Observable<ViewAnnouncement[]>{
    let endPoint = API_ANN_URL + '/get';
    if(skip != undefined)
      endPoint += '/' + skip + '/' + take;

    return this.http.get<ViewAnnouncement[]>(
      endPoint,
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
  
  removeAnnouncement(announcementId: string): Observable<any>{
    return this.http.delete(
      API_ANN_URL + '/remove/' + announcementId,
      this.httpOptions
    );
  }
}
