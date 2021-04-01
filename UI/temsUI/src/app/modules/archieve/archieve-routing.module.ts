import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ArchieveComponent } from 'src/app/tems-components/archieve/archieve.component';

const routes: Routes = [
  { path: '', component: ArchieveComponent },
  { path: '/:id', component: ArchieveComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ArchieveRoutingModule { }
