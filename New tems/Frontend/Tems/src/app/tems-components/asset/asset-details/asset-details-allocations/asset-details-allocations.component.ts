import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { ViewAssetSimplified } from '../../../../models/asset/view-asset-simplified.model';
import { EntityAllocationsListComponent } from '../../../entity-allocations-list/entity-allocations-list.component';

@Component({
  selector: 'app-asset-details-allocations',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    TranslateModule,
    EntityAllocationsListComponent
  ],
  templateUrl: './asset-details-allocations.component.html',
  styleUrls: ['./asset-details-allocations.component.scss']
})
export class AssetDetailsAllocationsComponent implements OnInit {

  @Input() asset: ViewAssetSimplified;

  constructor() {
  }

  ngOnInit(): void {
  }

}
