import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './public/dashboard/dashboard.component';
import { QuickAccessComponent } from './tems-components/equipment/quick-access/quick-access.component';


const routes: Routes = [
  // Kind of home page
  { path: '', component: DashboardComponent },

  // type = ['equipment', 'room', 'personnel'] 
  { path: 'quick-access/:type', component: QuickAccessComponent },
  
  { path: 'error-pages', loadChildren: () => import('./public/error-pages/error-pages.module').then(m => m.ErrorPagesModule) },
  
  { path: 'equipment', loadChildren: () => import('./modules/equipment/equipment.module').then(m => m.EquipmentModule) },
  { path: 'rooms', loadChildren: () => import('./modules/rooms/rooms.module').then(m => m.RoomsModule) },
  { path: 'personnel', loadChildren: () => import('./modules/personnel/personnel.module').then(m => m.PersonnelModule) },
  { path: 'keys', loadChildren: () => import('./modules/keys/keys.module').then(m => m.KeysModule) },
  { path: 'issues', loadChildren: () => import('./modules/issues/issues.module').then(m => m.IssuesModule) },
  { path: 'analytics', loadChildren: () => import('./modules/analytics/analytics.module').then(m => m.AnalyticsModule) },
  { path: 'communication', loadChildren: () => import('./modules/communication/communication.module').then(m => m.CommunicationModule) },
  { path: 'library', loadChildren: () => import('./modules/library/library.module').then(m => m.LibraryModule) },
  { path: 'reports', loadChildren: () => import('./modules/reports/reports.module').then(m => m.ReportsModule) },
  { path: 'administration', loadChildren: () => import('./modules/administration/administration.module').then(m => m.AdministrationModule) },
  { path: 'auth', loadChildren: () => import('./modules/authentication/authentication.module').then(m => m.AuthenticationModule) },
  { path: 'archieve', loadChildren: () => import('./modules/archieve/archieve.module').then(m => m.ArchieveModule) },
  { path: 'profile', loadChildren: () => import('./modules/profile/profile.module').then(m => m.ProfileModule) },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
