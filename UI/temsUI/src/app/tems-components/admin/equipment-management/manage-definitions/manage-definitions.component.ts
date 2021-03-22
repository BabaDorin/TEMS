import { ViewDefinitionSimplified } from './../../../../models/equipment/view-definition-simplified.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, OnInit } from '@angular/core';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-manage-definitions',
  templateUrl: './manage-definitions.component.html',
  styleUrls: ['./manage-definitions.component.scss']
})
export class ManageDefinitionsComponent extends TEMSComponent implements OnInit {

  definitions: ViewDefinitionSimplified[];
  constructor(
    private equipmentService: EquipmentService
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.equipmentService.getDefinitionsSimplified()
      .subscribe(result => {
        console.log(result);
        this.definitions = result;
      })
    )
  }
}
