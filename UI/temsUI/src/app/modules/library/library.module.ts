import { TranslateModule } from '@ngx-translate/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { GenericContainerModule } from './../../shared/generic-container/generic-container.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { FileUploadModule } from './../file-upload/file-upload.module';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DndDirective } from './../../tems-components/library/upload-library-item/dnd.directive';
import { ViewLibraryComponent } from '../../tems-components/library/view-library/view-library.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LibraryRoutingModule } from './library-routing.module';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { AngularFileUploaderModule } from 'angular-file-uploader';
import { FileIconComponent } from 'src/app/tems-components/tems-icons/file-icon/file-icon.component';
import { MatIconModule } from '@angular/material/icon';


@NgModule({
  declarations: [
    ViewLibraryComponent,
    UploadLibraryItemComponent,
    DndDirective,
    FileIconComponent
  ],
  imports: [
    GenericContainerModule,
    CommonModule,
    LibraryRoutingModule,
    AngularFileUploaderModule,
    TemsFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,
    MatIconModule,
    NgbModule,
    FileUploadModule,
    TranslateModule,
    NgxPaginationModule,
  ]
})
export class LibraryModule { }
