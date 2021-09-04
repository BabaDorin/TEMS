import { IOption } from './../../../../models/option.model';
import { LazyLoaderService } from './../../../../services/lazy-loader.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, EventEmitter, Inject, Input, OnInit, Optional, Output } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ViewEquipment } from 'src/app/models/equipment/view-equipment.model';
import { EquipmentService } from 'src/app/services/equipment.service';
import { PersonnelDetailsGeneralComponent } from 'src/app/tems-components/personnel/personnel-details-general/personnel-details-general.component';
import { DialogService } from '../../../../services/dialog.service';
import { SnackService } from '../../../../services/snack.service';
import { Property } from './../../../../models/equipment/view-property.model';
import { ClaimService } from './../../../../services/claim.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { RoomDetailsGeneralComponent } from './../../../room/room-details-general/room-details-general.component';
import { AttachEquipmentComponent } from './../../attach-equipment/attach-equipment.component';
import { ConfirmService } from 'src/app/confirm.service';

@Component({
  selector: 'app-equipment-details-general',
  templateUrl: './equipment-details-general.component.html',
  styleUrls: ['./equipment-details-general.component.scss']
})
export class EquipmentDetailsGeneralComponent extends TEMSComponent implements OnInit {

  @Input() equipmentId: string;
  @Input() displayViewMore: boolean = false;
  @Output() archivationStatusChanged = new EventEmitter();
  
  headerClass; // muted when archieved
  editing = false;

  equipment: ViewEquipment;
  generalProperties: Property[];
  detachedEquipment = [];

  get canAttach(){
    // returns true if equipment can have children
    // BEFREE: True if type has children. We don't care that much about definition children here.
    return this.equipment.definition.children.length > 0 && this.claims.canManage;
  }

  constructor(
    private equipmentService: EquipmentService,
    public claims: ClaimService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private translate: TranslateService,
    private lazyLoader: LazyLoaderService,
    private confirmService: ConfirmService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any) {
    super();

    if(dialogData != undefined){
      this.equipmentId = this.dialogData.equipmentId;
      this.displayViewMore = this.dialogData.displayViewMore;
    }
  }

  ngOnInit(): void {
    this.fetchEquipment();
  }

  // Fetch equipment data having it's ID and prepare it to be displayed on the view 
  fetchEquipment(){
    this.subscriptions.push(this.equipmentService.getEquipmentByID(this.equipmentId)
    .subscribe(response => {
      if(this.snackService.snackIfError(response))
        return;
      
      this.equipment = response;
      this.headerClass = (this.equipment.isArchieved) ? 'text-muted' : '';

      this.generalProperties= [
        { displayName: this.translate.instant('equipment.identifier'), value: this.equipment.definition.identifier},
        { displayName: this.translate.instant('equipment.type'), value: this.equipment.type.name},
        { displayName: this.translate.instant('equipment.TEMSID'), value: this.equipment.temsId },
        { displayName: this.translate.instant('equipment.serialNumber'), value: this.equipment.serialNumber},
        { displayName: this.translate.instant('equipment.description'), dataType: 'string', name: 'description', value: this.equipment.description},
      ];
  
      if(this.detachedEquipment != undefined){
        this.detachedEquipment = this.detachedEquipment.filter(q => {
          return this.equipment.children.findIndex(eq => eq.value == q.value) == -1
        });
      }
    }));
  }

  // Toggles the 'editing' property, which triggers the view to display it's data is a more "muted" way
  edit(){
    this.editing = true;    
  }

  viewMore(){
    if(this.dialogRef != undefined)
      this.dialogRef.close();
  }

  viewParent(){
    if(this.equipment.parent == undefined)
      return;
    
    this.dialogService.openDialog(
      EquipmentDetailsGeneralComponent,
      [
        { label: "displayViewMore", value: true },
        { label: "equipmentId", value: this.equipment.parent.value },
      ]
    );
  }

  // Displays the 'AttachEquipmentComponent' in a mat-dialog
  async attach(){
    await this.lazyLoader.loadModuleAsync('tems-ag-grid/attach-equipment-ag-grid.module.ts');

    let dialog = this.dialogService.openDialog(
      AttachEquipmentComponent,
      [{label: "equipment", value: this.equipment}]
    );

    this.subscriptions.push(
      dialog.componentInstance.childAttached
      .subscribe(attachedEq => this.attachedFromAttachView(attachedEq))
    );
  }

  // equipment's index within equipment's children list
  detached(index: number){
    // child is of type IOption, label => identifier, value => id, additional => flag indicating whether is it atached or not.
    let detachedChild = this.equipment.children[index];
    this.detachedEquipment.push(detachedChild);
    this.equipment.children.splice(index, 1);
  };

  // equipment's index within detachedEquipment list (makes sense)
  attached(indexFromDetachedEqList: number){
    let attachedChild = this.detachedEquipment[indexFromDetachedEqList];
    this.detachedEquipment.splice(indexFromDetachedEqList, 1);
    this.equipment.children.push(attachedChild);
  }

  // when some equipment is attached via AttachEquipmentComponent
  attachedFromAttachView(attachedEq: IOption){
    let childEqIndexFromDetached = this.detachedEquipment.findIndex(q => q.value == attachedEq.value);
    if(childEqIndexFromDetached != -1)
      this.detachedEquipment.splice(childEqIndexFromDetached, 1);
    
    this.equipment.children.push(attachedEq);
  }

  // Display allocatee information in a mat-dialog
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

  // Menu actions
  async archieve(){
    if(!this.equipment.isArchieved && !await this.confirmService.confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
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

  changeWorkingState(){
    this.subscriptions.push(
      this.equipmentService.changeWorkingState(this.equipmentId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.equipment.isDefect = !this.equipment.isDefect;
        this.generalProperties[this.generalProperties.length-1].value = this.equipment.isDefect;
      })
    );
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
    );
  }
}
