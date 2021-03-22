import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { ViewTypeSiplified } from './../../../../models/equipment/view-type-simplified.model';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-manage-types-properties',
  templateUrl: './manage-types-properties.component.html',
  styleUrls: ['./manage-types-properties.component.scss']
})
export class ManageTypesPropertiesComponent extends TEMSComponent implements OnInit {

  types: ViewTypeSiplified[];
  properties = ['Model', 'Manufacturer', 'Pret','Model', 'Manufacturer', 'Pret','Model', 'Manufacturer', 'Pret','Model', 'Manufacturer', 'Pret', ]
  constructor(
    private equipmentService: EquipmentService
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.equipmentService.getTypesSimplified()
      .subscribe(result => {
        this.types = result;
      })
    )
  }
}
