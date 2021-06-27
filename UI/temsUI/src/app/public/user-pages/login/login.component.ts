import { SnackService } from '../../../services/snack.service';
import { AuthService } from './../../../services/auth.service';
import { Router } from '@angular/router';
import { UserService } from 'src/app/services/user.service';
import { LoginModel } from './../../../models/identity/login.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { Component, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import * as EventEmitter from 'events';
import { emit } from 'process';

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
    private authService: AuthService,
    private snackService: SnackService,
    private router: Router
  ) { 
    super();
  }

  ngOnInit() {
    this.formlyData.fields = this.formlyParserService.parseLogin();
  }

  onSubmit(){
    let loginModel: LoginModel = this.formlyData.model.login;

    this.subscriptions.push(
      this.authService.logIn(loginModel)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        if(result.token != undefined){
          localStorage.setItem('token', result.token);
          this.router.navigateByUrl('').then(() => {window.location.reload()});
        }
      })
    )
  }

  forgotPassword(){
    this.snackService.snack({message: 'Ask system administrator to provide you a new password', status: 1}, 'default-snackbar');
  }
}
