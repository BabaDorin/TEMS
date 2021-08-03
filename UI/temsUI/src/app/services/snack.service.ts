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
    "2": 10 // Neutral
  }

  constructor(
    private _snack: MatSnackBar
  ) { }

  private sanitarizeResponse(response){
    // Sometimes, the API might return a http 500 response. In that case,
    // the response model ({message, status, additional}) is placed within the 'error' property of the response.

    if(response?.error != undefined){
      return response.error;
    }

    return response;
  }

  snack(response, sclass?){
    let responseModel = this.sanitarizeResponse(response);

    if(responseModel.status == undefined)
      return false;
    
    let panelClass = this.statusPanelClasses[responseModel.status];
    let seconds = this.statusDisplayTimes[responseModel.status];

    this._snack.open(responseModel.message, 'Ok', {
      duration: seconds * 1000,
      panelClass: [sclass ?? panelClass, 'text-center']
    });
  }

  // returns true if there is something to display in snack
  snackIfError(response): boolean{
    let responseModel = this.sanitarizeResponse(response);

    if(responseModel == undefined || responseModel.status == undefined || responseModel.status == 1)
      return false;

    this.snack(responseModel);
    return true;
  }
}
