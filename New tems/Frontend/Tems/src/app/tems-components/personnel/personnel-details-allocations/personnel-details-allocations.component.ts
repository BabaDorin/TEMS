import { DialogService } from './../../../services/dialog.service';
import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule } from '@ngx-translate/core';
import { EquipmentFilter } from 'src/app/helpers/filters/equipment.filter';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { ReportFromFilterComponent } from '../../reports/report-from-filter/report-from-filter.component';
import { IncludeEquipmentLabelsComponent } from 'src/app/shared/include-equipment-tags/include-equipment-tags.component';
import { SummaryEquipmentAnalyticsComponent } from '../../analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { AgGridEquipmentComponent } from '../../equipment/ag-grid-equipment/ag-grid-equipment.component';
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
    IncludeEquipmentLabelsComponent,
    SummaryEquipmentAnalyticsComponent,
    AgGridEquipmentComponent,
    EntityAllocationsListComponent
  ],
  templateUrl: './personnel-details-allocations.component.html',
  styleUrls: ['./personnel-details-allocations.component.scss']
})
export class PersonnelDetailsAllocationsComponent implements OnInit {
  
  @Input() personnel: ViewPersonnelSimplified;
  equipmentFilter: EquipmentFilter;
  @ViewChild('includeEquipmentLabels') includeEquipmentLabels: IncludeEquipmentLabelsComponent;
  defaultLabels = ['Equipment'];

  constructor(
    private lazyLoader: LazyLoaderService,
    private dialogService: DialogService) { 
  }

  ngOnInit(): void {
    if(this.personnel == undefined)
      return;
    
    let filter = new EquipmentFilter();
    filter.personnel = [this.personnel.id];
    filter.includeLabels = this.includeEquipmentLabels?.value ?? this.defaultLabels;
    this.equipmentFilter = Object.assign(new EquipmentFilter(), filter);
  }

  async generateReport(){
    await this.lazyLoader.loadModuleAsync('reports/report-from-filter.module.ts');
    this.dialogService.openDialog(
      ReportFromFilterComponent,
      [
        { label: 'equipmentFilter', value: this.equipmentFilter }
      ]
    );
  }

  includeLabelsChanged(){
    this.equipmentFilter.includeLabels = this.includeEquipmentLabels?.value ?? this.defaultLabels;
    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
  }

  refreshAgGrid(){
    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
  }
}
