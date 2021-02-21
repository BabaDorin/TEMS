import { EquipmentAllocationComponent } from './../../equipment-allocation/equipment-allocation.component';
import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';
import { ViewEquipmentAllocation } from './../../../../models/equipment/view-equipment-allocation.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-equipment-details-allocations',
  templateUrl: './equipment-details-allocations.component.html',
  styleUrls: ['./equipment-details-allocations.component.scss']
})
export class EquipmentDetailsAllocationsComponent implements OnInit {

  allocations: ViewEquipmentAllocation[];
  @Input() equipment: ViewEquipmentSimplified; 

  constructor(
    private equipmentService: EquipmentService,
    public dialog: MatDialog
  ) {
    console.log('EquipmentDetailsAllocationsComponent');
  }

  ngOnInit(): void {
    this.allocations = this.equipmentService.getEquipmentAllocations(this.equipment.id);
    console.log(this.equipment);
  }

  allocate(){
    this.openCreateAllocationDialog();
  }

  openCreateAllocationDialog(): void {
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(EquipmentAllocationComponent); 
    dialogRef.componentInstance.equipment = [ 
      {
        id: this.equipment.id,
        value: this.equipment.temsidOrSn
      }];

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

}
