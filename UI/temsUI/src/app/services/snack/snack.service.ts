import { MatSnackBar } from '@angular/material/snack-bar';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SnackService {

  constructor(
    private _snack: MatSnackBar
  ) { }

  snack(response, sclass?){
    if(response.status == undefined)
      return false;
    
    let panelClass = (response.status == 1) ? 'success-snackbar' : 'error-snackbar';
    if(sclass != undefined) panelClass = sclass;

    let seconds = (response.status == 1) ? 3 : 10;


    this._snack.open(response.message, 'Ok', {
      duration: seconds * 1000,
      panelClass: [panelClass, 'text-center']
    });

    return response.status == 1;
  }

  // returns true if there is something to display in snack
  snackIfError(response): boolean{
    if(response.status == undefined || response.status == 1)
      return false;

    this.snack(response);
    return true;
  }
}
