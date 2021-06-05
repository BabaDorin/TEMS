import { AddDefinitionComponent } from './../../../equipment/add-definition/add-definition.component';
import { DefinitionContainerModel } from './../../../../models/generic-container/definition-container.model';
import { ViewDefinitionSimplified } from 'src/app/models/equipment/view-definition-simplified.model';
import { TEMSComponent } from './../../../../tems/tems.component';
import { SnackService } from './../../../../services/snack/snack.service';
import { DialogService } from './../../../../services/dialog-service/dialog.service';
import { EquipmentService } from './../../../../services/equipment-service/equipment.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-equipment-definitions-list',
  templateUrl: './equipment-definitions-list.component.html',
  styleUrls: ['./equipment-definitions-list.component.scss']
})
export class EquipmentDefinitionsListComponent extends TEMSComponent implements OnInit {

  @Input() canManage: boolean = false;

  definitions: ViewDefinitionSimplified[];
  definitionContainerModels: DefinitionContainerModel[] = [];

  pageNumber: 1;

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService,
    private snackService: SnackService,
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
      q
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
