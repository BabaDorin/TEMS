import { LogsService } from './../../services/logs-service/logs.service';
import { SnackService } from './../../services/snack/snack.service';
import { EquipmentService } from './../../services/equipment-service/equipment.service';
import { TEMSComponent } from './../../tems/tems.component';
import { ViewLog } from './../../models/communication/logs/view-logs.model';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-log-container',
  templateUrl: './log-container.component.html',
  styleUrls: ['./log-container.component.scss']
})
export class LogContainerComponent extends TEMSComponent implements OnInit {

  @Input() log: ViewLog;
  @Output() removed = new EventEmitter();

  constructor(
    private logService: LogsService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
  }

  remove(){
    this.subscriptions.push(
      this.logService.archieve(this.log.id, true)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.removed.emit();
      })
    )
  }
}
