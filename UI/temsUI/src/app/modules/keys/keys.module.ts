import { TranslateModule } from '@ngx-translate/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgxPaginationModule } from 'ngx-pagination';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { KeysService } from 'src/app/services/keys.service';
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
    TemsAgGridModule,
    LoadingplaceholderModule,
    MatProgressBarModule,
    MatCardModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    TranslateModule,
    MatTabsModule,
    MatFormFieldModule,
    NgxPaginationModule,
  ],
  providers: [
      KeysService,
  ]
})
export class KeysModule { }
