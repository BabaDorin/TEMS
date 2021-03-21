import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor() { }

  hasClaim(claim: string): boolean{
    let token = localStorage.getItem('token');
    if(token == null) return false;

    return JSON.parse(window.atob(token.split('.')[1]))[claim] != undefined;
  }
}
