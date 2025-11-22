import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { IOption } from 'src/app/models/option.model';
import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';
import { SummaryIssuesAnalyticsComponent } from '../../../analytics/summary-issues-analytics/summary-issues-analytics.component';
import { EntityIssuesListComponent } from '../../../entity-issues-list/entity-issues-list.component';

@Component({
  selector: 'app-equipment-details-issues',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    SummaryIssuesAnalyticsComponent,
    EntityIssuesListComponent
  ],
  templateUrl: './equipment-details-issues.component.html',
  styleUrls: ['./equipment-details-issues.component.scss']
})
export class EquipmentDetailsIssuesComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  equipmentAlreadySelected : IOption;
  constructor() { }

  ngOnInit(): void {
    this.equipmentAlreadySelected = {
      value: this.equipment.id,
      label: this.equipment.temsIdOrSerialNumber,
    }
  }
}
