import { IOption } from './../../../models/option.model';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { AgGridEquipmentComponent } from './../ag-grid-equipment/ag-grid-equipment.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { EquipmentAllocationComponent } from '../equipment-allocation/equipment-allocation.component';

@Component({
  selector: 'app-view-equipment',
  templateUrl: './view-equipment.component.html',
  styleUrls: ['./view-equipment.component.scss']
})
export class ViewEquipmentComponent implements OnInit {

  @ViewChild('agGridEquipment') agGridEquipment: AgGridEquipmentComponent;

  constructor(
    public dialog: MatDialog
  ) {

  }

  ngOnInit(): void {

  }

  allocateSelected() {
    let selectedNodes = (this.agGridEquipment.getSelectedNodes() as ViewEquipmentSimplified[])
      .map(node => ({value: node.id, label: node.temsIdOrSerialNumber} as IOption));

    if (selectedNodes.length == 0)
      return;

    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(EquipmentAllocationComponent);

    dialogRef.componentInstance.equipment = selectedNodes;

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
}
