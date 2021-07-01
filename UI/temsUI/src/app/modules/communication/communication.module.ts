import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { RouterModule } from '@angular/router';
import { FormlyModule } from '@ngx-formly/core';
import { TranslateModule } from '@ngx-translate/core';
import { ViewLogsComponent } from 'src/app/tems-components/communication/view-logs/view-logs.component';
import { CommunicationService } from '../../services/communication.service';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { AnnouncementModule } from './../announcement/announcement.module';
import { EmailModule } from './../email/email/email.module';
import { CommunicationRoutingModule } from './communication-routing.module';


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
    TranslateModule,
    MatMenuModule,
    MatIconModule,
    TemsFormsModule,
  ],
  providers: [
    CommunicationService,
  ]
})
export class CommunicationModule { }
