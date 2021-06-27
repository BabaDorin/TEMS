import { MaterialModule } from '../material/material.module';
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
    MaterialModule
  ]
})
export class AuthenticationModule { }


