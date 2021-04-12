import { AttachEquipmentComponent } from './../../attach-equipment/attach-equipment.component';
import { DialogService } from './../../../../services/dialog-service/dialog.service';
import { IOption } from './../../../../models/option.model';
import { CAN_MANAGE_ENTITIES } from './../../../../models/claims';
import { TokenService } from './../../../../services/token-service/token.service';
import { SnackService } from './../../../../services/snack/snack.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Property } from './../../../../models/equipment/view-property.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ViewEquipment } from 'src/app/models/equipment/view-equipment.model';
import { Router } from '@angular/router';

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
  headerClass;

  canManage:boolean = false;
  equipment: ViewEquipment;
  get canAttach(){
    // returns true if equipment can have children
    return this.equipment.definition.children.length > 0;
  }
  generalProperties: Property[];
  specificProperties: Property[];
  detachedEquipments = [];
  editing = false;

  constructor(
    private equipmentService: EquipmentService,
    private tokenService: TokenService,
    private route: Router,
    private dialogService: DialogService,
    private snackService: SnackService) {
    super();
  }

  ngOnInit(): void {
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);

    this.subscriptions.push(this.equipmentService.getEquipmentByID(this.equipmentId)
      .subscribe(response => {
        if(this.snackService.snackIfError(response))
          return;
        
        console.log(response);
        this.equipment = response;
        this.headerClass = (this.equipment.isArchieved) ? 'text-muted' : '';

        this.generalProperties= [
          { displayName: 'Identifier', value: this.equipment.definition.identifier},
          { displayName: 'Type', value: this.equipment.type.name},
          { displayName: 'TemsID', value: this.equipment.temsId },
          { displayName: 'Serial Number', value: this.equipment.serialNumber},
          { displayName: 'Is Used', dataType: 'boolean', name: 'isUsed', value: this.equipment.isUsed},
          { displayName: 'Is Defect', dataType: 'boolean', name: 'isUsed', value: this.equipment.isDefect},
        ];
    
        this.specificProperties = this.equipment.specificTypeProperties;
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
      })
    )
  }

  viewMore(){
    if(this.dialogRef != undefined)
      this.dialogRef.close();
  }

  changeState(attribute: string){
    this.subscriptions.push(
      this.equipmentService.changeState(attribute, this.equipmentId)
      .subscribe(result => {
        console.log(result);

        if(result.status == 1){
          if(attribute == 'isDefect'){
            this.equipment.isDefect = !this.equipment.isDefect;
            this.generalProperties[this.generalProperties.length-1].value = this.equipment.isDefect;
          }
          else
          this.equipment.isUsed = !this.equipment.isUsed;
          this.generalProperties[this.generalProperties.length-2].value = this.equipment.isUsed;
        }
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

  detach(childId: string, index: number){
    if(!confirm("Are you sure you want to detach this equipment from it's parent?"))
      return;

    this.subscriptions.push(
      this.equipmentService.detach(childId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.detachedEquipments.push(this.equipment.children[index]);
        this.equipment.children.splice(index, 1);
      })
    )
  }
}
