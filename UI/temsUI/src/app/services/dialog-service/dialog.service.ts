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

  openDialog(component: ComponentType<any>, keyValue?: IOption[], afterClosed?: Function){
    
    let data = {
      dialogRef: this.dialogRef, 
    };

    if(keyValue != undefined){
      keyValue.forEach(element => {
        data[element.label] = element.value;
      });
    }

    this.dialogRef = this.dialog.open(component,
    {
      maxHeight: '80vh',
      autoFocus: false,
      data: data
    });

    this.dialogRef.componentInstance["dialogRef"] = this.dialogRef;
    
    if(afterClosed != undefined){
      this.dialogRef.afterClosed().subscribe(result => {
        afterClosed();
      })
    }
  }
}