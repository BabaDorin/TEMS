import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { ViewKeysAllocationsComponent } from './../../tems-components/keys/view-keys-allocations/view-keys-allocations.component';
import { KeysAllocationsComponent } from './../../tems-components/keys/keys-allocations/keys-allocations.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { KeysRoutingModule } from './keys-routing.module';
import { ViewKeysComponent } from 'src/app/tems-components/keys/view-keys/view-keys.component';
import { AgGridKeysComponent } from 'src/app/tems-components/keys/ag-grid-keys/ag-grid-keys.component';
import { KeysAllocationsListComponent } from 'src/app/tems-components/keys/keys-allocations-list/keys-allocations-list.component';
import { AddKeyComponent } from 'src/app/tems-components/keys/add-key/add-key.component';


@NgModule({
  declarations: [
    KeysAllocationsComponent,
    ViewKeysAllocationsComponent,
    ViewKeysComponent,
    AgGridKeysComponent,
    KeysAllocationsListComponent,
    AddKeyComponent
  ],
  imports: [
    CommonModule,
    KeysRoutingModule,
    TemsFormsModule,
  ],
  providers: [
      KeysService,
  ]
})
export class KeysModule { }
