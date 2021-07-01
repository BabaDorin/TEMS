import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { LoginComponent } from '../../public/user-pages/login/login.component';
import { TemsFormsModule } from '../tems-forms/tems-forms.module';
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


