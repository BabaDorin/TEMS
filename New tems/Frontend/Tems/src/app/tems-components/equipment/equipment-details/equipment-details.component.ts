import { SnackService } from 'src/app/services/snack.service';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { EquipmentService } from 'src/app/services/equipment.service';
import { ViewEquipmentSimplified } from './../../../models/equipment/view-equipment-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { MatTabLazyLoader } from 'src/app/helpers/mat-tab-lazy-loader.helper';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { EquipmentDetailsGeneralComponent } from './equipment-details-general/equipment-details-general.component';
import { EquipmentDetailsLogsComponent } from './equipment-details-logs/equipment-details-logs.component';
import { EquipmentDetailsIssuesComponent } from './equipment-details-issues/equipment-details-issues.component';
import { EquipmentDetailsAllocationsComponent } from './equipment-details-allocations/equipment-details-allocations.component';

@Component({
  selector: 'app-equipment-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    TranslateModule,
    EquipmentDetailsGeneralComponent,
    EquipmentDetailsLogsComponent,
    EquipmentDetailsIssuesComponent,
    EquipmentDetailsAllocationsComponent
  ],
  templateUrl: './equipment-details.component.html',
  styleUrls: ['./equipment-details.component.scss']
})
export class EquipmentDetailsComponent extends TEMSComponent implements OnInit {

  @Input() equipmentId;
  edit: boolean;
  equipmentSimplified = new ViewEquipmentSimplified();
  mainHeaderLabel="General";
  matTabLazyLoader = new MatTabLazyLoader(4);

  constructor(
    private activatedroute: ActivatedRoute, 
    public translate: TranslateService,
    private equipmentService: EquipmentService) {
      super();
  }

  ngOnInit(): void {
    if(this.equipmentId == undefined)
      this.equipmentId = this.activatedroute.snapshot.paramMap.get("id");
    this.edit=false;

    this.subscriptions.push(this.equipmentService.getEquipmentSimplifiedById(this.equipmentId)
      .subscribe(result => {
        this.equipmentSimplified = result;

        if(this.equipmentSimplified.isArchieved)
          this.mainHeaderLabel += " (Archieved)"

      }));
  }

  archivationStatusChanged(){
    this.mainHeaderLabel = "General"

    this.equipmentSimplified.isArchieved = !this.equipmentSimplified.isArchieved;
    if(this.equipmentSimplified.isArchieved)
          this.mainHeaderLabel += " (" + this.translate.instant('general.archive')+')';
  }
}
