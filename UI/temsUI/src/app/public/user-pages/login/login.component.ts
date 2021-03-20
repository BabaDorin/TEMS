import { UserService } from 'src/app/services/user-service/user.service';
import { LoginModel } from './../../../models/identity/login.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent extends TEMSComponent implements OnInit {

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }
  
  constructor(
    private formlyParserService: FormlyParserService,
    private userService: UserService
  ) { 
    super();
  }

  ngOnInit() {
    this.formlyData.fields = this.formlyParserService.parseLogin();
  }

  onSubmit(){
    let loginModel: LoginModel = this.formlyData.model.login;

    this.subscriptions.push(
      this.userService.logIn(loginModel)
      .subscribe(result => {
        console.log(result);
      })
    )
  }
}
