import { TypeService } from './../../../services/type.service';
import { Component, Inject, Input, OnInit, Optional, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import EventEmitter from 'events';
import { ViewPropertyComponent } from 'src/app/tems-components/equipment/view-property/view-property.component';
import { EquipmentType } from './../../../models/equipment/view-type.model';
import { DialogService } from './../../../services/dialog.service';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-view-type',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule
  ],
  templateUrl: './view-type.component.html',
  styleUrls: ['./view-type.component.scss']
})
export class ViewTypeComponent extends TEMSComponent implements OnInit {

  @Input() typeId: string;
  @Output() viewRelatedType = new EventEmitter();
  type = new EquipmentType();

  dialogRef;

  constructor(
    private typeService: TypeService,
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
      this.typeService.getFullType(this.typeId)
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