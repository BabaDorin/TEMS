import { TranslateModule } from '@ngx-translate/core';
import { GenericContainerModule } from './../../../shared/generic-container/generic-container.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ViewLibraryComponent } from './../../../tems-components/library/view-library/view-library.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@NgModule({
  declarations: [
    ViewLibraryComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
    NgxPaginationModule,
    GenericContainerModule,
    TranslateModule
  ],
  exports: [
    ViewLibraryComponent
  ]
})
export class ViewLibraryModule { }
