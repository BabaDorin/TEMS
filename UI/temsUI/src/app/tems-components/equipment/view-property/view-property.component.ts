import { Property } from './../../../models/equipment/view-property.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { EquipmentService } from 'src/app/services/equipment.service';
import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-view-property',
  templateUrl: './view-property.component.html',
  styleUrls: ['./view-property.component.scss']
})
export class ViewPropertyComponent extends TEMSComponent implements OnInit {

  @Input() propertyId: string;
  property: Property = new Property();

  constructor(
    private equipmentService: EquipmentService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    super();
    
    if(dialogData != undefined){
      this.propertyId = this.dialogData.propertyId;
    }
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
