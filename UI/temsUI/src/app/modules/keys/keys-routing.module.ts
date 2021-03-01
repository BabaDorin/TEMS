import { AgGridModule } from 'ag-grid-angular';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { ViewKeysAllocationsComponent } from './../../tems-components/keys/view-keys-allocations/view-keys-allocations.component';
import { ViewKeysComponent } from './../../tems-components/keys/view-keys/view-keys.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { KeysAllocationsComponent } from 'src/app/tems-components/keys/keys-allocations/keys-allocations.component';

const routes: Routes = [
  { path: '', component: ViewKeysComponent, },
  { path: 'all', component: ViewKeysComponent, },
  { path: 'allocate', component: KeysAllocationsComponent, },
  { path: 'allocations', component: ViewKeysAllocationsComponent, },
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
