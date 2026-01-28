import { SnackService } from 'src/app/services/snack.service';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AssetService } from 'src/app/services/asset.service';
import { ViewAssetSimplified } from './../../../models/asset/view-asset-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { MatTabLazyLoader } from 'src/app/helpers/mat-tab-lazy-loader.helper';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { AssetDetailsGeneralComponent } from './asset-details-general/asset-details-general.component';
import { AssetDetailsLogsComponent } from './asset-details-logs/asset-details-logs.component';
import { AssetDetailsIssuesComponent } from './asset-details-issues/asset-details-issues.component';
import { AssetDetailsAllocationsComponent } from './asset-details-allocations/asset-details-allocations.component';

@Component({
  selector: 'app-asset-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    TranslateModule,
    AssetDetailsGeneralComponent,
    AssetDetailsLogsComponent,
    AssetDetailsIssuesComponent,
    AssetDetailsAllocationsComponent
  ],
  templateUrl: './asset-details.component.html',
  styleUrls: ['./asset-details.component.scss']
})
export class AssetDetailsComponent extends TEMSComponent implements OnInit {

  @Input() assetId;
  edit: boolean;
  assetSimplified = new ViewAssetSimplified();
  mainHeaderLabel="General";
  matTabLazyLoader = new MatTabLazyLoader(4);

  constructor(
    private activatedroute: ActivatedRoute, 
    public translate: TranslateService,
    private assetService: AssetService) {
      super();
  }

  ngOnInit(): void {
    if(this.assetId == undefined)
      this.assetId = this.activatedroute.snapshot.paramMap.get("id");
    this.edit=false;

    this.subscriptions.push(this.assetService.getEquipmentSimplifiedById(this.assetId)
      .subscribe(result => {
        this.assetSimplified = result;

        if(this.assetSimplified.isArchieved)
          this.mainHeaderLabel += " (Archieved)"

      }));
  }

  archivationStatusChanged(){
    this.mainHeaderLabel = "General"

    this.assetSimplified.isArchieved = !this.assetSimplified.isArchieved;
    if(this.assetSimplified.isArchieved)
          this.mainHeaderLabel += " (" + this.translate.instant('general.archive')+')';
  }
}
