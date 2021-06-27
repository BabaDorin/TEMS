import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { AllocationService } from '../../../services/allocation.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { emit } from 'process';
import { ViewAllocationSimplified } from './../../../models/equipment/view-equipment-allocation.model';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { PersonnelDetailsGeneralComponent } from '../../personnel/personnel-details-general/personnel-details-general.component';
import { RoomDetailsGeneralComponent } from '../../room/room-details-general/room-details-general.component';
import { EquipmentDetailsGeneralComponent } from '../equipment-details/equipment-details-general/equipment-details-general.component';

@Component({
  selector: 'app-equipment-allocation-container',
  templateUrl: './equipment-allocation-container.component.html',
  styleUrls: ['./equipment-allocation-container.component.scss']
})
export class EquipmentAllocationContainerComponent extends TEMSComponent implements OnInit {

  @Input() allocation: ViewAllocationSimplified;
  @Input() canManage: boolean = false;
  @Output() allocationRemoved = new EventEmitter();
  @Output() allocationReturned = new EventEmitter();
  
  constructor(
    private allocationService: AllocationService,
    private snackService: SnackService,
    private dialogService: DialogService
  ) {
    super();
  }

  ngOnInit(): void {
  }

  return(){
    this.subscriptions.push(
      this.allocationService.markAsReturned(this.allocation.id)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1){
          this.allocation.dateReturned = new Date;
          this.allocationReturned.emit();
        }
        
        
      })
    )
  }

  remove(){
    if(!confirm('Are you sure you want to remove that allocation?'))
      return;
    
    this.subscriptions.push(
      this.allocationService.archieve(this.allocation.id)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.allocationRemoved.emit();
      })
    )
  }

  viewRoom(roomId: string){
    this.dialogService.openDialog(
      RoomDetailsGeneralComponent,
      [
        { label: "roomId", value: roomId },
        { label: "displayViewMore", value: true }
      ]
    );
  }
  
  viewPersonnel(personnelId: string){
    this.dialogService.openDialog(
      PersonnelDetailsGeneralComponent,
      [
        { label: "personnelId", value: personnelId },
        { label: "displayViewMore", value: true }
      ]
    );
  }

  viewEquipment(equipmentId: string){
    this.dialogService.openDialog(
      EquipmentDetailsGeneralComponent,
      [
        { label: "equipmentId", value: equipmentId },
        { label: "displayViewMore", value: true }
      ]
    );
  }
  
}
