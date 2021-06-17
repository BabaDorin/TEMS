import { CanSendEmailGuard } from './../../guards/can-send-email.guard';
import { CanManageSystemConfigurationGuard } from './../../guards/can-manage-system-configuration.guard';
import { SendEmailComponent } from './../../tems-components/send-email/send-email.component';
import { CanViewEntitiesGuard } from './../../guards/can-view-entities.guard';
import { ViewLogsComponent } from './../../tems-components/communication/view-logs/view-logs.component';
import { ViewAnnouncementsComponent } from './../../tems-components/communication/view-announcements/view-announcements.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

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
