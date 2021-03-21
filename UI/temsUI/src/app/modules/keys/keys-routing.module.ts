import { CanViewEntitiesGuard } from './../../guards/can-view-entities.guard';
import { CanAllocateKeysGuard } from './../../guards/can-allocate-keys.guard';
import { AgGridModule } from 'ag-grid-angular';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { ViewKeysAllocationsComponent } from './../../tems-components/keys/view-keys-allocations/view-keys-allocations.component';
import { ViewKeysComponent } from './../../tems-components/keys/view-keys/view-keys.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { KeysAllocationsComponent } from 'src/app/tems-components/keys/keys-allocations/keys-allocations.component';

const routes: Routes = [
  { path: '', component: ViewKeysComponent, canActivate: [CanViewEntitiesGuard]},
  { path: 'all', component: ViewKeysComponent, canActivate: [CanViewEntitiesGuard]},
  { path: 'allocate', component: KeysAllocationsComponent, canActivate: [CanAllocateKeysGuard]},
  { path: 'allocations', component: ViewKeysAllocationsComponent, canActivate: [CanViewEntitiesGuard]},
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule,
    TemsFormsModule,
    AgGridModule,
  ]
})
export class KeysRoutingModule { }
