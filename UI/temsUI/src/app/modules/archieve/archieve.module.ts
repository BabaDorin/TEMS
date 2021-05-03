import { AgGridAngular, AgGridModule } from 'ag-grid-angular';
import { AgGridArchievedItemsComponent } from './../../tems-components/archieve/ag-grid-archieved-items/ag-grid-archieved-items.component';
import { ViewArchieveComponent } from './../../tems-components/archieve/view-archieve/view-archieve.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ArchieveRoutingModule } from './archieve-routing.module';


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
    NgxPaginationModule,
  ]
})
export class ArchieveModule { }
