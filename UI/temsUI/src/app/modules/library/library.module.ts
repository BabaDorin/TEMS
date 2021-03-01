import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DndDirective } from './../../tems-components/library/upload-library-item/dnd.directive';
import { MaterialModule } from '../material/material.module';
import { ViewLibraryComponent } from '../../tems-components/library/view-library/view-library.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LibraryRoutingModule } from './library-routing.module';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { AngularFileUploaderModule } from 'angular-file-uploader';


@NgModule({
  declarations: [
    ViewLibraryComponent,
    UploadLibraryItemComponent,
    DndDirective,
  ],
  imports: [
    CommonModule,
    MaterialModule,
    LibraryRoutingModule,
    AngularFileUploaderModule,
    NgbModule,
  ]
})
export class LibraryModule { }
