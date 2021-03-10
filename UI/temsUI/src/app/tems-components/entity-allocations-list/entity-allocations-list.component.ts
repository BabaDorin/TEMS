import { AllocationService } from './../../services/allocation-service/allocation.service';
import { ViewPersonnelSimplified } from './../../models/personnel/view-personnel-simplified.model';
import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit, Input } from '@angular/core';
import { ViewEquipmentAllocation } from 'src/app/models/equipment/view-equipment-allocation.model';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { EquipmentAllocationComponent } from '../equipment/equipment-allocation/equipment-allocation.component';

@Component({
  selector: 'app-entity-allocations-list',
  templateUrl: './entity-allocations-list.component.html',
  styleUrls: ['./entity-allocations-list.component.scss']
})
export class EntityAllocationsListComponent implements OnInit {

  allocations: ViewEquipmentAllocation[];
  @Input() equipment: ViewEquipmentSimplified; 
  @Input() room: ViewRoomSimplified; 
  @Input() personnel: ViewPersonnelSimplified; 

  constructor(
    private allocationService: AllocationService,
    public dialog: MatDialog
  ) {

  }

  ngOnInit(): void {
    if(this.equipment == undefined && this.room == undefined && this.personnel == undefined){
      console.warn('EntityAllocationsListComponent requires an entity in order to display logs');
      return;
    }

    if(this.equipment)
      this.allocations = this.allocationService.getEquipmentAllocations(this.equipment.id);

    if(this.room)
      this.allocations = this.allocationService.getEquipmentAllocationsToRoom(this.room.id);

    if(this.personnel)
      this.allocations = this.allocationService.getEquipmentAllocationsToPersonnel(this.personnel.id);
  }

  addAllocation(): void {
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(EquipmentAllocationComponent); 
    
    if(this.equipment){
      dialogRef.componentInstance.equipment = [
        {
          value: this.equipment.id, 
          label: this.equipment.temsIdOrSerialNumber
        }];
    }

    if(this.room){
      dialogRef.componentInstance.room = [
        {
          value: this.room.id, 
          label: this.room.identifier
        }];
    }

    if(this.personnel){
      dialogRef.componentInstance.personnel = [
        {
          value: this.personnel.id, 
          label: this.personnel.name
        }];
    }

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
}
