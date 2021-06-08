import { ViewEquipment } from 'src/app/models/equipment/view-equipment.model';
import { IOption } from './../../../models/option.model';
import { FormGroup, FormControl } from '@angular/forms';
import { TEMSComponent } from './../../../tems/tems.component';
import { SnackService } from './../../../services/snack/snack.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Inject, Input, OnInit } from '@angular/core';
import { AttachEquipment } from 'src/app/models/equipment/attach-equipment.model';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Optional } from '@angular/core';

@Component({
  selector: 'app-attach-equipment',
  templateUrl: './attach-equipment.component.html',
  styleUrls: ['./attach-equipment.component.scss']
})
export class AttachEquipmentComponent extends TEMSComponent implements OnInit {

  @Input() equipment: ViewEquipment;

  attachEquipmentFormGroup = new FormGroup({
    equipmentDefinition: new FormControl(),
    equipmentToAttach: new FormControl()
  });
  dialogRef;
  
  availableEquipment = [] as IOption[];

  constructor(
    private equipmentService: EquipmentService,
    private snackService: SnackService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(dialogData != undefined){
      this.equipment = dialogData.equipment;
    }
  }

  onDefinitionChanged(newDefinition){
    this.fetchEquipmentOfDefinitions([newDefinition.value]);
  }

  ngOnInit(): void {
    this.fetchEquipmentOfDefinitions(this.equipment.definition.children.map(q => q.id));
  }

  fetchEquipmentOfDefinitions(definitions: string[]){
    this.unsubscribeFromAll();

    this.subscriptions.push(
      this.equipmentService.getEquipmentOfDefinitions(definitions)
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;
        
        this.availableEquipment = result;
      })
    )
  }

  onSubmit(model){
    let selectedEq = model.equipmentToAttach.value;
    
    if(selectedEq == undefined || selectedEq.length == 0){
      this.snackService.snack({message: "Please, provide at least one equipment to allocate", status: 0});
      return;
    }

    let attachEquipment = new AttachEquipment();
    attachEquipment.parentId = this.equipment.id;
    attachEquipment.childrenIds = selectedEq.map(q => q.value);

    console.log('000');
    console.log(attachEquipment);

    this.subscriptions.push(
      this.equipmentService.attach(attachEquipment)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.dialogRef.close();
      })      
    )
  }
}
