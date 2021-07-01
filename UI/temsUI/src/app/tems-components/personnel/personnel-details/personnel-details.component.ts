import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ViewPersonnel } from 'src/app/models/personnel/view-personnel.model';
import { PersonnelService } from '../../../services/personnel.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-personnel-details',
  templateUrl: './personnel-details.component.html',
  styleUrls: ['./personnel-details.component.scss']
})
export class PersonnelDetailsComponent extends TEMSComponent implements OnInit {

  @Input() personnelId;
  edit: boolean = false;
  personnelSimplified = new ViewPersonnelSimplified();
  personnel: ViewPersonnel;
  mainHeaderLabel="General";

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
        console.log(result)
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
