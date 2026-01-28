import { ConfirmService } from './../../confirm.service';
import { AssetLabelComponent } from './../asset/asset-label/asset-label.component';
import { TEMSComponent } from './../../tems/tems.component';
import { AssetService } from './../../services/asset.service';
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
    AssetLabelComponent
  ],
  templateUrl: './temsid-generator.component.html',
  styleUrls: ['./temsid-generator.component.scss']
})
export class TemsidGeneratorComponent extends TEMSComponent implements OnInit, OnDestroy {

  temsIdValue = "TEMSID";
  @ViewChild('assetLabel') assetLabel: AssetLabelComponent;

  subscriptions: any[] = [];

  constructor(
    private translate: TranslateService,
    private assetService: AssetService,
    private confirmService: ConfirmService) {
    super();
  }

  ngOnInit(): void {
  }

  async download(){
    this.subscriptions.push(
      this.assetService.isTEMSIDAvailable(this.temsIdValue)
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
    this.assetLabel.downloadLabel();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => {
      try { s?.unsubscribe?.(); } catch (e) { /* ignore cleanup errors */ }
    });
  }
}
