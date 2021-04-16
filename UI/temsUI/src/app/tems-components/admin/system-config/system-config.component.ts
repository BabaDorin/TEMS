import { SnackService } from './../../../services/snack/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { SystemConfigurationService } from 'src/app/services/system-configuration.service';

@Component({
  selector: 'app-system-config',
  templateUrl: './system-config.component.html',
  styleUrls: ['./system-config.component.scss']
})
export class SystemConfigComponent extends TEMSComponent implements OnInit {

  constructor(
    private systemConfigurationService: SystemConfigurationService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {

  }

  integrateSIC(){
    this.subscriptions.push(
      this.systemConfigurationService.integrateSIC()
      .subscribe(result => {
        this.snackService.snack(result);
      })
    )
  }
}
