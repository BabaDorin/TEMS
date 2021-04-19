import { FileUploadModule } from './../file-upload/file-upload.module';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DndDirective } from './../../tems-components/library/upload-library-item/dnd.directive';
import { MaterialModule } from '../material/material.module';
import { ViewLibraryComponent } from '../../tems-components/library/view-library/view-library.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LibraryRoutingModule } from './library-routing.module';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { AngularFileUploaderModule } from 'angular-file-uploader';
import { FileIconComponent } from 'src/app/tems-components/tems-icons/file-icon/file-icon.component';


@NgModule({
  declarations: [
    ViewLibraryComponent,
    UploadLibraryItemComponent,
    DndDirective,
    FileIconComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    LibraryRoutingModule,
    AngularFileUploaderModule,
    TemsFormsModule,
    NgbModule,
    FileUploadModule,
  ]
})
export class LibraryModule { }
