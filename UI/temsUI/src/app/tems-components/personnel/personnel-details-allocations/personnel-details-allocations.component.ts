import { DialogService } from './../../../services/dialog.service';
import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { EquipmentFilter } from 'src/app/helpers/filters/equipment.filter';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { ReportFromFilterComponent } from '../../reports/report-from-filter/report-from-filter.component';
import { IncludeEquipmentLabelsComponent } from 'src/app/shared/include-equipment-tags/include-equipment-tags.component';

@Component({
  selector: 'app-personnel-details-allocations',
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
    console.log('includeChanged');
  }
}
