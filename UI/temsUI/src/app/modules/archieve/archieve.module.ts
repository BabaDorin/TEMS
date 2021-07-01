import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { AgGridModule } from 'ag-grid-angular';
import { NgxPaginationModule } from 'ngx-pagination';
import { AgGridArchievedItemsComponent } from './../../tems-components/archieve/ag-grid-archieved-items/ag-grid-archieved-items.component';
import { ViewArchieveComponent } from './../../tems-components/archieve/view-archieve/view-archieve.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
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
