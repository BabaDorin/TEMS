import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ViewLibraryComponent } from './../../tems-components/library/view-library/view-library.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { AngularFileUploaderModule } from 'angular-file-uploader';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { DndDirective } from './../../tems-components/library/upload-library-item/dnd.directive';
import { FileUploadModule } from './../file-upload/file-upload.module';
import { LibraryRoutingModule } from './library-routing.module';
import { TemsFormsModule } from '../tems-forms/tems-forms.module';
import { ViewLibraryModule } from './view-library/view-library.module';



@NgModule({
  declarations: [
    UploadLibraryItemComponent,
    DndDirective,
  ],
  imports: [
    CommonModule,
    LibraryRoutingModule,
    AngularFileUploaderModule,
    TemsFormsModule,
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
