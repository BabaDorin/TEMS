import { CAN_MANAGE_ENTITIES } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { SnackService } from './../../services/snack/snack.service';
import { TEMSComponent } from './../../tems/tems.component';
import { AllocationService } from './../../services/allocation-service/allocation.service';
import { ViewPersonnelSimplified } from './../../models/personnel/view-personnel-simplified.model';
import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
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
export class EntityAllocationsListComponent extends TEMSComponent implements OnInit, OnChanges {

  @Input() equipment: ViewEquipmentSimplified; 
  @Input() room: ViewRoomSimplified; 
  @Input() personnel: ViewPersonnelSimplified; 

  @Input() equipmentIds: string[] = [];
  @Input() personnelIds: string[] = [];
  @Input() definitionIds: string[] = [];
  @Input() roomIds: string[] = [];
  @Input() onlyActive: boolean;
  @Input() onlyClosed: boolean;
  
  @Output() allocationCreated = new EventEmitter();
  equipmentNotAllocated = false;
  
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
    this.fetchAllocations();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.cancelFirstOnChange){
      this.cancelFirstOnChange = false;
      return;
    }

    this.fetchAllocations();
  }
  
  fetchAllocations(){
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);

    if(this.equipment) this.equipmentIds = [this.equipment.id];
    if(this.room) this.roomIds = [this.room.id];
    if(this.personnel) this.personnelIds = [this.personnel.id];

    if(this.onlyActive == undefined) this.onlyActive = false;
    if(this.onlyClosed == undefined) this.onlyClosed = false;
    
    let include = 'any';
    if(this.onlyActive) include = 'active';
    if(this.onlyClosed) include = 'returned';

    let endPoint = this.allocationService.getAllocations(
      this.equipmentIds,
      this.definitionIds,
      this.personnelIds,
      this.roomIds,
      include
    );

    this.loading = true;
    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
        return;

        this.allocations = result;

        this.equipmentNotAllocated = (this.equipment != undefined && this.allocations.findIndex(q => q.dateReturned == null) == -1);
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
        this.allocationCreated.emit();
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
        
        if(this.onlyActive)
          this.allocations.splice(index, 1);
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
