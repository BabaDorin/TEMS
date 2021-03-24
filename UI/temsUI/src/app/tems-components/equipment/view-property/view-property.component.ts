import { Property } from './../../../models/equipment/view-property.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-property',
  templateUrl: './view-property.component.html',
  styleUrls: ['./view-property.component.scss']
})
export class ViewPropertyComponent extends TEMSComponent implements OnInit {

  @Input() propertyId: string;
  property: Property = new Property();

  constructor(
    private equipmentService: EquipmentService
  ) { 
    super();
  }

  ngOnInit(): void {
    if(this.propertyId == undefined)
      return;

    this.subscriptions.push(
      this.equipmentService.getPropertyById(this.propertyId)
      .subscribe(result => {
        this.property = result;
      })
    )
  }
}
