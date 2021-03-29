import { CAN_MANAGE_ENTITIES } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { SnackService } from './../../services/snack/snack.service';
import { TEMSComponent } from './../../tems/tems.component';
import { AllocationService } from './../../services/allocation-service/allocation.service';
import { ViewPersonnelSimplified } from './../../models/personnel/view-personnel-simplified.model';
import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit, Input } from '@angular/core';
import { ViewAllocationSimplified,} from 'src/app/models/equipment/view-equipment-allocation.model';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { EquipmentAllocationComponent } from '../equipment/equipment-allocation/equipment-allocation.component';
import { DialogService } from 'src/app/services/dialog-service/dialog.service';
import { Router } from '@angular/router';
import { IOption } from 'src/app/models/option.model';

@Component({
  selector: 'app-entity-allocations-list',
  templateUrl: './entity-allocations-list.component.html',
  styleUrls: ['./entity-allocations-list.component.scss']
})
export class EntityAllocationsListComponent extends TEMSComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified; 
  @Input() room: ViewRoomSimplified; 
  @Input() personnel: ViewPersonnelSimplified; 
  
  allocations: ViewAllocationSimplified[];
  loading = true;
  canManage = false;

  constructor(
    private allocationService: AllocationService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private tokenService: TokenService,
    private router: Router
  ) {
    super();
  }

  ngOnInit(): void {

    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);
    if(this.equipment == undefined && this.room == undefined && this.personnel == undefined){
      console.warn('EntityAllocationsListComponent requires an entity in order to display logs');
      return;
    }

    let endPoint;

    if(this.equipment)
      endPoint = this.allocationService.getEquipmentAllocations(this.equipment.id);

    if(this.room)
      endPoint = this.allocationService.getEquipmentAllocationsToRoom(this.room.id);

    if(this.personnel)
      endPoint = this.allocationService.getEquipmentAllocationsToPersonnel(this.personnel.id);

    this.loading = true;
    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
        return;

        this.allocations = result;
      })
    )
  }

  addAllocation(): void {
    let selectedEntityType: string;
    let selectedEntities: IOption[];

    if(this.equipment){
      selectedEntityType = "equipment";
      selectedEntities = [
        {
          value: this.equipment.id, 
          label: this.equipment.temsIdOrSerialNumber
        }];
    }

    if(this.room){
      selectedEntityType = "room";
      selectedEntities = [
        {
          value: this.room.id, 
          label: this.room.identifier
        }];
    }

    if(this.personnel){
      selectedEntityType = "personnel";
      selectedEntities = [
        {
          value: this.personnel.id, 
          label: this.personnel.name
        }];
    }

    this.dialogService.openDialog(
      EquipmentAllocationComponent,
      [{label: selectedEntityType, value: selectedEntities}],
      () => {
        this.ngOnInit();
      }
    )
  }

  viewRoom(roomId: string){
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(()=>
    this.router.navigate(['/rooms/details/' + roomId]));
  }
  
  viewPersonnel(personnelId: string){
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(()=>
    this.router.navigate(['/personnel/details/' + personnelId]));
  }

  return(allocationId: string, index: number){
    this.subscriptions.push(
      this.allocationService.markAsReturned(allocationId)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.allocations[index].dateReturned = new Date;
      })
    )
  }

  remove(allocationId: string, index:number){
    if(!confirm('Are you sure you want to remove that allocation?'))
      return;
    
    this.subscriptions.push(
      this.allocationService.remove(allocationId)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.allocations.splice(index, 1);
      })
    )
  }
}
