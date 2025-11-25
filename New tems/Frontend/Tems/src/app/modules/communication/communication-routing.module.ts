import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CanSendEmailGuard } from './../../guards/can-send-email.guard';
import { CanViewEntitiesGuard } from './../../guards/can-view-entities.guard';
import { ViewAnnouncementsComponent } from './../../tems-components/communication/view-announcements/view-announcements.component';
import { ViewLogsComponent } from './../../tems-components/communication/view-logs/view-logs.component';
import { SendEmailComponent } from './../../tems-components/send-email/send-email.component';

const routes: Routes = [
  { path: 'announcements', component: ViewAnnouncementsComponent },
  { path: 'logs', component: ViewLogsComponent, canActivate: [CanViewEntitiesGuard] },
  // { path: 'dashboard', component: ViewLogsComponent },
  { path: 'sendemail', component: SendEmailComponent, canActivate: [CanSendEmailGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CommunicationRoutingModule { }
