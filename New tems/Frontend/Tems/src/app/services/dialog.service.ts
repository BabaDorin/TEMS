import { IOption } from '../models/option.model';
import { ComponentType } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  // public dialogRef: MatDialogRef<any>;

  constructor(
    private dialog: MatDialog,
  ) { 

  }

  openDialog(
    component: ComponentType<any>,
    keyValue?: IOption[],
    afterClosed?: Function,
    configOverrides?: MatDialogConfig
  ): MatDialogRef<any> {
    let dialogRef: MatDialogRef<any>;
    
    let data = {
      dialogRef: dialogRef, 
    };

    if(keyValue != undefined){
      keyValue.forEach(element => {
        data[element.label] = element.value;
      });
    }

    dialogRef = this.dialog.open(component,
    {
      width: '520px',
      maxWidth: '95vw',
      maxHeight: '80vh',
      autoFocus: false,
      panelClass: 'custom-dialog-container',
      data: data,
      ...configOverrides
    });

    dialogRef.componentInstance["dialogRef"] = dialogRef;
    
    if(afterClosed != undefined){
      dialogRef.afterClosed().subscribe(() => {
        afterClosed();
      })
    }

    return dialogRef;
  }
}
