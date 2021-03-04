import { IOption } from 'src/app/models/option.model';
import { ViewPersonnel } from './../../models/personnel/view-personnel.model';
import { Injectable } from '@angular/core';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';

@Injectable({
  providedIn: 'root'
})
export class PersonnelService {

  constructor() { }

  getAllAutocompleteOptions(): IOption[]{
    return [
      {value: '1', label:'Baba Dori'},
      {value: '2', label:'Vasile Versace'},
      {value: '3', label:'Ciuta Johnny'},
    ]
  }

  getPersonnelSimplified(personnelId: string): ViewPersonnelSimplified{
    return new ViewPersonnelSimplified();
  }

  getPersonnel(): ViewPersonnelSimplified[]{
    return [
      new ViewPersonnelSimplified(),
      new ViewPersonnelSimplified(),
      new ViewPersonnelSimplified(),
      new ViewPersonnelSimplified(),
      new ViewPersonnelSimplified(),
    ]
  }

  getPersonnelById(personnelId: string): ViewPersonnel{
    return new ViewPersonnel();
  }
}
