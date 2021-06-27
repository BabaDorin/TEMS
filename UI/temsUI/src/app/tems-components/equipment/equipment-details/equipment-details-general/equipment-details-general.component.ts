import { ClaimService } from './../../../../services/claim.service';
import { RoomDetailsGeneralComponent } from './../../../room/room-details-general/room-details-general.component';
import { AttachEquipmentComponent } from './../../attach-equipment/attach-equipment.component';
import { DialogService } from '../../../../services/dialog.service';
import { CAN_MANAGE_ENTITIES } from './../../../../models/claims';
import { TokenService } from '../../../../services/token.service';
import { SnackService } from '../../../../services/snack.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Property } from './../../../../models/equipment/view-property.model';
import { EquipmentService } from 'src/app/services/equipment.service';
import { Component, Input, OnInit, Output, EventEmitter, OnChanges, SimpleChanges, Inject, Optional } from '@angular/core';
import { ViewEquipment } from 'src/app/models/equipment/view-equipment.model';
import { PersonnelDetailsGeneralComponent } from 'src/app/tems-components/personnel/personnel-details-general/personnel-details-general.component';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-equipment-details-general',
  templateUrl: './equipment-details-general.component.html',
  styleUrls: ['./equipment-details-general.component.scss']
})
export class EquipmentDetailsGeneralComponent extends TEMSComponent implements OnInit {

  @Input() equipmentId: string;
  @Input() displayViewMore: boolean = false;
  @Output() archivationStatusChanged = new EventEmitter();
  
  dialogRef;
  headerClass; // muted when archieved
  equipment: ViewEquipment;
  generalProperties: Property[];
  detachedEquipments = [];
  editing = false;

  get canAttach(){
    // returns true if equipment can have children
    return this.equipment.definition.children.length > 0;
  }

  constructor(
    private equipmentService: EquipmentService,
    public claims: ClaimService,
    private dialogService: DialogService,
    private snackService: SnackService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any) {
    super();

    if(dialogData != undefined){
      this.equipmentId = this.dialogData.equipmentId;
      this.displayViewMore = this.dialogData.displayViewMore;
    }
  }

  ngOnInit(): void {
    this.subscriptions.push(this.equipmentService.getEquipmentByID(this.equipmentId)
      .subscribe(response => {
        if(this.snackService.snackIfError(response))
          return;
        
        this.equipment = response;
        console.log(response);
        this.headerClass = (this.equipment.isArchieved) ? 'text-muted' : '';

        this.generalProperties= [
          { displayName: 'Identifier', value: this.equipment.definition.identifier},
          { displayName: 'Type', value: this.equipment.type.name},
          { displayName: 'TemsID', value: this.equipment.temsId },
          { displayName: 'Serial Number', value: this.equipment.serialNumber},
          { displayName: 'Is Used', dataType: 'boolean', name: 'isUsed', value: this.equipment.isUsed},
          { displayName: 'Is Defect', dataType: 'boolean', name: 'isUsed', value: this.equipment.isDefect},
        ];
    
        if(this.detachedEquipments != undefined){
          this.detachedEquipments = this.detachedEquipments.filter(q => {
            return this.equipment.children.findIndex(eq => eq.value == q.value) == -1
          })
        }
      }))
  }

  edit(){
    this.editing = true;    
  }

  archieve(){
    if(!this.equipment.isArchieved && !confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
      return;

    let newArchivationStatus = !this.equipment.isArchieved;
    this.subscriptions.push(
      this.equipmentService.archieveEquipment(this.equipmentId, newArchivationStatus)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.equipment.isArchieved = newArchivationStatus;
          this.headerClass = (this.equipment.isArchieved) ? 'text-muted' : '';

        this.archivationStatusChanged.emit(this.equipment.isArchieved);

        this.ngOnInit();
      })
    )
  }

  viewMore(){
    if(this.dialogRef != undefined)
      this.dialogRef.close();
  }

  changeWorkingState(){
    this.subscriptions.push(
      this.equipmentService.changeWorkingState(this.equipmentId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.equipment.isDefect = !this.equipment.isDefect;
        this.generalProperties[this.generalProperties.length-1].value = this.equipment.isDefect;
      })
    )
  }

  changeUsingState(){
    this.subscriptions.push(
      this.equipmentService.changeUsingState(this.equipmentId)
      .subscribe(result =>{
        if(this.snackService.snackIfError(result))
          return;
      
        this.equipment.isUsed = !this.equipment.isUsed;
        this.generalProperties[this.generalProperties.length-2].value = this.equipment.isUsed;
      })
    )
  }

  attach(){
    this.dialogService.openDialog(
      AttachEquipmentComponent,
      [{label: "equipment", value: this.equipment}],
      () => {
        this.ngOnInit();
      }
    )
  }

  detached(index: number){
    this.detachedEquipments.push(this.equipment.children[index]);
    this.equipment.children.splice(index, 1);
  }

  viewAllocatee(){
    // Allocated to personnel
    if(this.equipment.personnel != undefined){
      this.dialogService.openDialog(
        PersonnelDetailsGeneralComponent,
        [
          { label: "personnelId", value: this.equipment.personnel.value },
          { label: "displayViewMore", value: true }
        ]
      );
    }

    // Allocated to room
    if(this.equipment.room != undefined){
      this.dialogService.openDialog(
        RoomDetailsGeneralComponent,
        [
          { label: "roomId", value: this.equipment.room.value },
          { label: "displayViewMore", value: true }
        ]
      );
    }
  }
}
