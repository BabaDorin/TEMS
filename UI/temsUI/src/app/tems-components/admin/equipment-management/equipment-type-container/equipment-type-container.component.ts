import { SnackService } from '../../../../services/snack.service';
import { ViewType } from './../../../../models/equipment/view-type.model';
import { EquipmentService } from './../../../../services/equipment.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-equipment-type-container',
  templateUrl: './equipment-type-container.component.html',
  styleUrls: ['./equipment-type-container.component.scss']
})
export class EquipmentTypeContainerComponent extends TEMSComponent implements OnInit {

  @Input() typeId: string;
  type: ViewType;

  constructor(
    private equipmentService: EquipmentService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    // this.subscriptions.push(
    //   this.equipmentService.getFullType(this.typeId)
    //   .subscribe(result => {
    //     if(this.snackService.snackIfError(result))
    //       return;
        
    //     this.type = result;
    //   });

    //   this.a["media"].;
    // )
  }

}