import { IncludeEquipmentLabelsComponent } from './../../../shared/include-equipment-tags/include-equipment-tags.component';
import { DialogService } from './../../../services/dialog.service';
import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { EquipmentFilter } from './../../../helpers/filters/equipment.filter';
import { Component, Input, OnInit, NO_ERRORS_SCHEMA } from '@angular/core';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { ReportFromFilterComponent } from '../../reports/report-from-filter/report-from-filter.component';
import { ViewChild } from '@angular/core';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-room-details-allocations',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    TranslateModule
  ],
  schemas: [NO_ERRORS_SCHEMA],
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
  }

  includeLabelsChanged(){
    this.equipmentFilter.includeLabels = this.includeEquipmentLabels?.value ?? this.defaultLabels;
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
