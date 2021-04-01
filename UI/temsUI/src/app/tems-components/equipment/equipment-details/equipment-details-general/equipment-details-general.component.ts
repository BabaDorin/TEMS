import { SnackService } from './../../../../services/snack/snack.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Property } from './../../../../models/equipment/view-property.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ViewEquipment } from 'src/app/models/equipment/view-equipment.model';
import { ActivatedRoute, Router } from '@angular/router';

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

  equipment: ViewEquipment;
  generalProperties: Property[];
  specificProperties: Property[];
  editing = false;

  constructor(
    private equipmentService: EquipmentService,
    private route: Router,
    private snackService: SnackService) {
    super();
  }

  ngOnInit(): void {
    // if(this.equipmentId == undefined)
      // this.equipmentId = this.route.snapshot.paramMap.get('id');

    this.subscriptions.push(this.equipmentService.getEquipmentByID(this.equipmentId)
      .subscribe(response => {
        console.log(response);
        this.equipment = response;
        this.headerClass = (this.equipment.isArchieved) ? 'text-muted' : '';

        this.generalProperties= [
          { displayName: 'Identifier', value: this.equipment.definition.label},
          { displayName: 'Type', value: this.equipment.type},
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

    this.subscriptions.push(
      this.equipmentService.archieveEquipment(this.equipmentId)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.equipment.isArchieved = !this.equipment.isArchieved;
          this.headerClass = (this.equipment.isArchieved) ? 'text-muted' : '';

        this.archivationStatusChanged.emit(this.equipment.isArchieved);
      })
    )
  }

  viewMore(){
    // console.log('here');
    // this.route.navigateByUrl('/equipment/details/' + this.equipmentId);
    
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
}
