import { Component, Inject, Input, OnInit, Optional, Output } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import EventEmitter from 'events';
import { ViewPropertyComponent } from 'src/app/tems-components/equipment/view-property/view-property.component';
import { EquipmentType } from './../../../models/equipment/view-type.model';
import { DialogService } from './../../../services/dialog.service';
import { EquipmentService } from './../../../services/equipment.service';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-view-type',
  templateUrl: './view-type.component.html',
  styleUrls: ['./view-type.component.scss']
})
export class ViewTypeComponent extends TEMSComponent implements OnInit {

  @Input() typeId: string;
  @Output() viewRelatedType = new EventEmitter();
  type = new EquipmentType();

  dialogRef;

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService,
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
        this.type = result;
      })
    )
  }

  viewProperty(propertyId: string){
    this.dialogService.openDialog(
      ViewPropertyComponent,
      [{ label: 'propertyId', value: propertyId}]
    )
  }

  viewType(typeId: string){
    this.dialogService.openDialog(
      ViewTypeComponent,
      [{ label: 'typeId', value: typeId}]
    )
  }
}