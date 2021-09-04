import { IncludeEquipmentLabelsComponent } from './../../../shared/include-equipment-tags/include-equipment-tags.component';
import { DialogService } from './../../../services/dialog.service';
import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { EquipmentFilter } from './../../../helpers/filters/equipment.filter';
import { Component, Input, OnInit } from '@angular/core';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { ReportFromFilterComponent } from '../../reports/report-from-filter/report-from-filter.component';
import { ViewChild } from '@angular/core';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-room-details-allocations',
  templateUrl: './room-details-allocations.component.html',
  styleUrls: ['./room-details-allocations.component.scss']
})
export class RoomDetailsAllocationsComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  @ViewChild('includeEquipmentLabels') includeEquipmentLabels: IncludeEquipmentLabelsComponent;

  equipmentFilter: EquipmentFilter;
  defaultLabels = ['Equipment'];

  constructor(
    private lazyLoader: LazyLoaderService,
    private dialogService: DialogService
  ) { 
    this.equipmentFilter = new EquipmentFilter();
  }

  ngOnInit(): void {
    if(this.room == undefined)
      return;
    
    let filter = new EquipmentFilter();
    filter.rooms = [this.room.id];
    filter.includeLabels = this.includeEquipmentLabels?.value ?? this.defaultLabels;
    this.equipmentFilter = Object.assign(new EquipmentFilter(), filter);
    console.log('ngOnInit');
  }

  includeLabelsChanged(){
    this.equipmentFilter.includeLabels = this.includeEquipmentLabels?.value ?? this.defaultLabels;
    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
    console.log('includeChanged');
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
