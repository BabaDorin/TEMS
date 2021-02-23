import { Error404Component } from './public/error-pages/error404/error404.component';
import { QuickAccessComponent } from './tems-components/equipment/quick-access/quick-access.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './public/dashboard/dashboard.component';


const routes: Routes = [
  // Kind of home page
  { path: '', component: DashboardComponent },

  // type = ['equipment', 'room', 'personnel'] 
  { path: 'quick-access/:type', component: QuickAccessComponent },
  
  { path: 'error-pages', loadChildren: () => import('./public/error-pages/error-pages.module').then(m => m.ErrorPagesModule) },
  
  { path: 'equipment', loadChildren: () => import('./tems-components/equipment/equipment.module').then(m => m.EquipmentModule) },
  { path: 'rooms', loadChildren: () => import('./rooms/rooms.module').then(m => m.RoomsModule) },


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
