import { DialogService } from './../../../services/dialog.service';
import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { Component, Input, OnInit } from '@angular/core';
import { EquipmentFilter } from 'src/app/helpers/filters/equipment.filter';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { ReportFromFilterComponent } from '../../reports/report-from-filter/report-from-filter.component';

@Component({
  selector: 'app-personnel-details-allocations',
  templateUrl: './personnel-details-allocations.component.html',
  styleUrls: ['./personnel-details-allocations.component.scss']
})
export class PersonnelDetailsAllocationsComponent implements OnInit {
  
  @Input() personnel: ViewPersonnelSimplified;
  equipmentFilter: EquipmentFilter;

  constructor(
    private lazyLoader: LazyLoaderService,
    private dialogService: DialogService) { 
    this.equipmentFilter = new EquipmentFilter();
  }

  ngOnInit(): void {
    if(this.personnel == undefined)
      return;
    
    this.equipmentFilter.personnel = [this.personnel.id];
    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
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
}
