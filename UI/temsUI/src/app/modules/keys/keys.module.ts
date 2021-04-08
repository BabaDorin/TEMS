import { NgxPaginationModule } from 'ngx-pagination';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { ViewKeysAllocationsComponent } from './../../tems-components/keys/view-keys-allocations/view-keys-allocations.component';
import { KeysAllocationsComponent } from './../../tems-components/keys/keys-allocations/keys-allocations.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { KeysRoutingModule } from './keys-routing.module';
import { ViewKeysComponent } from 'src/app/tems-components/keys/view-keys/view-keys.component';
import { KeysAllocationsListComponent } from 'src/app/tems-components/keys/keys-allocations-list/keys-allocations-list.component';
import { AddKeyComponent } from 'src/app/tems-components/keys/add-key/add-key.component';
import { TemsAgGridModule } from '../tems-ag-grid/tems-ag-grid.module';
import { LoadingplaceholderModule } from '../loadingplaceholder/loadingplaceholder.module';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';


@NgModule({
  declarations: [
    KeysAllocationsComponent,
    ViewKeysAllocationsComponent,
    ViewKeysComponent,
    KeysAllocationsListComponent,
    AddKeyComponent
  ],
  imports: [
    CommonModule,
    KeysRoutingModule,
    TemsFormsModule,
    MaterialModule,
    TemsAgGridModule,
    LoadingplaceholderModule,
    NgbPaginationModule,
    NgxPaginationModule,
  ],
  providers: [
      KeysService,
  ]
})
export class KeysModule { }
