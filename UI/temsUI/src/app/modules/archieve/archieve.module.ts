import { TranslateModule } from '@ngx-translate/core';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AgGridAngular, AgGridModule } from 'ag-grid-angular';
import { AgGridArchievedItemsComponent } from './../../tems-components/archieve/ag-grid-archieved-items/ag-grid-archieved-items.component';
import { ViewArchieveComponent } from './../../tems-components/archieve/view-archieve/view-archieve.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ArchieveRoutingModule } from './archieve-routing.module';
import { MatOptionModule } from '@angular/material/core';


@NgModule({
  declarations: [
    ViewArchieveComponent,
    AgGridArchievedItemsComponent
  ],
  imports: [
    AgGridModule,
    CommonModule,
    ArchieveRoutingModule,
    TemsFormsModule,
    MatProgressBarModule,
    MatIconModule,
    MatFormFieldModule,
    TranslateModule,
    MatInputModule,
    MatOptionModule,
    NgxPaginationModule,
  ]
})
export class ArchieveModule { }
