import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { IOption } from 'src/app/models/option.model';
import { ViewAssetSimplified } from './../../../../models/asset/view-asset-simplified.model';
import { SummaryIssuesAnalyticsComponent } from '../../../analytics/summary-issues-analytics/summary-issues-analytics.component';
import { EntityIssuesListComponent } from '../../../entity-issues-list/entity-issues-list.component';

@Component({
  selector: 'app-asset-details-issues',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    SummaryIssuesAnalyticsComponent,
    EntityIssuesListComponent
  ],
  templateUrl: './asset-details-issues.component.html',
  styleUrls: ['./asset-details-issues.component.scss']
})
export class AssetDetailsIssuesComponent implements OnInit {

  @Input() asset: ViewAssetSimplified;
  assetAlreadySelected : IOption;
  constructor() { }

  ngOnInit(): void {
    this.assetAlreadySelected = {
      value: this.asset.id,
      label: this.asset.temsIdOrSerialNumber,
    }
  }
}
