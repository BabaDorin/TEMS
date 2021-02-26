import { ViewLibraryComponent } from '../../tems-components/library/view-library/view-library.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', component: ViewLibraryComponent },
  { path: 'all', component: ViewLibraryComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LibraryRoutingModule { }
