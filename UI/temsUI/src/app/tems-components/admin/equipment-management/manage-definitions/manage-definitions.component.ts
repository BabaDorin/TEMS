import { AddDefinitionComponent } from './../../../equipment/add-definition/add-definition.component';
import { DialogService } from './../../../../services/dialog-service/dialog.service';
import { ViewDefinitionSimplified } from './../../../../models/equipment/view-definition-simplified.model';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, OnInit } from '@angular/core';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { ViewDefinitionComponent } from 'src/app/tems-components/equipment/view-definition/view-definition.component';

@Component({
  selector: 'app-manage-definitions',
  templateUrl: './manage-definitions.component.html',
  styleUrls: ['./manage-definitions.component.scss']
})
export class ManageDefinitionsComponent extends TEMSComponent implements OnInit {

  definitions: ViewDefinitionSimplified[];
  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService
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

  view(definitionId: string){
    this.dialogService.openDialog(
      ViewDefinitionComponent,
      [{label: "definitionId", value: definitionId}],
    )
  }

  add(){
    this.dialogService.openDialog(
      AddDefinitionComponent,
      undefined,
      () => {
        this.unsubscribeFromAll();
        this.subscriptions.push(
          this.equipmentService.getDefinitionsSimplified()
          .subscribe(result => {
            if(result.length > this.definitions.length)
              this.definitions = result;
          })
        )
      }
    )
  }

  remove(definitionId: string, index: number){
    if(!confirm("Are you sure you want to remove that defintion?" + index))
    return;

    this.unsubscribeFromAll();
    this.subscriptions.push(
      this.equipmentService.archieveDefinition(definitionId)
      .subscribe(result => {
        if(result.status == 1)
          this.definitions.splice(index, 1);
      })
    )
  }

  edit(definitionId: string, index: number){
    this.dialogService.openDialog(
      AddDefinitionComponent,
      [{value: definitionId, label: "updateDefinitionId"}],
      () => {
        this.unsubscribeFromAll();
        this.equipmentService.getDefinitionSimplifiedById(definitionId)
        .subscribe(result => {
          this.definitions[index] = result;
        });
      }
    )
  }
}
