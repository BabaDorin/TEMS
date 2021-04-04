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
  }  

  changePass(model){
    this.subscriptions.push(
      this.userService.changePassword(model)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.changePasswordFormlyData.model = {};
      })
    )
  }
}
