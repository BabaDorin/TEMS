import { TranslateService } from '@ngx-translate/core';
import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { PropertyContainerModel } from 'src/app/models/generic-container/property-container.model';
import { DialogService } from '../../../../services/dialog.service';
import { SnackService } from '../../../../services/snack.service';
import { ViewPropertySimplified } from './../../../../models/equipment/view-property-simplified.model';
import { IGenericContainerModel } from './../../../../models/generic-container/IGenericContainer.model';
import { EquipmentService } from './../../../../services/equipment.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { AddPropertyComponent } from './../../../equipment/add-property/add-property.component';
import { ConfirmService } from 'src/app/confirm.service';
import { GenericContainerComponent } from 'src/app/shared/generic-container/generic-container.component';

@Component({
  selector: 'app-properties-list',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    TranslateModule,
    NgxPaginationModule,
    GenericContainerComponent
  ],
  templateUrl: './properties-list.component.html',
  styleUrls: ['./properties-list.component.scss']
})
export class PropertiesListComponent extends TEMSComponent implements OnInit {

  @Input() canManage: boolean = false;

  pageNumber = 1;
  itemsPerPage = 10;
  properties: ViewPropertySimplified[];
  propContainerModels: IGenericContainerModel[] = [];

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService,
    private snackService: SnackService,
    public translate: TranslateService,
    private confirmService: ConfirmService
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
        q,
        this.confirmService));
  }
}
