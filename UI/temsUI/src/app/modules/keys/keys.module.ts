import { KeysService } from 'src/app/services/keys-service/keys.service';
import { ViewKeysAllocationsComponent } from './../../tems-components/keys/view-keys-allocations/view-keys-allocations.component';
import { KeysAllocationsComponent } from './../../tems-components/keys/keys-allocations/keys-allocations.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { KeysRoutingModule } from './keys-routing.module';
import { ViewKeysComponent } from 'src/app/tems-components/keys/view-keys/view-keys.component';
import { AgGridKeysComponent } from 'src/app/tems-components/keys/ag-grid-keys/ag-grid-keys.component';


@NgModule({
  declarations: [
    KeysAllocationsComponent,
    ViewKeysAllocationsComponent,
    ViewKeysComponent,
    AgGridKeysComponent,
  ],
  imports: [
    CommonModule,
    KeysRoutingModule,
  ],
  providers: [
      KeysService,
  ]
})
export class KeysModule { }
