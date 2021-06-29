import { TranslateModule } from '@ngx-translate/core';
import { MatIconModule } from '@angular/material/icon';
import { AnalyticsModule } from 'src/app/modules/analytics/analytics.module';
import { ProfileAnalyticsComponent } from './../../tems-components/profile/profile-analytics/profile-analytics.component';
import { EntitySharedModule } from 'src/app/modules/entity-shared/entity-shared.module';
import { ProfileSettingsComponent } from './../../tems-components/profile/profile-settings/profile-settings.component';
import { ProfileTicketsComponent } from './../../tems-components/profile/profile-tickets/profile-tickets.component';
import { ProfileAllocationsComponent } from './../../tems-components/profile/profile-allocations/profile-allocations.component';
import { ProfileGeneralComponent } from './../../tems-components/profile/profile-general/profile-general.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { ViewProfileComponent } from './../../tems-components/view-profile/view-profile.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProfileRoutingModule } from './profile-routing.module';


@NgModule({
  declarations: [
    ViewProfileComponent,
    ProfileGeneralComponent,
    ProfileAllocationsComponent,
    ProfileTicketsComponent,
    ProfileSettingsComponent,
    ProfileAnalyticsComponent,
  ],
  imports: [
    CommonModule,
    ProfileRoutingModule,
    TemsFormsModule,
    AnalyticsModule,
    MatIconModule,
    TranslateModule,
    EntitySharedModule,
  ]
})
export class ProfileModule { }
