import { IncludeAssetLabelsComponent } from './../../../shared/include-asset-tags/include-asset-tags.component';
import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { ReportFromFilterComponent } from './../../reports/report-from-filter/report-from-filter.component';
import { AssetFilter } from './../../../helpers/filters/asset.filter';
import { MultipleSelectionDropdownComponent } from './../../../shared/forms/multiple-selection-dropdown/multiple-selection-dropdown.component';
import { TypeService } from './../../../services/type.service';
import { TypeEndpoint } from './../../../helpers/endpoints/type.endpoint';
import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ViewAssetSimplified } from 'src/app/models/asset/view-asset-simplified.model';
import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { AssetAllocationComponent } from '../asset-allocation/asset-allocation.component';
import { IOption } from './../../../models/option.model';
import { ClaimService } from './../../../services/claim.service';
import { AgGridAssetComponent } from './../ag-grid-asset/ag-grid-asset.component';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { SummaryAssetAnalyticsComponent } from '../../analytics/summary-asset-analytics/summary-asset-analytics.component';

@Component({
  selector: 'app-view-asset',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatTabsModule,
    TranslateModule,
    AgGridAssetComponent,
    MultipleSelectionDropdownComponent,
    IncludeAssetLabelsComponent,
    SummaryAssetAnalyticsComponent
  ],
  templateUrl: './view-asset.component.html',
  styleUrls: ['./view-asset.component.scss']
})
export class ViewAssetComponent implements OnInit {

  // typePreOptions: IOption[] = [];
  typeEndpoint: TypeEndpoint;
  assetFilter: AssetFilter;
  defaultLabels = ['Equipment']; // When no label is selected => equipment is selected

  @ViewChild('typeSelection') typeSelection: MultipleSelectionDropdownComponent;
  @ViewChild('agGridEquipment') agGridEquipment: AgGridAssetComponent;
  @ViewChild('includeEquipmentLabels') includeEquipmentLabels: IncludeAssetLabelsComponent;

  constructor(
    public dialogService: DialogService,
    public router: Router,
    private snackService: SnackService,
    public claims: ClaimService,
    public translate: TranslateService,
    private typeService: TypeService,
    private lazyLoader: LazyLoaderService
  ) {

    let includeDerived = false;
    if(this.includeEquipmentLabels != undefined){
      includeDerived = this.includeEquipmentLabels.includeComponents || this.includeEquipmentLabels.includeParts;
    }

    this.typeEndpoint = new TypeEndpoint(this.typeService, includeDerived);

    this.assetFilter = new AssetFilter();
    this.assetFilter.includeLabels = this.includeEquipmentLabels?.value ?? ['Equipment'];
  }

  ngOnInit(): void {
  }

  typesChanged(eventData){
    if(eventData == undefined)
      return;
    
    this.assetFilter.types = eventData;
    this.assetFilter = Object.assign(new AssetFilter(), this.assetFilter);
  }

  addLogSelected(){
    if(!this.claims.canManageAssets)
      return;

    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      AddLogComponent,
      [{label: "asset", value: selectedNodes }]
    )
  }

  addIssue(){
    if(!this.claims.canManageAssets)
      return;

    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      CreateIssueComponent,
      [{label: "assetAlreadySelected", value: selectedNodes }]
    )
  }

  allocateSelected() {
    if(!this.claims.canManageAssets)
      return;

    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      AssetAllocationComponent,
      [{label: "asset", value: selectedNodes }]
    )
  }

  getSelectedNodes(): IOption[] {
    let selectedNodes = this.agGridEquipment.getSelectedNodes();

    if (selectedNodes.length == 0)
      return;
    
    if(selectedNodes.length > 20){
      this.snackService.snack({message: "You can not treat more than 20 items at a time", status: 0})
      return;
    }

    selectedNodes = (this.agGridEquipment.getSelectedNodes() as ViewAssetSimplified[])
      .map(node => ({value: node.id, label: node.temsIdOrSerialNumber} as IOption));

    return selectedNodes;
  }

  addNew(){
    if(!this.claims.canManageAssets)
      return;

    this.router.navigate(["/asset/add"]);
  }

  async generateReport(){
    await this.lazyLoader.loadModuleAsync('reports/report-from-filter.module.ts');
    this.dialogService.openDialog(
      ReportFromFilterComponent,
      [
        { label: 'assetFilter', value: this.assetFilter }
      ]
    );
  }

  includeTagsChanged(){
    // this.assetFilter.includeChildren = this.includeDerived;
    this.assetFilter.includeLabels = this.includeEquipmentLabels?.value ?? ['Equipment'];
    this.assetFilter = Object.assign(new AssetFilter(), this.assetFilter);

    let includeDerivedTypes = this.assetFilter.includeLabels.indexOf('Component') > -1 || this.assetFilter.includeLabels.indexOf('Part') > -1
    this.typeEndpoint = new TypeEndpoint(this.typeService, includeDerivedTypes);
  }
}
