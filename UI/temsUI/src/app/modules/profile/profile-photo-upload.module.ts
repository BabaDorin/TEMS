import { MatButtonModule } from '@angular/material/button';
import { UploadProfilePhotoComponent } from './../../tems-components/profile/upload-profile-photo/upload-profile-photo.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImageCropperModule } from 'ngx-image-cropper';



@NgModule({
  declarations: [
    UploadProfilePhotoComponent
  ],
  imports: [
    CommonModule,
    ImageCropperModule,
    MatButtonModule
  ],
  exports: [
    UploadProfilePhotoComponent
  ]
})
export class ProfilePhotoUploadModule { }
