import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AnnouncementModule } from './../announcement/announcement.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { RouterModule } from '@angular/router';
import { EmailModule } from './../email/email/email.module';
import { CommunicationService } from '../../services/communication.service';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommunicationRoutingModule } from './communication-routing.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { ViewLogsComponent } from 'src/app/tems-components/communication/view-logs/view-logs.component';
import { MatOptionModule } from '@angular/material/core';

@NgModule({
  declarations: [
    ViewLogsComponent,
  ],
  imports: [
    CommonModule,
    CommunicationRoutingModule,
    EntitySharedModule,
    EmailModule,
    RouterModule,
    AnnouncementModule,
    ReactiveFormsModule,
    MatInputModule,
    MatOptionModule,
    MatFormFieldModule,
    FormsModule,
    MatMenuModule,
    MatIconModule,
    FormlyModule
  ],
  providers: [
    CommunicationService,
  ]
})
export class CommunicationModule { }
