import { API_LBR_URL } from './../../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './../tems-service/tems.service';
import { Observable } from 'rxjs';
import { ViewLibraryItem } from './../../models/library/view-library-item.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LibraryService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  getItems(): Observable<ViewLibraryItem[]>{
    return this.http.get<ViewLibraryItem[]>(
      API_LBR_URL + '/getlibraryitems',
      this.httpOptions
    );
  }

  uploadFile(formData): Observable<any>{
    return this.http.post(
      API_LBR_URL + '/uploadfile', 
      formData, 
      { 
        reportProgress: true, 
        observe: 'events', 
      });
  }

  cancelThread(index):Observable<any>{
    return this.http.get(
      API_LBR_URL + '/cancel',
      this.httpOptions
    );
  }
}
