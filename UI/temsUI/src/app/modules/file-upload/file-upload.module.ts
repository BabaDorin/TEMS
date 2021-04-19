import { MatIconModule } from '@angular/material/icon';
import { FileUploadComponent } from './../../tems-components/file-upload/file-upload.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    FileUploadComponent
  ],
  imports: [
    CommonModule,
    MatIconModule
  ],
  exports: [
    FileUploadComponent
  ]
})
export class FileUploadModule { }
