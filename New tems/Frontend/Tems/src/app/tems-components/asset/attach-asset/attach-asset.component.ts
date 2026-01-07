import { TranslateService } from '@ngx-translate/core';
import { DefinitionService } from './../../../services/definition.service';
import { ViewType } from './../../../models/asset/view-type.model';
import { TypeService } from './../../../services/type.service';
import { AssetFilter } from 'src/app/helpers/filters/asset.filter';
import { Component, Inject, Input, OnInit, Optional, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { TranslateModule } from '@ngx-translate/core';
import { ViewEquipment } from 'src/app/models/asset/view-asset.model';
import { AssetService } from 'src/app/services/asset.service';
import { SnackService } from '../../../services/snack.service';
import { IOption } from './../../../models/option.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { IncludeAssetLabelsComponent } from '../../../shared/include-asset-tags/include-asset-tags.component';
import { AgGridAttachAssetComponent } from '../ag-grid-attach-asset/ag-grid-attach-asset.component';

@Component({
  selector: 'app-attach-equipment',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatOptionModule,
    TranslateModule,
    IncludeAssetLabelsComponent,
    AgGridAttachAssetComponent
  ],
  templateUrl: './attach-asset.component.html',
  styleUrls: ['./attach-asset.component.scss']
})
export class AttachAssetComponent extends TEMSComponent implements OnInit {

  @Input() equipment: ViewEquipment;
  @Output() childAttached = new EventEmitter();

  // Asset's child types
  types: IOption[] = [];
  // Definitions of selected type
  definitions: IOption[] = [];

  assetFilter: AssetFilter;
  tagOptions = ['Part', 'Component'];
  defaultLabels = ['Part'];

  attachEquipmentFormGroup = new FormGroup({
    assetDefinition: new FormControl(),
    assetType: new FormControl(),
    includeEquipmentLabels: new FormControl()
  });

  private getSelectedType() {
    return this.attachEquipmentFormGroup.controls.assetType.value;
  }

  private getSelectedDefinition() {
    return this.attachEquipmentFormGroup.controls.assetDefinition.value;
  }

  private setSelectedDefinition(value){
    this.attachEquipmentFormGroup.controls.assetDefinition.setValue(value);
  }

  constructor(
    public assetService: AssetService,
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
      let filter = new AssetFilter();
      filter.includeLabels = this.attachEquipmentFormGroup.value?.includeEquipmentLabels ?? this.defaultLabels;
      filter.types = this.types.map(q => q.value);
      this.assetFilter = filter;
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
    // equipment filter not initialized yet
    if(this.assetFilter == undefined)
      return;

    this.assetFilter.includeLabels = this.attachEquipmentFormGroup.value.includeEquipmentLabels ?? this.defaultLabels;

    // ether equipment of selected type, or equipment of any type which is child of equipment's type
    let selectedType = this.getSelectedType();
    if (selectedType != undefined)
      this.assetFilter.types = [selectedType]
    else
      this.assetFilter.types = this.types.map(q => q.value);

    let selectedDefinition = this.getSelectedDefinition();
    if (selectedDefinition != undefined)
      this.assetFilter.definitions = [selectedDefinition];

    this.assetFilter = Object.assign(new AssetFilter(), this.assetFilter);
  }

  fetchRelevantTypes() : Promise<any> {
    // relevant types = child types of current equipment
    return new Promise((resolve, reject) => {
      this.typeService.getFullType(this.equipment.definition.assetType.value)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          reject();
        
        this.types = (result as ViewType).children.map(q => ({ value: q.id, label: q.name } as IOption));
        resolve(true);
      });
    });
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