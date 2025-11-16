import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatBadgeModule } from '@angular/material/badge';
import { TranslateModule } from '@ngx-translate/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MatTabLazyLoader } from 'src/app/helpers/mat-tab-lazy-loader.helper';
import { ViewPersonnel } from 'src/app/models/personnel/view-personnel.model';
import { PersonnelService } from '../../../services/personnel.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelDetailsGeneralComponent } from '../personnel-details-general/personnel-details-general.component';
import { PersonnelDetailsLogsComponent } from '../personnel-details-logs/personnel-details-logs.component';
import { PersonnelDetailsIssuesComponent } from '../personnel-details-issues/personnel-details-issues.component';
import { PersonnelDetailsAllocationsComponent } from '../personnel-details-allocations/personnel-details-allocations.component';

@Component({
  selector: 'app-personnel-details',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MatCardModule,
    MatBadgeModule,
    TranslateModule,
    PersonnelDetailsGeneralComponent,
    PersonnelDetailsLogsComponent,
    PersonnelDetailsIssuesComponent,
    PersonnelDetailsAllocationsComponent
  ],
  templateUrl: './personnel-details.component.html',
  styleUrls: ['./personnel-details.component.scss']
})
export class PersonnelDetailsComponent extends TEMSComponent implements OnInit {

  personnelId;
  edit: boolean = false;
  personnelSimplified = new ViewPersonnelSimplified();
  personnel: ViewPersonnel;
  mainHeaderLabel="General";
  matTabLazyLoader = new MatTabLazyLoader(4);

  constructor(
    private activatedroute: ActivatedRoute,
    private personnelService: PersonnelService,
    public translate: TranslateService
  ) { 
    super();
  }

  ngOnInit(): void {
    if(this.personnelId == undefined)
      this.personnelId = this.activatedroute.snapshot.paramMap.get("id");
      
    this.subscriptions.push(this.personnelService.getPersonnelById(this.personnelId)
      .subscribe(result => {
        this.personnel = result;

        this.personnelSimplified = this.personnelService.getPersonnelSimplifiedFromPersonnel(this.personnel);

        if(this.personnelSimplified.isArchieved)
          this.mainHeaderLabel += " (" + this.translate.instant('archive.archived') + ")";
      }));
  }

  archivationStatusChanged(){
    this.mainHeaderLabel = "General"

    this.personnelSimplified.isArchieved = !this.personnelSimplified.isArchieved;
    if(this.personnelSimplified.isArchieved)
          this.mainHeaderLabel += " (" + this.translate.instant('archive.archived') + ")";
  }
}
