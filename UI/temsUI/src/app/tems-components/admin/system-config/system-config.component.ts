import { SnackService } from './../../../services/snack/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { SystemConfigurationService } from 'src/app/services/system-configuration.service';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-system-config',
  templateUrl: './system-config.component.html',
  styleUrls: ['./system-config.component.scss']
})
export class SystemConfigComponent extends TEMSComponent implements OnInit {

  spaceConstraintsFormGroup = new FormGroup({
    libraryLimit: new FormControl(),
    generatedReportsLimit: new FormControl(),
  });
  
  emailConfigFormGroup = new FormGroup({
    senderAddress: new FormControl(),
    senderPassword: new FormControl()
  });

  timeConfigFormGroup = new FormGroup({
    routineInterval: new FormControl(),
    archievePeriod: new FormControl()
  });

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

  onSubmit_spaceConstraints(){
    console.log(this.spaceConstraintsFormGroup);
  }

  onSubmit_emailConfigFormGroup(){
    console.log(this.emailConfigFormGroup);
  }

  onSubmit_timeConfigFormGroup(){
    console.log(this.timeConfigFormGroup);
  }
}
