import { Component, Inject, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { ViewProfile } from 'src/app/models/profile/view-profile.model';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { SnackService } from 'src/app/services/snack.service';
import { UserService } from 'src/app/services/user.service';
import { AccountGeneralInfoModel } from './../../../models/identity/account-general-info.model';
import { ChangePasswordModel } from './../../../models/identity/change-password.model';
import { EmailPreferencesModel } from './../../../models/identity/email-preferences.model';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-profile-settings',
  templateUrl: './profile-settings.component.html',
  styleUrls: ['./profile-settings.component.scss']
})
export class ProfileSettingsComponent extends TEMSComponent implements OnInit {

  @Input() profile: ViewProfile;
  isCurrentUser: boolean;

  changePasswordFormlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  emailPreferencesFormlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  accountGeneralInfoFormlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    @Inject(ViewProfile) prof,
    @Inject(Boolean) isCurrentUser,
    private userService: UserService,
    private snackService: SnackService,
    private router: Router,
    private formlyParserService: FormlyParserService) {
    super();
    console.log(prof);
    this.profile = prof;
    this.isCurrentUser = isCurrentUser;
  };

  ngOnInit(): void {
    if(!this.isCurrentUser)
      this.router.navigate(['/error-pages/403'])


    this.changePasswordFormlyData.fields = this.formlyParserService.parseChangePassword();

    this.emailPreferencesFormlyData.fields = this.formlyParserService.parseChangeEmailPreferences();
    this.emailPreferencesFormlyData.model = {
      email: this.profile.email,
      getNotifications: this.profile.getEmailNotifications
    }

    this.accountGeneralInfoFormlyData.fields = this.formlyParserService.parseAccountGeneralInfo();
    this.accountGeneralInfoFormlyData.model = {
      fullName: this.profile.fullName,
      username: this.profile.username,
    }
  };

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

  editGeneralInfo(model){
    let accountGeneralInfoModel: AccountGeneralInfoModel = model;
    accountGeneralInfoModel.userId = this.profile.id;
    
    this.userService.editAccountGeneralInfo(accountGeneralInfoModel)
      .subscribe(result => {
        this.snackService.snack(result);
      });
  }
}
