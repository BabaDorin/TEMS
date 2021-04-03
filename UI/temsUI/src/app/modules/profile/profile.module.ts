import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { MaterialModule } from './../material/material.module';
import { ViewProfileComponent } from './../../tems-components/view-profile/view-profile.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProfileRoutingModule } from './profile-routing.module';


@NgModule({
  declarations: [
    ViewProfileComponent
  ],
  imports: [
    CommonModule,
    ProfileRoutingModule,
    TemsFormsModule,
  ]
})
export class ProfileModule { }
