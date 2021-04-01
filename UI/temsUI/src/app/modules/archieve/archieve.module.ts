import { NgxPaginationModule } from 'ngx-pagination';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ArchieveRoutingModule } from './archieve-routing.module';
import { ArchieveComponent } from 'src/app/tems-components/archieve/archieve.component';


@NgModule({
  declarations: [
    ArchieveComponent
  ],
  imports: [
    CommonModule,
    ArchieveRoutingModule,
    TemsFormsModule,
    NgxPaginationModule,
  ]
})
export class ArchieveModule { }
