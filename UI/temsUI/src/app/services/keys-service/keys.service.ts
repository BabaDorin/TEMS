import { Injectable } from '@angular/core';
import { ViewKeySimplified } from 'src/app/models/key/view-key.model';

@Injectable({
  providedIn: 'root'
})
export class KeysService {

  getKeys(){
    return [
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
      new ViewKeySimplified(),
    ]
  }

}
