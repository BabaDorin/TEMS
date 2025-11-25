import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AgGridModule } from 'ag-grid-angular';
// import { KeysAllocationsComponent } from 'src/app/tems-components/keys/keys-allocations/keys-allocations.component';
import { CanAllocateKeysGuard } from './../../guards/can-allocate-keys.guard';
import { CanViewKeysGuard } from './../../guards/can-view-keys.guard';
// import { ViewKeysAllocationsComponent } from './../../tems-components/keys/view-keys-allocations/view-keys-allocations.component';
// import { ViewKeysComponent } from './../../tems-components/keys/view-keys/view-keys.component';
import { TEMS_FORMS_IMPORTS } from './../tems-forms/tems-forms.module';

const routes: Routes = [
  // { path: '', component: ViewKeysComponent, canActivate: [CanViewKeysGuard]},
  // { path: 'all', component: ViewKeysComponent, canActivate: [CanViewKeysGuard]},
  // { path: 'allocate', component: KeysAllocationsComponent, canActivate: [CanAllocateKeysGuard]},
  // { path: 'allocations', component: ViewKeysAllocationsComponent, canActivate: [CanViewKeysGuard]},
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule,
    ...TEMS_FORMS_IMPORTS,
    AgGridModule,
  ]
})
export class KeysRoutingModule { }
