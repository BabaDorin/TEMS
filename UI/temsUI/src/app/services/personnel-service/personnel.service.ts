import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PersonnelService {

  constructor() { }

  getAllAutocompleteOptions(){
    return [
      {id: '1', value:'Baba Dori'},
      {id: '2', value:'Vasile Versace'},
      {id: '3', value:'Ciuta Johnny'},
    ]
  }
}
