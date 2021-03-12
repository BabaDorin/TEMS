import { KeysAllocationsComponent } from 'src/app/tems-components/keys/keys-allocations/keys-allocations.component';
import { ViewKeySimplified } from 'src/app/models/key/view-key.model';
import { AgGridKeysComponent } from 'src/app/tems-components/keys/ag-grid-keys/ag-grid-keys.component';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AgGridEquipmentComponent } from '../../equipment/ag-grid-equipment/ag-grid-equipment.component';
import { IOption } from 'src/app/models/option.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AddKeyComponent } from '../add-key/add-key.component';

@Component({
  selector: 'app-view-keys',
  templateUrl: './view-keys.component.html',
  styleUrls: ['./view-keys.component.scss']
})
export class ViewKeysComponent extends TEMSComponent implements OnInit {

  @ViewChild('agGridKeys') agGridKeys: AgGridKeysComponent;
  unallocatedKeys: ViewKeySimplified[] = [];
  allocatedKeys: ViewKeySimplified[] = [];

  constructor(
    public dialog: MatDialog,
    private keysService: KeysService,
  ) { 
    super();
  }

  ngOnInit(): void {
    this.getKeys();
  }

  getKeys(){
    let keys = [] as ViewKeySimplified[];
    
    this.subscriptions.push(this.keysService.getKeys()
      .subscribe(result => {
        keys = result;

        this.unallocatedKeys = keys.filter(key => key.allocatedTo.value == "--");
        this.allocatedKeys = keys.filter(key => key.allocatedTo.value != "--");
      }));
  }

  allocateSelectedKeys(){
    let selectedNodes = (this.agGridKeys.getSelectedNodes() as ViewKeySimplified[])
      .map(node => ({value: node.id, label: node.identifier} as IOption));

    if (selectedNodes.length == 0)
      return;

    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(KeysAllocationsComponent);

    dialogRef.componentInstance.keysAlreadySelectedOptions = selectedNodes;

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      this.getKeys();
    });
  }

  addKey(){
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(AddKeyComponent);

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      this.getKeys();
    });
  }
}
