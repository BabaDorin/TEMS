import { ConfirmService } from './../../../confirm.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { AttachEquipment } from 'src/app/models/asset/attach-asset.model';
import { IOption } from '../../../models/option.model';
import { AssetService } from '../../../services/asset.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from '../../../tems/tems.component';
import { AssetDetailsComponent } from '../asset-details/asset-details.component';

@Component({
  selector: 'app-child-asset-container',
  standalone: true,
  imports: [
    CommonModule,
    MatExpansionModule,
    MatIconModule,
    MatButtonModule,
    TranslateModule,
    AssetDetailsComponent
  ],
  templateUrl: './child-asset-container.component.html',
  styleUrls: ['./child-asset-container.component.scss']
})
export class ChildAssetContainerComponent extends TEMSComponent implements OnInit {

  @Input() childEquipment: IOption;
  @Input() canManage: boolean = false;
  @Input() detachable: boolean = true;
  @Input() isAttached: boolean = true;
  @Input() parentId: string;

  @Output() detached = new EventEmitter();
  @Output() attached = new EventEmitter();

  constructor(
    private assetService: AssetService,
    private snackService: SnackService,
    private confirmService: ConfirmService,
  ) {
    super();
  }

  ngOnInit(): void {
  }

  async detach() {
    if (!await this.confirmService.confirm("Are you sure you want to detach this equipment from it's parent?"))
      return;

    this.subscriptions.push(
      this.assetService.detach(this.childEquipment.value)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.detached.emit();
        })
    );
  }

  attach() {
    if (this.parentId == undefined) {
      this.snackService.snack({
        message: 'Parent id not specified',
        status: 1
      });
      return;
    }

    let attachChildModel = new AttachEquipment();
    attachChildModel.childrenIds = [this.childEquipment.value];
    attachChildModel.parentId = this.parentId;

    this.subscriptions.push(
      this.assetService.attach(attachChildModel)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.attached.emit();
        })
    );
  }
}
