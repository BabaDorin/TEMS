import { AddPropertyComponent } from './../../../equipment/add-property/add-property.component';
import { DialogService } from '../../../../services/dialog.service';
import { AddTypeComponent } from './../../../equipment/add-type/add-type.component';
import { ViewTypeComponent } from './../../../equipment/view-type/view-type.component';
import { ViewPropertySimplified } from './../../../../models/equipment/view-property-simplified.model';
import { EquipmentService } from 'src/app/services/equipment.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { ViewPropertyComponent } from 'src/app/tems-components/equipment/view-property/view-property.component';
import { ViewTypeSimplified } from 'src/app/models/equipment/view-type-simplified.model';

@Component({
  selector: 'app-manage-types-properties',
  templateUrl: './manage-types-properties.component.html',
  styleUrls: ['./manage-types-properties.component.scss']
})
export class ManageTypesPropertiesComponent extends TEMSComponent implements OnInit {

  types: ViewTypeSimplified[];
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

  addType(){
    this.dialogService.openDialog(
      AddTypeComponent,
      undefined,
      () => {
        this.unsubscribeFromAll();
        this.subscriptions.push(
          this.equipmentService.getTypesSimplified()
          .subscribe(result => {
            if(result.length > this.types.length)
              this.types = result;
          })
        )
      }
    );
  }

  addProperty(){
    this.dialogService.openDialog(
      AddPropertyComponent,
      undefined,
      () => {
        this.unsubscribeFromAll();
        this.subscriptions.push(
          this.equipmentService.getPropertiesSimplified()
          .subscribe(result => {
            if(result.length > this.types.length)
              this.properties = result;
          })
        )
      }
    );
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

  editType(typeId: string, index: number){
    this.dialogService.openDialog(
      AddTypeComponent,
      [{value: typeId, label: "updateTypeId"}],
      () => {
        this.unsubscribeFromAll();
        this.subscriptions.push(
          this.equipmentService.getTypeSimplifiedById(typeId)
          .subscribe(result => {
            this.types[index] = result;
          })
        )
      }
    );
  }

  editProperty(propertyId: string, index: number){
    this.dialogService.openDialog(
      AddPropertyComponent,
      [{value: propertyId, label: "propertyId"}],
      () => {
        this.unsubscribeFromAll();
        this.subscriptions.push(
          this.equipmentService.getPropertySimplifiedById(propertyId)
          .subscribe(result => {
            this.properties[index] = result;
          })
        )
      }
    );
  }

  removeType(typeId: string, index){
    if(confirm("Do you realy want to remove that type?"  + index)){
      this.equipmentService.archieveType(typeId)
      .subscribe(result => {
        console.log(result);
        if(result.status == 1)
          this.types.splice(index, 1);
      })
    }
  }

  removeProperty(propertyId: string, index: number){
    if(confirm("Do you realy want to remove that property?"  + index)){
      this.equipmentService.archieveProperty(propertyId)
      .subscribe(result => {
        console.log(result);
        if(result.status == 1)
          this.properties.splice(index, 1);
      })
    }
  }
}
