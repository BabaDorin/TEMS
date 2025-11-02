import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { KeysService } from 'src/app/services/keys.service';
import { AddKeyComponent } from 'src/app/tems-components/keys/add-key/add-key.component';
import { KeysAllocationsListComponent } from 'src/app/tems-components/keys/keys-allocations-list/keys-allocations-list.component';
import { ViewKeysComponent } from 'src/app/tems-components/keys/view-keys/view-keys.component';
import { LoadingplaceholderModule } from '../loadingplaceholder/loadingplaceholder.module';
import { TemsAgGridModule } from '../tems-ag-grid/tems-ag-grid.module';
import { KeysAllocationsComponent } from './../../tems-components/keys/keys-allocations/keys-allocations.component';
import { ViewKeysAllocationsComponent } from './../../tems-components/keys/view-keys-allocations/view-keys-allocations.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { KeysRoutingModule } from './keys-routing.module';



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
    MatButtonModule,
    MatTabsModule,
    MatFormFieldModule,
    NgxPaginationModule,
  ],
  providers: [
      KeysService,
  ]
})
export class KeysModule { }
