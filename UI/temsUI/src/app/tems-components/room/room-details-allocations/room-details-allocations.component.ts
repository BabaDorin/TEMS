import { DialogService } from './../../../services/dialog.service';
import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { EquipmentFilter } from './../../../helpers/filters/equipment.filter';
import { Component, Input, OnInit } from '@angular/core';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { ReportFromFilterComponent } from '../../reports/report-from-filter/report-from-filter.component';

@Component({
  selector: 'app-room-details-allocations',
  templateUrl: './room-details-allocations.component.html',
  styleUrls: ['./room-details-allocations.component.scss']
})
export class RoomDetailsAllocationsComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  equipmentFilter: EquipmentFilter;

  constructor(
    private lazyLoader: LazyLoaderService,
    private dialogService: DialogService
  ) { 
    this.equipmentFilter = new EquipmentFilter();
  }

  ngOnInit(): void {
    if(this.room == undefined)
      return;
    
    this.equipmentFilter.rooms = [this.room.id];
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
