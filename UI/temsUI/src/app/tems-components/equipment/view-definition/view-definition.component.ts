import { DialogService } from './../../../services/dialog-service/dialog.service';
import { Definition } from './../../../models/equipment/add-definition.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, OnInit } from '@angular/core';
import { ViewTypeComponent } from '../view-type/view-type.component';
import { ViewPropertyComponent } from '../view-property/view-property.component';

@Component({
  selector: 'app-view-definition',
  templateUrl: './view-definition.component.html',
  styleUrls: ['./view-definition.component.scss']
})
export class ViewDefinitionComponent extends TEMSComponent implements OnInit {

  definitionId: string;
  definition = new Definition();

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService
  ) { 
    super();
  }

  ngOnInit(): void {
    if(this.definitionId != undefined){
      this.equipmentService.getFullDefinition(this.definitionId)
      .subscribe(result => {
        console.log(result);
        this.definition = result;
      })
    }
  }

  viewType(typeId: string){
    this.dialogService.openDialog(
      ViewTypeComponent,
      [{value: typeId, label: "typeId"}],
    );
  }

  viewProperty(propertyId: string){
    this.dialogService.openDialog(
      ViewPropertyComponent,
      [{value: propertyId, label: "propertyId"}],
    );
  }

  viewDefinition(definitionId: string){
    this.dialogService.openDialog(
      ViewDefinitionComponent,
      [{value: definitionId, label: "definitionId"}],
    );
  }
}