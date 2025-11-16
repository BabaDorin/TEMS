import { ConfirmService } from './../../confirm.service';
import { EquipmentLabelComponent } from './../equipment/equipment-label/equipment-label.component';
import { TEMSComponent } from './../../tems/tems.component';
import { EquipmentService } from './../../services/equipment.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-temsid-generator',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    TranslateModule,
    MatIconModule,
    EquipmentLabelComponent
  ],
  templateUrl: './temsid-generator.component.html',
  styleUrls: ['./temsid-generator.component.scss']
})
export class TemsidGeneratorComponent extends TEMSComponent implements OnInit, OnDestroy {

  temsIdValue = "TEMSID";
  @ViewChild('equipmentLabel') equipmentLabel: EquipmentLabelComponent;

  subscriptions: any[] = [];

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

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => {
      try { s?.unsubscribe?.(); } catch (e) { /* ignore cleanup errors */ }
    });
  }
}
