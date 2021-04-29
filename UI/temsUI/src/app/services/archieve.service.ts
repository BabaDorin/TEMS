import { ArchievedItem } from './../models/archieve/archieved-item.model';
import { Observable } from 'rxjs';
import { IOption } from './../models/option.model';
import { API_ARCH_URL } from './../models/backend.config';
import { TEMSService } from './tems-service/tems.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ArchieveService extends TEMSService {

  constructor(
    private http: HttpClient
  ) {
    super();
  }

  getArchievedItems(itemType: string): Observable<ArchievedItem[]>{
    return this.http.get<ArchievedItem[]>(
      API_ARCH_URL + '/getarchieveditems/' + itemType,
      this.httpOptions
    );
  }

  setArchivationStatus(itemType: string, itemId: string, status: boolean): Observable<any>{
    return this.http.get(
      API_ARCH_URL + '/setArchivationStatus/' + itemType + '/' + itemId + '/' + status,
      this.httpOptions 
    );
  }
}
