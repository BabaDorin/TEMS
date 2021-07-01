import { ViewLibraryComponent } from './../../tems-components/library/view-library/view-library.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { AngularFileUploaderModule } from 'angular-file-uploader';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { FileIconComponent } from 'src/app/tems-components/tems-icons/file-icon/file-icon.component';
import { DndDirective } from './../../tems-components/library/upload-library-item/dnd.directive';
import { FileUploadModule } from './../file-upload/file-upload.module';
import { LibraryRoutingModule } from './library-routing.module';
import { ViewLibraryModule } from './view-library/view-library.module';
import { TemsFormsModule } from '../tems-forms/tems-forms.module';



@NgModule({
  declarations: [
    UploadLibraryItemComponent,
    DndDirective,
    FileIconComponent
  ],
  imports: [
    // GenericContainerModule,
    CommonModule,
    LibraryRoutingModule,
    AngularFileUploaderModule,
    TemsFormsModule,
    MatFormFieldModule,
    MatInputModule,
    // MatProgressBarModule,
    MatIconModule,
    NgbModule,
    FileUploadModule,
    TranslateModule,
    MatButtonModule,
    // NgxPaginationModule,
    ViewLibraryModule,
  ],
  exports: [
    UploadLibraryItemComponent,
    ViewLibraryComponent,
  ]
})
export class LibraryModule { }
