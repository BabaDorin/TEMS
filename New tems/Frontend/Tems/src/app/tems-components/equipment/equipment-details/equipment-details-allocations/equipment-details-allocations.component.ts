import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { ViewEquipmentSimplified } from '../../../../models/equipment/view-equipment-simplified.model';
import { EntityAllocationsListComponent } from '../../../entity-allocations-list/entity-allocations-list.component';

@Component({
  selector: 'app-equipment-details-allocations',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    TranslateModule,
    EntityAllocationsListComponent
  ],
  templateUrl: './equipment-details-allocations.component.html',
  styleUrls: ['./equipment-details-allocations.component.scss']
})
export class EquipmentDetailsAllocationsComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;

  constructor() {
  }

  ngOnInit(): void {
  }

}
