import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { AddAnnouncementComponent } from './../../add-announcement/add-announcement.component';
import { CommunicationService } from './../../services/communication-service/communication.service';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommunicationRoutingModule } from './communication-routing.module';
import { AnnouncementsListComponent } from 'src/app/announcements-list/announcements-list.component';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { ViewLogsComponent } from 'src/app/tems-components/communication/view-logs/view-logs.component';

@NgModule({
  declarations: [
    AnnouncementsListComponent,
    AddAnnouncementComponent,
    ViewLogsComponent,
  ],
  imports: [
    CommonModule,
    CommunicationRoutingModule,
    MaterialModule,
    EntitySharedModule,
    TemsFormsModule,
  ],
  providers: [
    CommunicationService,
  ]
})
export class CommunicationModule { }
