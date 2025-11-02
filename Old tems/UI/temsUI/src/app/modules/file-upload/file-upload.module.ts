import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { FileUploadComponent } from './../../tems-components/file-upload/file-upload.component';

@NgModule({
  declarations: [
    FileUploadComponent
  ],
  imports: [
    CommonModule,
    TranslateModule,
    MatIconModule
  ],
  exports: [
    FileUploadComponent,
  ]
})
export class FileUploadModule { }
