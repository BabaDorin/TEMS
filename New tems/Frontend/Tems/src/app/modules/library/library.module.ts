import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ViewLibraryComponent } from './../../tems-components/library/view-library/view-library.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
// TODO: angular-file-uploader is not compatible with Angular 20
// import { AngularFileUploaderModule } from 'angular-file-uploader';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { DndDirective } from './../../tems-components/library/upload-library-item/dnd.directive';
import { FileUploadModule } from './../file-upload/file-upload.module';
import { LibraryRoutingModule } from './library-routing.module';
import { TEMS_FORMS_IMPORTS } from '../tems-forms/tems-forms.module';
import { ViewLibraryModule } from './view-library/view-library.module';



@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    LibraryRoutingModule,
    UploadLibraryItemComponent,
    DndDirective,
    // AngularFileUploaderModule,  // TODO: Not compatible with Angular 20
    ...TEMS_FORMS_IMPORTS,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    FileUploadModule,
    TranslateModule,
    MatButtonModule,
    ViewLibraryModule,
    TranslateModule,
    MatProgressBarModule
  ],
  exports: [
    UploadLibraryItemComponent,
    ViewLibraryComponent,
  ]
})
export class LibraryModule { }
