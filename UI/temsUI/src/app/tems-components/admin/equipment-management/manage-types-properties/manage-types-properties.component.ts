import { ViewTypeComponent } from './../../../equipment/view-type/view-type.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ViewPropertySimplified } from './../../../../models/equipment/view-property-simplified.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { ViewTypeSiplified } from './../../../../models/equipment/view-type-simplified.model';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { ViewPropertyComponent } from 'src/app/tems-components/equipment/view-property/view-property.component';

@Component({
  selector: 'app-manage-types-properties',
  templateUrl: './manage-types-properties.component.html',
  styleUrls: ['./manage-types-properties.component.scss']
})
export class ManageTypesPropertiesComponent extends TEMSComponent implements OnInit {

  types: ViewTypeSiplified[];
  properties: ViewPropertySimplified[];

  constructor(
    private equipmentService: EquipmentService,
    private dialog: MatDialog
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.equipmentService.getTypesSimplified()
      .subscribe(result => {
        console.log(result);
        this.types = result;
      })
    );

    this.subscriptions.push(
      this.equipmentService.getPropertiesSimplified()
      .subscribe(result => {
        console.log(result);
        this.properties = result;
      })
    )
  }

  viewType(typeId: string){
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(ViewTypeComponent,
      {
        maxHeight: '80vh',
        width: '40vh',
        autoFocus: false
      });

    dialogRef.componentInstance.typeId = typeId;
    dialogRef.componentInstance.dialogRef = dialogRef;

    dialogRef.afterClosed().subscribe(result => {

    })
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
}
