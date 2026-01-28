import { DialogService } from './../../../services/dialog.service';
import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule } from '@ngx-translate/core';
import { AssetFilter } from 'src/app/helpers/filters/asset.filter';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { ReportFromFilterComponent } from '../../reports/report-from-filter/report-from-filter.component';
import { IncludeAssetLabelsComponent } from 'src/app/shared/include-asset-tags/include-asset-tags.component';
import { SummaryAssetAnalyticsComponent } from '../../analytics/summary-asset-analytics/summary-asset-analytics.component';
import { AgGridAssetComponent } from '../../asset/ag-grid-asset/ag-grid-asset.component';
import { EntityAllocationsListComponent } from '../../entity-allocations-list/entity-allocations-list.component';

@Component({
  selector: 'app-personnel-details-allocations',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    TranslateModule,
    IncludeAssetLabelsComponent,
    SummaryAssetAnalyticsComponent,
    AgGridAssetComponent,
    EntityAllocationsListComponent
  ],
  templateUrl: './personnel-details-allocations.component.html',
  styleUrls: ['./personnel-details-allocations.component.scss']
})
export class PersonnelDetailsAllocationsComponent implements OnInit {
  
  @Input() personnel: ViewPersonnelSimplified;
  assetFilter: AssetFilter;
  @ViewChild('includeEquipmentLabels') includeEquipmentLabels: IncludeAssetLabelsComponent;
  defaultLabels = ['Equipment'];

  constructor(
    private lazyLoader: LazyLoaderService,
    private dialogService: DialogService) { 
  }

  ngOnInit(): void {
    if(this.personnel == undefined)
      return;
    
    let filter = new AssetFilter();
    filter.personnel = [this.personnel.id];
    filter.includeLabels = this.includeEquipmentLabels?.value ?? this.defaultLabels;
    this.assetFilter = Object.assign(new AssetFilter(), filter);
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

  includeLabelsChanged(){
    this.assetFilter.includeLabels = this.includeEquipmentLabels?.value ?? this.defaultLabels;
    this.assetFilter = Object.assign(new AssetFilter(), this.assetFilter);
  }

  refreshAgGrid(){
    this.assetFilter = Object.assign(new AssetFilter(), this.assetFilter);
  }
}
