import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { IOption } from 'src/app/models/option.model';
import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';
import { EntityLogsListComponent } from '../../../entity-logs-list/entity-logs-list.component';

@Component({
  selector: 'app-equipment-details-logs',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    EntityLogsListComponent
  ],
  templateUrl: './equipment-details-logs.component.html',
  styleUrls: ['./equipment-details-logs.component.scss']
})
export class EquipmentDetailsLogsComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  equipmentOption: IOption;

  constructor() { 
  }

  ngOnInit(): void {
    if(this.equipment != undefined)
      this.equipmentOption = { 
        value: this.equipment.id,
        label: this.equipment.temsIdOrSerialNumber,
      };
  }
}
