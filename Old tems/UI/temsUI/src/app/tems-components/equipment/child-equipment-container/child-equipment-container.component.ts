import { ConfirmService } from './../../../confirm.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AttachEquipment } from 'src/app/models/equipment/attach-equipment.model';
import { IOption } from '../../../models/option.model';
import { EquipmentService } from '../../../services/equipment.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from '../../../tems/tems.component';

@Component({
  selector: 'app-child-equipment-container',
  templateUrl: './child-equipment-container.component.html',
  styleUrls: ['./child-equipment-container.component.scss']
})
export class ChildEquipmentContainerComponent extends TEMSComponent implements OnInit {

  @Input() childEquipment: IOption;
  @Input() canManage: boolean = false;
  @Input() detachable: boolean = true;
  @Input() isAttached: boolean = true;
  @Input() parentId: string;

  @Output() detached = new EventEmitter();
  @Output() attached = new EventEmitter();

  constructor(
    private equipmentService: EquipmentService,
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
      this.equipmentService.detach(this.childEquipment.value)
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
      this.equipmentService.attach(attachChildModel)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.attached.emit();
        })
    );
  }
}
