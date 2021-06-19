import { EmailSenderCredentials } from './../../../models/system-configuration/emai-sender.model';
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
    libraryAllocatedStorageSpace: new FormControl(),
    generatedReportsLimit: new FormControl(),
  });
  
  emailConfigFormGroup = new FormGroup({
    senderAddress: new FormControl(),
    senderPassword: new FormControl()
  });

  timeConfigFormGroup = new FormGroup({
    routineInterval: new FormControl(),
    archieveInterval: new FormControl()
  });

  guestTicketConfigFormGroup = new FormGroup({
    creationAllowance: new FormControl(),
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
    let space = this.spaceConstraintsFormGroup.controls.libraryAllocatedStorageSpace.value;

    this.subscriptions.push(
      this.systemConfigurationService.setLibraryAllocateStorageSpace(space)
      .subscribe(result => {
        this.snackService.snack(result);
      })
    );
  }

  onSubmit_emailConfigFormGroup(){
    let emailSenderCredentialsModel = new EmailSenderCredentials;
    emailSenderCredentialsModel.address = this.emailConfigFormGroup.controls.senderAddress.value;
    emailSenderCredentialsModel.password = this.emailConfigFormGroup.controls.senderPassword.value;

    this.subscriptions.push(
      this.systemConfigurationService.setEmailSender(emailSenderCredentialsModel)
      .subscribe(result => {
        this.snackService.snack(result);
      })
    );
  }

  setRoutineCheckInterval(){
    let routineCheckInterval = this.timeConfigFormGroup.controls.routineInterval.value;

    this.subscriptions.push(
      this.systemConfigurationService.setRoutineCheckInterval(routineCheckInterval)
      .subscribe(result => {
        this.snackService.snack(result);
      })
    )
  }

  setArchieveInterval(){
    let archieveInterval = this.timeConfigFormGroup.controls.archieveInterval.value;

    this.subscriptions.push(
      this.systemConfigurationService.setArchieveInterval(archieveInterval)
      .subscribe(result => {
        this.snackService.snack(result);
      })
    )
  }
}
