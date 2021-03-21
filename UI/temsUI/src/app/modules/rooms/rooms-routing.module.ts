import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';
import { ViewRoomsComponent } from '../../tems-components/room/view-rooms/view-rooms.component';
import { CollegeMapComponent } from '../../tems-components/college-map/college-map.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomDetailsComponent } from '../../tems-components/room/room-details/room-details.component';
import { AddRoomComponent } from '../../tems-components/room/add-room/add-room.component';
import { CanViewEntitiesGuard } from 'src/app/guards/can-view-entities.guard';

const routes: Routes = [
  { path: '', component: ViewRoomsComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'view', component: ViewRoomsComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'add', component: AddRoomComponent, canActivate: [CanManageEntitiesGuard] },
  { path: 'map', component: CollegeMapComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'details/:id', component: RoomDetailsComponent, canActivate: [CanViewEntitiesGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoomsRoutingModule { }
