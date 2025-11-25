import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { LoginComponent } from '../../public/user-pages/login/login.component';
import { TEMS_FORMS_IMPORTS } from '../tems-forms/tems-forms.module';
import { AuthenticationRoutingModule } from './authentication-routing.module';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    LoginComponent,
    AuthenticationRoutingModule,
    ...TEMS_FORMS_IMPORTS,
    TranslateModule,
    MatButtonModule
  ]
})
export class AuthenticationModule { }


