import { AnnouncementModule } from './../announcement/announcement.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { RouterModule } from '@angular/router';
import { EmailModule } from './../email/email/email.module';
import { CommunicationService } from '../../services/communication.service';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommunicationRoutingModule } from './communication-routing.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { ViewLogsComponent } from 'src/app/tems-components/communication/view-logs/view-logs.component';

@NgModule({
  declarations: [
    ViewLogsComponent,
  ],
  imports: [
    CommonModule,
    CommunicationRoutingModule,
    MaterialModule,
    EntitySharedModule,
    EmailModule,
    RouterModule,
    AnnouncementModule,
    ReactiveFormsModule,
    FormsModule,
    FormlyModule
  ],
  providers: [
    CommunicationService,
  ]
})
export class CommunicationModule { }
