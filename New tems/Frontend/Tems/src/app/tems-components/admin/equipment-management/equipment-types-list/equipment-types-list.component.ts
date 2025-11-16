import { TranslateService } from '@ngx-translate/core';
import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { ViewTypeSimplified } from 'src/app/models/equipment/view-type-simplified.model';
import { AddTypeComponent } from 'src/app/tems-components/equipment/add-type/add-type.component';
import { EquipmentTypeContainerModel } from '../../../../models/generic-container/equipment-type-container.model';
import { DialogService } from '../../../../services/dialog.service';
import { SnackService } from '../../../../services/snack.service';
import { EquipmentService } from './../../../../services/equipment.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { ConfirmService } from 'src/app/confirm.service';
import { GenericContainerComponent } from 'src/app/shared/generic-container/generic-container.component';

@Component({
  selector: 'app-equipment-types-list',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    TranslateModule,
    NgxPaginationModule,
    GenericContainerComponent
  ],
  templateUrl: './equipment-types-list.component.html',
  styleUrls: ['./equipment-types-list.component.scss']
})
export class EquipmentTypesListComponent extends TEMSComponent implements OnInit {

  @Input() canManage: boolean = false;

  pageNumber = 1;
  itemsPerPage = 10;
  types: ViewTypeSimplified[] = [];
  typeContainerModels: EquipmentTypeContainerModel[] = [];

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService,
    private snackService: SnackService,
    public translate: TranslateService,
    private confirmService: ConfirmService
  ) {
    super();
  }

  eventEmitted(eventData, index){
    if(eventData == 'removed')
      this.typeContainerModels.splice(index, 1);
  }

  addType(){
    this.dialogService.openDialog(
      AddTypeComponent,
      undefined,
      () => {
        this.fetchTypes();
      }
    );
  }

  fetchTypes(){
    this.subscriptions.push(
      this.equipmentService.getTypesSimplified()
      .subscribe(result => {
        this.types = result;
        this.buildCardModels();
      })
    )
  }

  buildCardModels(){
    this.typeContainerModels = this.types.map(q => new EquipmentTypeContainerModel(
      this.equipmentService,
      this.dialogService,
      this.snackService,
      q,
      this.confirmService))
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.equipmentService.getTypesSimplified()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.types = result;
        this.buildCardModels();
      })
    );
  }
}
