import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { MATERIAL_MODULES } from 'src/app/shared/constants/material-modules.const';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { FormlyFieldConfig, FormlyModule } from '@ngx-formly/core';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { SnackService } from '../../../services/snack.service';
import { LoginModel } from './../../../models/identity/login.model';
import { AuthService } from './../../../services/auth.service';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    TranslateModule,
    FormlyModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './login.component.html'
})
export class LoginComponent extends TEMSComponent implements OnInit {

  public formlyData = {
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
    
    // Check if user is already logged in
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/tems']);
    }
  }

  onSubmit(){
    // Redirect to IdentityServer for OIDC authentication
    this.authService.logIn();
  }

  forgotPassword(){
    this.snackService.snack({message: 'Ask system administrator to provide you a new password', status: 1}, 'default-snackbar');
  }
}
