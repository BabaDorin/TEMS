import { ViewLibraryModule } from './view-library/view-library.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { AngularFileUploaderModule } from 'angular-file-uploader';
import { NgxPaginationModule } from 'ngx-pagination';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { FileIconComponent } from 'src/app/tems-components/tems-icons/file-icon/file-icon.component';
import { ViewLibraryComponent } from '../../tems-components/library/view-library/view-library.component';
import { GenericContainerModule } from './../../shared/generic-container/generic-container.module';
import { DndDirective } from './../../tems-components/library/upload-library-item/dnd.directive';
import { FileUploadModule } from './../file-upload/file-upload.module';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { LibraryRoutingModule } from './library-routing.module';



@NgModule({
  declarations: [
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
    MatButtonModule,
    NgxPaginationModule,
    ViewLibraryModule,
  ]
})
export class LibraryModule { }
