import { TranslateModule } from '@ngx-translate/core';
import { TemsFormsModule } from '../tems-forms/tems-forms.module';
import { LoginComponent } from '../../public/user-pages/login/login.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthenticationRoutingModule } from './authentication-routing.module';


@NgModule({
  declarations: [
    LoginComponent,
  ],
  imports: [
    CommonModule,
    AuthenticationRoutingModule,
    TemsFormsModule,
    TranslateModule,
  ]
})
export class AuthenticationModule { }


