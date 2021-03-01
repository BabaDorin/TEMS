import { ViewLibraryComponent } from '../../tems-components/library/view-library/view-library.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';

const routes: Routes = [
  { path: '', component: ViewLibraryComponent },
  { path: 'all', component: ViewLibraryComponent },
  { path: 'add', component: UploadLibraryItemComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LibraryRoutingModule { }
