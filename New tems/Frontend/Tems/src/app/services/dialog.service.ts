import { IOption } from '../models/option.model';
import { ComponentType } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  // public dialogRef: MatDialogRef<any>;

  constructor(
    private dialog: MatDialog,
  ) { 

  }

  openDialog(component: ComponentType<any>, keyValue?: IOption[], afterClosed?: Function): MatDialogRef<any> {
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
      maxHeight: '80vh',
      autoFocus: false,
      data: data
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
