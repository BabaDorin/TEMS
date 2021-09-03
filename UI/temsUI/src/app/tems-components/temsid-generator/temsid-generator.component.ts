import { ConfirmService } from './../../confirm.service';
import { EquipmentLabelComponent } from './../equipment/equipment-label/equipment-label.component';
import { TEMSComponent } from './../../tems/tems.component';
import { EquipmentService } from './../../services/equipment.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-temsid-generator',
  templateUrl: './temsid-generator.component.html',
  styleUrls: ['./temsid-generator.component.scss']
})
export class TemsidGeneratorComponent extends TEMSComponent implements OnInit {

  temsIdValue = "TEMSID";
  @ViewChild('equipmentLabel') equipmentLabel: EquipmentLabelComponent;

  constructor(
    private translate: TranslateService,
    private equipmentService: EquipmentService,
    private confirmService: ConfirmService) {
    super();
  }

  ngOnInit(): void {
  }

  async download(){
    this.subscriptions.push(
      this.equipmentService.isTEMSIDAvailable(this.temsIdValue)
      .subscribe(async result => {
        if(result == false && !await this.confirmService.confirm(this.translate.instant('TEMSID.TEMSIDNotAvailable_DownloadAnyway')))
        {
          return;
        }

        this.downloadNonQuery();
      })
    ) 
  }

  downloadNonQuery(){
    this.equipmentLabel.downloadLabel();
  }
}
