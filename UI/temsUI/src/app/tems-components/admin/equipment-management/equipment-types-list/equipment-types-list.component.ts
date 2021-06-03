import { EquipmentTypeContainerModel } from './../../../../models/generic-container/EquipmentTypeContainerModel';
import { SnackService } from './../../../../services/snack/snack.service';
import { EquipmentService } from './../../../../services/equipment-service/equipment.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Component, OnInit, Input } from '@angular/core';
import { ViewTypeSiplified } from 'src/app/models/equipment/view-type-simplified.model';

@Component({
  selector: 'app-equipment-types-list',
  templateUrl: './equipment-types-list.component.html',
  styleUrls: ['./equipment-types-list.component.scss']
})
export class EquipmentTypesListComponent extends TEMSComponent implements OnInit {

  @Input() canManage: boolean = false;

  pageNumber = 1;
  types: ViewTypeSiplified[] = [];
  typeContainerModels: EquipmentTypeContainerModel[] = [];
  
  constructor(
    private equipmentService: EquipmentService,
    private snackService: SnackService,
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.equipmentService.getTypesSimplified()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.types = result;
        this.typeContainerModels = this.types.map(q => new EquipmentTypeContainerModel(this.equipmentService, q))
      })
    );
  }
}
