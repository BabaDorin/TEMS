import { CommunicationService } from './../../services/communication-service/communication.service';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { ViewAnnouncement } from './../../models/communication/announcement/view-announcement.model';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommunicationRoutingModule } from './communication-routing.module';
import { ViewAnnouncementsComponent } from 'src/app/tems-components/communication/view-announcements/view-announcements.component';
import { AnnouncementsListComponent } from 'src/app/announcements-list/announcements-list.component';


@NgModule({
  declarations: [
    ViewAnnouncementsComponent,
    AnnouncementsListComponent,
  ],
  imports: [
    CommonModule,
    CommunicationRoutingModule,
    MaterialModule,
  ],
  providers: [
    CommunicationService,
  ]
})
export class CommunicationModule { }
