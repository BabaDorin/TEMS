import { ViewLibraryItem } from './../../models/library/view-library-item.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LibraryService {

  constructor() { }

  getItems(){
    return [
      new ViewLibraryItem(),
      new ViewLibraryItem(),
      new ViewLibraryItem(),
      new ViewLibraryItem(),
      new ViewLibraryItem(),
    ]
  }
}
