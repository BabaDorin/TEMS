import { IOption } from './../../models/option.model';
import { ComponentType } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  constructor(
    private dialog: MatDialog,
  ) { 

  }

  openDialog(component: ComponentType<any>, keyValue?: IOption[], afterClosed?: Function){
    let dialogRef: MatDialogRef<any>;

    dialogRef = this.dialog.open(component,
      {
        maxHeight: '80vh',
        autoFocus: false
      });

    if(keyValue != undefined){
      keyValue.forEach(element => {
        dialogRef.componentInstance[element.label] = element.value;
      });
    }

    dialogRef.componentInstance["dialogRef"] = dialogRef;

    dialogRef.afterClosed().subscribe(result => {
      if(afterClosed != undefined)
        afterClosed();
    })
  }
}