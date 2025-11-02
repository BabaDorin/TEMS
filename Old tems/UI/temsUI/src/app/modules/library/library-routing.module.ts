import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CanViewEntitiesGuard } from 'src/app/guards/can-view-entities.guard';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { ViewLibraryComponent } from '../../tems-components/library/view-library/view-library.component';
import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';

const routes: Routes = [
  { path: '', component: ViewLibraryComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'all', component: ViewLibraryComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'add', component: UploadLibraryItemComponent, canActivate: [CanManageEntitiesGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LibraryRoutingModule { }
