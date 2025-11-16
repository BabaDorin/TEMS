import { TranslateService } from '@ngx-translate/core';
import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { ViewDefinitionSimplified } from 'src/app/models/equipment/view-definition-simplified.model';
import { DialogService } from '../../../../services/dialog.service';
import { SnackService } from '../../../../services/snack.service';
import { DefinitionContainerModel } from './../../../../models/generic-container/definition-container.model';
import { EquipmentService } from './../../../../services/equipment.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { AddDefinitionComponent } from './../../../equipment/add-definition/add-definition.component';
import { ConfirmService } from 'src/app/confirm.service';
import { GenericContainerComponent } from 'src/app/shared/generic-container/generic-container.component';

@Component({
  selector: 'app-equipment-definitions-list',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    TranslateModule,
    NgxPaginationModule,
    GenericContainerComponent
  ],
  templateUrl: './equipment-definitions-list.component.html',
  styleUrls: ['./equipment-definitions-list.component.scss']
})
export class EquipmentDefinitionsListComponent extends TEMSComponent implements OnInit {

  pageNumber = 1;
  itemsPerPage = 10;

  @Input() canManage: boolean = false;

  definitions: ViewDefinitionSimplified[];
  definitionContainerModels: DefinitionContainerModel[] = [];

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
    this.fetchDefinitions();
  }

  fetchDefinitions(){
    this.subscriptions.push(
      this.equipmentService.getDefinitionsSimplified()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.definitions = result;
        this.buildCardModels();
      })
    );
  }

  buildCardModels(){
    this.definitionContainerModels = this.definitions.map(q => new DefinitionContainerModel(
      this.equipmentService,
      this.dialogService,
      this.snackService,
      q,
      this.confirmService
    ));
  }

  eventEmitted(eventData, index){
    if(eventData == 'removed')
      this.definitionContainerModels.splice(index, 1);
  }

  addDefinition(){
    this.dialogService.openDialog(
      AddDefinitionComponent,
      undefined,
      () => {
        this.fetchDefinitions();
      }
    )
  }
}
