import { MatSnackBar } from '@angular/material/snack-bar';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SnackService {

  constructor(
    private _snack: MatSnackBar
  ) { }

  snack(response){
    if(response.status == undefined)
      return false;
    
    let panelClass = (response.status == 1) ? 'success-snackbar' : 'error-snackbar';
    let seconds = (response.status == 1) ? 3 : 10;

    this._snack.open(response.message, 'Ok', {
      duration: seconds * 1000,
      panelClass: [panelClass]
    });

    return response.status == 1;
  }
}
