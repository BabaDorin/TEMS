import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { IOption } from 'src/app/models/option.model';
import { ViewAssetSimplified } from './../../../../models/asset/view-asset-simplified.model';
import { EntityLogsListComponent } from '../../../entity-logs-list/entity-logs-list.component';

@Component({
  selector: 'app-asset-details-logs',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    EntityLogsListComponent
  ],
  templateUrl: './asset-details-logs.component.html',
  styleUrls: ['./asset-details-logs.component.scss']
})
export class AssetDetailsLogsComponent implements OnInit {

  @Input() asset: ViewAssetSimplified;
  assetOption: IOption;

  constructor() { 
  }

  ngOnInit(): void {
    if(this.asset != undefined)
      this.assetOption = { 
        value: this.asset.id,
        label: this.asset.temsIdOrSerialNumber,
      };
  }
}
