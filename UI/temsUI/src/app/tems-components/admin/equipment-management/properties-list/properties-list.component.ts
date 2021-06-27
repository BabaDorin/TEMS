import { IGenericContainerModel } from './../../../../models/generic-container/IGenericContainer.model';
import { AddPropertyComponent } from './../../../equipment/add-property/add-property.component';
import { ViewPropertySimplified } from './../../../../models/equipment/view-property-simplified.model';
import { SnackService } from '../../../../services/snack.service';
import { DialogService } from '../../../../services/dialog.service';
import { EquipmentService } from './../../../../services/equipment.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { Component, Input, OnInit } from '@angular/core';
import { PropertyContainerModel } from 'src/app/models/generic-container/property-container.model';

@Component({
  selector: 'app-properties-list',
  templateUrl: './properties-list.component.html',
  styleUrls: ['./properties-list.component.scss']
})
export class PropertiesListComponent extends TEMSComponent implements OnInit {

  @Input() canManage: boolean = false;

  pageNumber: 1;
  properties: ViewPropertySimplified[];
  propContainerModels: IGenericContainerModel[] = [];

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService,
    private snackService: SnackService
  ) {
    super();
  }
  
  ngOnInit(): void {
    this.fetchProperties();
  }

  addProp(){
    this.dialogService.openDialog(
      AddPropertyComponent,
      undefined,
      () => {
        this.fetchProperties();
      }
    )
  }

  eventEmitted(eventData, index){
    if(eventData == 'removed')
      this.propContainerModels.splice(index, 1);
  }

  fetchProperties(){
    this.subscriptions.push(
      this.equipmentService.getPropertiesSimplified()
      .subscribe(result => {
        this.properties = result;
  
        this.buildPropContainerModels();
      })
    );
  }

  buildPropContainerModels(){
    if(this.properties != undefined)
      this.propContainerModels = this.properties.map(q => new PropertyContainerModel(
        this.equipmentService,
        this.dialogService,
        this.snackService,
        q));
  }
}
