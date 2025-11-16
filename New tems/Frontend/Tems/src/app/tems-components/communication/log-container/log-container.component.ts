import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ViewLog } from '../../../models/communication/logs/view-logs.model';
import { LogsService } from '../../../services/logs.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from '../../../tems/tems.component';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-log-container',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatCardModule,
    TranslateModule
  ],
  templateUrl: './log-container.component.html',
  styleUrls: ['./log-container.component.scss']
})
export class LogContainerComponent extends TEMSComponent implements OnInit {

  @Input() log: ViewLog;
  @Input() canManage: boolean = false;
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
    if(!this.canManage)
      return;

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
