import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ViewType, EquipmentType } from './../../../models/equipment/view-type.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Component, Inject, Input, OnInit, Optional, Output } from '@angular/core';
import EventEmitter from 'events';
import { ViewPropertyComponent } from '../view-property/view-property.component';

@Component({
  selector: 'app-view-type',
  templateUrl: './view-type.component.html',
  styleUrls: ['./view-type.component.scss']
})
export class ViewTypeComponent extends TEMSComponent implements OnInit {

  @Input() typeId: string;
  @Output() viewRelatedType = new EventEmitter();
  type = new EquipmentType();

  constructor(
    private equipmentService: EquipmentService,
    private dialog: MatDialog,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();
    
    if(this.dialogData != undefined){
      this.typeId = dialogData.typeId;
    }
   }

  ngOnInit(): void {
    this.subscriptions.push(
      this.equipmentService.getFullType(this.typeId)
      .subscribe(result => {
        console.log(result);
        this.type = result;
      })
    )
  }

  viewProperty(propertyId: string){
    let dialogRef: MatDialogRef<any>;

    dialogRef = this.dialog.open(ViewPropertyComponent,
      {
        maxHeight: '80vh',
        autoFocus: false
      });

    dialogRef.componentInstance.propertyId = propertyId;
    dialogRef.afterClosed().subscribe(result => {
    })
  }

  viewType(typeId: string){
    let dialogRef: MatDialogRef<any>;

    dialogRef = this.dialog.open(ViewTypeComponent,
      {
        maxHeight: '80vh',
        autoFocus: false
      });

    dialogRef.componentInstance.typeId = typeId;
    dialogRef.afterClosed().subscribe(result => {
    })
  }

}

// TODO: Extract methods that display modals to another file