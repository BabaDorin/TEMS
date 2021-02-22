import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './public/dashboard/dashboard.component';


const routes: Routes = [
  // Kind of home page
  { path: '', component: DashboardComponent },

  { path: 'equipment', loadChildren: () => import('./tems-components/equipment/equipment.module').then(m => m.EquipmentModule) },
  { path: 'rooms', loadChildren: () => import('./rooms/rooms.module').then(m => m.RoomsModule) },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
