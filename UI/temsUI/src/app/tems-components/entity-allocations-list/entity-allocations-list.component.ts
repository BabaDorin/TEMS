import { TEMSComponent } from './../../tems/tems.component';
import { AllocationService } from './../../services/allocation-service/allocation.service';
import { ViewPersonnelSimplified } from './../../models/personnel/view-personnel-simplified.model';
import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit, Input } from '@angular/core';
import { ViewAllocationSimplified,} from 'src/app/models/equipment/view-equipment-allocation.model';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { EquipmentAllocationComponent } from '../equipment/equipment-allocation/equipment-allocation.component';

@Component({
  selector: 'app-entity-allocations-list',
  templateUrl: './entity-allocations-list.component.html',
  styleUrls: ['./entity-allocations-list.component.scss']
})
export class EntityAllocationsListComponent extends TEMSComponent implements OnInit {

  allocations: ViewAllocationSimplified[];
  @Input() equipment: ViewEquipmentSimplified; 
  @Input() room: ViewRoomSimplified; 
  @Input() personnel: ViewPersonnelSimplified; 

  constructor(
    private allocationService: AllocationService,
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit(): void {
    if(this.equipment == undefined && this.room == undefined && this.personnel == undefined){
      console.warn('EntityAllocationsListComponent requires an entity in order to display logs');
      return;
    }

    if(this.equipment)
      this.subscriptions.push(this.allocationService.getEquipmentAllocations(this.equipment.id)
        .subscribe(result => {
          console.log(result);
          this.allocations = result;
        }));

    if(this.room)
      this.subscriptions.push(this.allocationService.getEquipmentAllocationsToRoom(this.room.id)
        .subscribe(result => {
          console.log(result);
          this.allocations = result;
        }));

    if(this.personnel)
      this.subscriptions.push(this.allocationService.getEquipmentAllocationsToPersonnel(this.personnel.id)
        .subscribe(result => {
          console.log(result);
          this.allocations = result;
        }));
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
