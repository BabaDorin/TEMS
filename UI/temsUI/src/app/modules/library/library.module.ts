import { MaterialModule } from '../material/material.module';
import { ViewLibraryComponent } from '../../tems-components/library/view-library/view-library.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LibraryRoutingModule } from './library-routing.module';


@NgModule({
  declarations: [
    ViewLibraryComponent,
  ],
  imports: [
    CommonModule,
    MaterialModule,
    LibraryRoutingModule
  ]
})
export class LibraryModule { }
