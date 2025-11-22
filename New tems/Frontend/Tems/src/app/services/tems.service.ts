import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TEMSService {

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':'application/json; charset=utf-8'
    })
  }

  constructor() { }
}
