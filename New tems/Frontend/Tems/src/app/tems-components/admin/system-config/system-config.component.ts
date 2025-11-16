import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { TEMS_FORMS_IMPORTS } from 'src/app/modules/tems-forms/tems-forms.module';

import { SystemConfigurationService } from 'src/app/services/system-configuration.service';
import { SnackService } from '../../../services/snack.service';
import { AppSettings } from './../../../models/system-configuration/app-settings.model';
import { EmailSenderCredentials } from './../../../models/system-configuration/emai-sender.model';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-system-config',
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
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './system-config.component.html',
  styleUrls: ['./system-config.component.scss']
})
export class SystemConfigComponent extends TEMSComponent implements OnInit {

  appSettingsModel: AppSettings;

  spaceConstraintsFormGroup = new FormGroup({
    libraryAllocatedStorageSpace: new FormControl(),
    // generatedReportsLimit: new FormControl(),
  });
  
  emailConfigFormGroup = new FormGroup({
    senderAddress: new FormControl(),
    senderPassword: new FormControl()
  });

  timeConfigFormGroup = new FormGroup({
    routineInterval: new FormControl(),
    archiveInterval: new FormControl()
  });

  guestTicketConfigFormGroup = new FormGroup({
    creationAllowance: new FormControl(),
  });

  libraryPassFormGroup = new FormGroup({
    libraryPassword: new FormControl()
  });

  constructor(
    private systemConfigurationService: SystemConfigurationService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.fetchAppSettings();
  }

  fetchAppSettings(){
    this.subscriptions.push(
      this.systemConfigurationService.getAppSettingsModel()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.appSettingsModel = result;

        this.spaceConstraintsFormGroup.setValue({libraryAllocatedStorageSpace: result.libraryAllocatedStorageSpace});
        this.timeConfigFormGroup.setValue({routineInterval: result.routineCheckInterval, archiveInterval: result.archiveInterval});
        this.guestTicketConfigFormGroup.setValue({creationAllowance: result.allowGuestsToCreateTickets});
        this.libraryPassFormGroup.setValue({libraryPassword: result.libraryGuestPassword});
      })
    )
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

  setArchiveInterval(){
    let archieveInterval = this.timeConfigFormGroup.controls.archiveInterval.value;

    this.subscriptions.push(
      this.systemConfigurationService.setArchieveInterval(archieveInterval)
      .subscribe(result => {
        this.snackService.snack(result);
      })
    )
  }

  onSubmit_libraryPassFormGroup(){
    let newPass = this.libraryPassFormGroup.controls.libraryPassword.value;

    this.subscriptions.push(
      this.systemConfigurationService.setLibraryPassword(newPass)
      .subscribe(result => {
        this.snackService.snack(result);
      })
    );
  }

  guestTicketCreationAllowanceChanged(){
    let flag = this.guestTicketConfigFormGroup.controls.creationAllowance.value;
    this.subscriptions.push(
      this.systemConfigurationService.guestTicketCreationAllowanceChanged(flag)
      .subscribe()
    )
  }
}
