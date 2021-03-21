import { CanViewEntitiesGuard } from './../../guards/can-view-entities.guard';
import { ViewLogsComponent } from './../../tems-components/communication/view-logs/view-logs.component';
import { ViewAnnouncementsComponent } from './../../tems-components/communication/view-announcements/view-announcements.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: 'announcements', component: ViewAnnouncementsComponent },
  { path: 'logs', component: ViewLogsComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'dashboard', component: ViewLogsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CommunicationRoutingModule { }
