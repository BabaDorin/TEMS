import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { EquipmentService } from 'src/app/services/equipment.service';
import { ViewEquipmentSimplified } from './../../../models/equipment/view-equipment-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-equipment-details',
  templateUrl: './equipment-details.component.html',
  styleUrls: ['./equipment-details.component.scss']
})
export class EquipmentDetailsComponent extends TEMSComponent implements OnInit {

  @Input() equipmentId;
  edit: boolean;
  equipmentSimplified = new ViewEquipmentSimplified();
  mainHeaderLabel="General";

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
