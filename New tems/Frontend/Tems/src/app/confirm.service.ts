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
  confirm(message: string): Promise<Boolean> {
    return new Promise((resolve) => {
      let dialogRef = this.dialogService.openDialog(
        ConfirmComponent,
        [ { label: 'message', value: message } ]);
  
      dialogRef.afterClosed().subscribe(result => {
        resolve(result === true);
      });
    });
  }

  // if message is confirmed, run the specified action
  async confirmThenAct(message: string, action) {
    if(action == undefined)
      return;

    if(await confirm(message)){
      action();
    }
  }
}
