import { MatProgressBarModule } from '@angular/material/progress-bar';
import { GenericContainerModule } from './../../../shared/generic-container/generic-container.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { ViewLibraryComponent } from './../../../tems-components/library/view-library/view-library.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';



@NgModule({
  declarations: [
    ViewLibraryComponent
  ],
  imports: [
    CommonModule,
    NgxPaginationModule,
    MatButtonModule,
    GenericContainerModule,
    MatProgressBarModule
  ],
  exports: [
    ViewLibraryModule
  ]
})
export class ViewLibraryModule { }
