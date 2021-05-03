import { IOption } from './../../models/option.model';
import { ComponentType } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  dialogRef: MatDialogRef<any>;

  constructor(
    private dialog: MatDialog,
  ) { 

  }

  openDialog(component: ComponentType<any>, keyValue?: IOption[], afterClosed?: Function, beforeClosed?: Function){
    this.dialogRef = this.dialog.open(component,
      {
        maxHeight: '80vh',
        autoFocus: false
      });

    if(keyValue != undefined){
      keyValue.forEach(element => {
        this.dialogRef.componentInstance[element.label] = element.value;
      });
    }

    this.dialogRef.componentInstance["dialogRef"] = this.dialogRef;

    // this.dialogRef.beforeClosed().subscribe(result => {
    //   beforeClosed();
    // })

    this.dialogRef.afterClosed().subscribe(result => {
      if(afterClosed != undefined)
        afterClosed();
    })
  }
}