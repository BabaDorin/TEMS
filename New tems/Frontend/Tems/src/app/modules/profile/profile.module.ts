import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { AnalyticsModule } from 'src/app/modules/analytics/analytics.module';
import { EntitySharedModule } from 'src/app/modules/entity-shared/entity-shared.module';
import { ProfileAllocationsComponent } from './../../tems-components/profile/profile-allocations/profile-allocations.component';
import { ProfileAnalyticsComponent } from './../../tems-components/profile/profile-analytics/profile-analytics.component';
import { ProfileGeneralComponent } from './../../tems-components/profile/profile-general/profile-general.component';
import { ProfileSettingsComponent } from './../../tems-components/profile/profile-settings/profile-settings.component';
import { ProfileTicketsComponent } from './../../tems-components/profile/profile-tickets/profile-tickets.component';
import { ViewProfileComponent } from './../../tems-components/view-profile/view-profile.component';
import { TEMS_FORMS_IMPORTS } from './../tems-forms/tems-forms.module';
import { ProfileRoutingModule } from './profile-routing.module';



@NgModule({
  declarations: [
  ],
  imports: [
    ViewProfileComponent,
    ProfileGeneralComponent,
    ProfileAllocationsComponent,
    ProfileTicketsComponent,
    ProfileSettingsComponent,
    ProfileAnalyticsComponent,
    CommonModule,
    ProfileRoutingModule,
    ...TEMS_FORMS_IMPORTS,
    AnalyticsModule,
    MatIconModule,
    TranslateModule,
    EntitySharedModule,
    MatButtonModule
  ]
})
export class ProfileModule { }
