import { MatSnackBar } from '@angular/material/snack-bar';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SnackService {

  statusPanelClasses = {
    "0": "error-snackbar", // Fail
    "1": "success-snackbar", // Success
    "2": "default-snackbar" // Neutral
  }
  
  statusDisplayTimes = {
    "0": 10, // Fail
    "1": 3, // Success
    "2": 5 // Neutral
  }

  constructor(
    private _snack: MatSnackBar
  ) { }

  private sanitarizeResponse(response){
    // Sometimes, the API might return an http 500 response. In that case,
    // the response model itself is placed within the 'error' property of the response.
    if(response?.error != undefined){
      return response.error;
    }

    return response;
  }

  snack(response, sclass?){
    response = this.sanitarizeResponse(response);

    if(response.status == undefined)
      return false;
    
    let panelClass = this.statusPanelClasses[response.status];
    let seconds = this.statusDisplayTimes[response.status];

    this._snack.open(response.message, 'Ok', {
      duration: seconds * 1000,
      panelClass: [sclass ?? panelClass, 'text-center']
    });
  }

  // returns true if there is something to display in snack
  snackIfError(response): boolean{
    response = this.sanitarizeResponse(response);

    if(response == undefined || response.status == undefined || response.status == 1)
      return false;

    this.snack(response);
    return true;
  }
}
