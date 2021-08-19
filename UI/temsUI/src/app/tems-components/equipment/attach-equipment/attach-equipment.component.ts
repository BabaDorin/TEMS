import { TranslateService } from '@ngx-translate/core';
import { DefinitionService } from './../../../services/definition.service';
import { ViewType } from './../../../models/equipment/view-type.model';
import { TypeService } from './../../../services/type.service';
import { EquipmentFilter } from 'src/app/helpers/filters/equipment.filter';
import { Component, Inject, Input, OnInit, Optional, Output, EventEmitter } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AttachEquipment } from 'src/app/models/equipment/attach-equipment.model';
import { ViewEquipment } from 'src/app/models/equipment/view-equipment.model';
import { EquipmentService } from 'src/app/services/equipment.service';
import { SnackService } from '../../../services/snack.service';
import { IOption } from './../../../models/option.model';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-attach-equipment',
  templateUrl: './attach-equipment.component.html',
  styleUrls: ['./attach-equipment.component.scss']
})
export class AttachEquipmentComponent extends TEMSComponent implements OnInit {

  dialogRef;
  @Input() equipment: ViewEquipment;
  @Output() childAttached = new EventEmitter();

  // Equipment's child types
  types: IOption[] = [];
  // Definitions of selected type
  definitions: IOption[] = [];

  equipmentFilter: EquipmentFilter;

  attachEquipmentFormGroup = new FormGroup({
    equipmentDefinition: new FormControl(),
    equipmentType: new FormControl(),
    includeAttached: new FormControl(false)
  });

  private getIncludeAttached() {
    return this.attachEquipmentFormGroup.controls.includeAttached.value;
  }

  private getSelectedType() {
    return this.attachEquipmentFormGroup.controls.equipmentType.value;
  }

  private getSelectedDefinition() {
    return this.attachEquipmentFormGroup.controls.equipmentDefinition.value;
  }

  private setSelectedDefinition(value){
    this.attachEquipmentFormGroup.controls.equipmentDefinition.setValue(value);
  }

  constructor(
    public equipmentService: EquipmentService,
    private snackService: SnackService,
    private typeService: TypeService,
    private definitionService: DefinitionService,
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if (dialogData != undefined) {
      this.equipment = dialogData.equipment;
    }
  }

  ngOnInit(): void {
    this.fetchRelevantTypes()
    .then(() => {
      let filter = new EquipmentFilter();
      filter.onlyDetached = true;
      filter.types = this.types.map(q => q.value);
      this.equipmentFilter = filter;
    });

    this.definitions = this.equipment.definition.children.map(q => ({ value: q.id, label: q.identifier } as IOption));
  }

  typeChanged(newType){
    // Cancel current selected definition
    this.setSelectedDefinition(undefined);

    this.subscriptions.push(
      this.definitionService.getDefinitionsOfType(newType.value)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.definitions = result;
      })
    );

    this.filterChanged();
  }

  filterChanged() {
    this.equipmentFilter.onlyDetached = !this.getIncludeAttached();

    // ether equipment of selected type, or equipment of any type which is child of equipment's type
    let selectedType = this.getSelectedType();
    if (selectedType != undefined)
      this.equipmentFilter.types = [selectedType]
    else
      this.equipmentFilter.types = this.types.map(q => q.value);

    let selectedDefinition = this.getSelectedDefinition();
    if (selectedDefinition != undefined)
      this.equipmentFilter.definitions = [selectedDefinition];

    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
  }

  fetchRelevantTypes() : Promise<any> {
    // relevant types = child types of current equipment
    return new Promise((resolve, reject) => {
      this.typeService.getFullType(this.equipment.definition.equipmentType.value)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          reject();
        
        this.types = (result as ViewType).children.map(q => ({ value: q.id, label: q.name } as IOption));
        resolve(true);
      });
    });
  }

  onSubmit(model) {
    let selectedEq = model.equipmentToAttach.value;

    if (selectedEq == undefined || selectedEq.length == 0) {
      this.snackService.snack({ message: "Please, provide at least one equipment to allocate", status: 0 });
      return;
    }

    let attachEquipment = new AttachEquipment();
    attachEquipment.parentId = this.equipment.id;
    attachEquipment.childrenIds = selectedEq.map(q => q.value);

    this.subscriptions.push(
      this.equipmentService.attach(attachEquipment)
        .subscribe(result => {
          this.snackService.snack(result);

          if (result.status == 1)
            this.dialogRef.close();
        })
    )
  }

  attached(rowData){
    let attachedEq = {
      value: rowData.id,
      label: rowData.definition,
      additional: rowData.type,
    } as IOption;

    this.childAttached.emit(attachedEq);
  }
}