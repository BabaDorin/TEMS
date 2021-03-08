import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { ViewEquipmentSimplified } from './../../../models/equipment/view-equipment-simplified.model';
import { Component, ElementRef, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-equipment-details',
  templateUrl: './equipment-details.component.html',
  styleUrls: ['./equipment-details.component.scss']
})
export class EquipmentDetailsComponent implements OnInit {

  @Input() equipmentId;
  edit: boolean;
  equipmentSimplified: ViewEquipmentSimplified;
  // equipment: ViewEquipment;
  

  constructor(private activatedroute: ActivatedRoute, private elementRef: ElementRef,
    private equipmentService: EquipmentService) {
  }

  ngOnInit(): void {
    if(this.equipmentId == undefined)
      this.equipmentId = this.activatedroute.snapshot.paramMap.get("id");
    this.edit=false;

    this.equipmentSimplified = this.equipmentService.getEquipmentSimplifiedById(this.equipmentId);
  }
}
