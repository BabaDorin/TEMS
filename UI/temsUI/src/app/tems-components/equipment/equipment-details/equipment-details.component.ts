import { TEMSComponent } from './../../../tems/tems.component';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { ViewEquipmentSimplified } from './../../../models/equipment/view-equipment-simplified.model';
import { Component, ElementRef, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-equipment-details',
  templateUrl: './equipment-details.component.html',
  styleUrls: ['./equipment-details.component.scss']
})
export class EquipmentDetailsComponent extends TEMSComponent implements OnInit {

  @Input() equipmentId;
  edit: boolean;
  equipmentSimplified = new ViewEquipmentSimplified();
  mainHeaderLabel="General";

  constructor(
    private activatedroute: ActivatedRoute, 
    private elementRef: ElementRef,
    private equipmentService: EquipmentService) {
      super();
  }

  ngOnInit(): void {
    if(this.equipmentId == undefined)
      this.equipmentId = this.activatedroute.snapshot.paramMap.get("id");
    this.edit=false;

    this.subscriptions.push(this.equipmentService.getEquipmentSimplifiedById(this.equipmentId)
      .subscribe(result => {
        this.equipmentSimplified = result;

        if(this.equipmentSimplified.isArchieved)
          this.mainHeaderLabel += " (Archieved)"

      }));

    console.log('equipmentsimplified:');
    console.log(this.equipmentSimplified);
  }

  archivationStatusChanged(newStatus: boolean){
    this.mainHeaderLabel = "General"

    this.equipmentSimplified.isArchieved = !this.equipmentSimplified.isArchieved;
    if(this.equipmentSimplified.isArchieved)
          this.mainHeaderLabel += " (Archieved)"
  }
}
