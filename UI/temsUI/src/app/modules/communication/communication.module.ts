import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { CommunicationService } from './../../services/communication-service/communication.service';
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
    TemsFormsModule,
  ],
  providers: [
    CommunicationService,
  ]
})
export class CommunicationModule { }
