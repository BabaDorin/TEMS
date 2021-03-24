import { AddPropertyComponent } from './../../../equipment/add-property/add-property.component';
import { DialogService } from './../../../../services/dialog-service/dialog.service';
import { AddTypeComponent } from './../../../equipment/add-type/add-type.component';
import { ViewTypeComponent } from './../../../equipment/view-type/view-type.component';
import { ViewPropertySimplified } from './../../../../models/equipment/view-property-simplified.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { ViewTypeSiplified } from './../../../../models/equipment/view-type-simplified.model';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { ViewPropertyComponent } from 'src/app/tems-components/equipment/view-property/view-property.component';

@Component({
  selector: 'app-manage-types-properties',
  templateUrl: './manage-types-properties.component.html',
  styleUrls: ['./manage-types-properties.component.scss']
})
export class ManageTypesPropertiesComponent extends TEMSComponent implements OnInit {

  types: ViewTypeSiplified[];
  properties: ViewPropertySimplified[];

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService,
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.equipmentService.getTypesSimplified()
      .subscribe(result => {
        console.log(result);
        this.types = result;
      })
    );

    this.subscriptions.push(
      this.equipmentService.getPropertiesSimplified()
      .subscribe(result => {
        console.log(result);
        this.properties = result;
      })
    )
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

  editType(typeId: string){
    this.dialogService.openDialog(
      AddTypeComponent,
      [{value: typeId, label: "updateTypeId"}],
    );
  }

  editProperty(propertyId: string){
    this.dialogService.openDialog(
      AddPropertyComponent,
      [{value: propertyId, label: "propertyId"}],
    );
  }

  removeType(typeId: string){
    if(confirm("Do you realy want to remove that type?")){
      this.equipmentService.removeType(typeId)
      .subscribe(result => {
        console.log(result);
      })
    }
  }
}
