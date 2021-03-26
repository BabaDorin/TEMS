import { TEMSComponent } from './../../../../tems/tems.component';
import { Property } from './../../../../models/equipment/view-property.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit } from '@angular/core';
import { ViewEquipment } from 'src/app/models/equipment/view-equipment.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-equipment-details-general',
  templateUrl: './equipment-details-general.component.html',
  styleUrls: ['./equipment-details-general.component.scss']
})
export class EquipmentDetailsGeneralComponent extends TEMSComponent implements OnInit {

  @Input() equipmentId: string;

  equipment: ViewEquipment;
  generalProperties: Property[];
  specificProperties: Property[];
  editing = false;

  constructor(
    private equipmentService: EquipmentService,
    private route: ActivatedRoute) {
    super();
  }

  ngOnInit(): void {
    // if(this.equipmentId == undefined)
      // this.equipmentId = this.route.snapshot.paramMap.get('id');
    
    this.subscriptions.push(this.equipmentService.getEquipmentByID(this.equipmentId)
      .subscribe(response => {
        console.log(response);
        this.equipment = response;

        this.generalProperties= [
          { displayName: 'Identifier', value: this.equipment.definition.label},
          { displayName: 'Type', value: this.equipment.type.name},
          { displayName: 'TemsID', value: this.equipment.temsId },
          { displayName: 'Serial Number', value: this.equipment.serialNumber},
          { displayName: 'Is Used', dataType: 'boolean', value: this.equipment.isUsed},
          { displayName: 'Is Defect', dataType: 'boolean', value: this.equipment.isUsed},
        ];
    
        this.specificProperties = this.equipment.specificTypeProperties;
      }))
  }

  edit(){
    this.editing = true;    
  }
}
