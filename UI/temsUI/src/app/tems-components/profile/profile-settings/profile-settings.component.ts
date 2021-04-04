import { EmailPreferencesModel } from './../../../models/identity/email-preferences.model';
import { ChangePasswordModel } from './../../../models/identity/change-password.model';
import { UserService } from 'src/app/services/user-service/user.service';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { FormGroup, FormControl } from '@angular/forms';
import { TEMSComponent } from './../../../tems/tems.component';
import { Component, Input, OnInit } from '@angular/core';
import { ViewProfile } from 'src/app/models/profile/view-profile.model';
import { SnackService } from 'src/app/services/snack/snack.service';
import { FormlyFieldConfig } from '@ngx-formly/core';

@Component({
  selector: 'app-profile-settings',
  templateUrl: './profile-settings.component.html',
  styleUrls: ['./profile-settings.component.scss']
})
export class ProfileSettingsComponent extends TEMSComponent implements OnInit {

  @Input() profile: ViewProfile;

  private changePasswordFormlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  private emailPreferencesFormlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    prof: ViewProfile,
    private userService: UserService,
    private snackService: SnackService,
    private formlyParserService: FormlyParserService) {
    super();
    console.log(prof);
    this.profile = prof;
  }

  ngOnInit(): void {
    this.changePasswordFormlyData.fields = this.formlyParserService.parseChangePassword();
    this.emailPreferencesFormlyData.fields = this.formlyParserService.parseChangeEmailPreferences();
    this.emailPreferencesFormlyData.model = {
      email: this.profile.email,
      getNotifications: this.profile.getEmailNotifications
    }
  }  

  changePass(model){

    let changePasswordModel: ChangePasswordModel = model;
    changePasswordModel.userId = this.profile.id;

    this.subscriptions.push(
      this.userService.changePassword(changePasswordModel)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.changePasswordFormlyData.model = {};
      })
    )
  }

  saveEmailPreferences(model){
    let emailPreferencesModel: EmailPreferencesModel = model;
    emailPreferencesModel.userId = this.profile.id;

    this.userService.changeEmailPreferences(emailPreferencesModel)
      .subscribe(result => {
        this.snackService.snack(result);
      });
  }
}
