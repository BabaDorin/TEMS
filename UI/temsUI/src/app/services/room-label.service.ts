import { SnackService } from './snack/snack.service';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { TEMSService } from './tems-service/tems.service';
import { Injectable } from '@angular/core';
import { IOption } from '../models/option.model';
import { of } from 'rxjs/internal/observable/of';
import { API_ROOM_URL } from '../models/backend.config';

@Injectable({
  providedIn: 'root'
})
export class RoomLabelService extends TEMSService {

  constructor(
    private http: HttpClient,
    private snackService: SnackService
  ) {
    super();
  }

  private roomLabels = []; 

  getAllAutocompleteOptions(filter?: string): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_ROOM_URL + '/getlabels',
      this.httpOptions
    );
  }
}
