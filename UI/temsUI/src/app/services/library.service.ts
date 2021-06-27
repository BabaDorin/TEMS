import { API_LBR_URL } from '../models/backend.config';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './tems.service';
import { Observable } from 'rxjs';
import { ViewLibraryItem } from '../models/library/view-library-item.model';
import { Injectable } from '@angular/core';
import { Fraction } from 'src/app/models/analytics/fraction.model';

@Injectable({
  providedIn: 'root'
})
export class LibraryService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  getItems(accessPass?: string): Observable<ViewLibraryItem[]>{
    let endPoint = API_LBR_URL + '/getlibraryitems';
    if(accessPass != undefined)
      endPoint += '/' + accessPass;

    return this.http.get<ViewLibraryItem[]>(
      endPoint,
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

  downloadItem(itemId: string): Observable<any>{
    return this.http.get(
      API_LBR_URL + '/download/' + itemId,
      {
        reportProgress: true,
        responseType: 'blob',
      }
    );
  }

  removeItem(itemId: string): Observable<any>{
    return this.http.get(
      API_LBR_URL + '/remove/' + itemId,
      this.httpOptions
    );
  }
  
  formatBytes(bytes, decimals?) {
    if (bytes === 0) {
      return '0 Bytes';
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals || 2;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }

  getSpaceUsageData(accessPassword): Observable<Fraction>{
    let endPoint = API_LBR_URL + '/GetSpaceUsageData';
    if(accessPassword != undefined) 
      endPoint += "/" + accessPassword;

    return this.http.get<Fraction>(
      endPoint,
      this.httpOptions
    );
  }

  getAvailableLibraryStorageSpace(): Observable<number>{
    return this.http.get<number>(
      API_LBR_URL + '/getavailablelibrarystoragespace',
      this.httpOptions
    );
  }
}
