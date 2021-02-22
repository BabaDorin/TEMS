import { ViewRoomsComponent } from './../tems-components/room/view-rooms/view-rooms.component';
import { CollegeMapComponent } from './../tems-components/college-map/college-map.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomDetailsComponent } from '../tems-components/room/room-details/room-details.component';

const routes: Routes = [
  { path: '', component: ViewRoomsComponent },
  { path: 'view', component: ViewRoomsComponent },
  { path: 'map', component: CollegeMapComponent },
  { path: 'details/:id', component: RoomDetailsComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoomsRoutingModule { }
