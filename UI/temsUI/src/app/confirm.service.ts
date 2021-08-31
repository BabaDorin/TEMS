import { ConfirmComponent } from './tems-components/confirm/confirm.component';
import { DialogService } from './services/dialog.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  constructor(private dialogService: DialogService) { }

  // Provide a message and an action to be completed if the user confirms.
  // Also, the method returns the confirmation flag (true / false);
  confirm(message: string, action?): Promise<Boolean> {
    return new Promise((resolve) => {
      let confirmed = false;

      let dialogRef = this.dialogService.openDialog(
        ConfirmComponent,
        [ { label: 'message', value: message } ]);
  
      let subscription = (dialogRef.componentInstance as ConfirmComponent).optionSelected
        .subscribe((confirmed) => {
          if(!confirmed)
            resolve(false);
          else
          {
            if(action != undefined) 
              action(); 
            subscription.unsubscribe(); 
            resolve(true);  
          }
      });
    });
  }
}
