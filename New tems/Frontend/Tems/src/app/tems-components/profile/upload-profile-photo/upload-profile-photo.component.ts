import { SnackService } from './../../../services/snack.service';
import { Component, Output, EventEmitter, OnInit, Inject, Optional } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ImageCroppedEvent, base64ToFile, ImageCropperModule } from 'ngx-image-cropper';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { FileUploadComponent } from '../../file-upload/file-upload.component';

@Component({
  selector: 'app-upload-profile-photo',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatProgressBarModule,
    TranslateModule,
    FileUploadComponent,
    ImageCropperModule
  ],
  templateUrl: './upload-profile-photo.component.html',
  styleUrls: ['./upload-profile-photo.component.scss']
})
export class UploadProfilePhotoComponent implements OnInit {

  /*
  If user uploads an image (.png, .jpg etc.) then he will be able to resize it.
  
  If he uploads a gif, it won't be possible to resize due to the fact that canvas (which is used for cropping)
  can't deal with gifs (And I don't think it is necessary to get over-complicate things just to allow users to
  crop their gifs).

  The cropped image or uploaded gif is being sent to userService via an instance of ProfilePhoto
  */

  @Output() imageSelected = new EventEmitter();

  selectedFileFormat: any = 'jpg';
  imageChangedEvent;
  croppedImage;
  fileSelected: File;
  restrictCropping: boolean = false;
  fileUrl;

  constructor(
    public translate: TranslateService,
    private snack: SnackService,
    @Optional() public dialogRef: MatDialogRef<UploadProfilePhotoComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) {
  }

  ngOnInit(): void {
    // ...existing code...
  }

  fileChangeEvent(event: any): void {
    if (event == undefined)
      return;

    // Setting this directly to fileSelected will trigger to UI to render image-cropper component
    // but we have to figure out the selected file's format first
    let tempFile = event.target.files[0];  
    this.selectedFileFormat = tempFile.name.substr(tempFile.name.lastIndexOf('.') + 1) 
    
    this.imageChangedEvent = event;
    this.fileSelected = tempFile;

    if(this.selectedFileFormat == 'gif'){
      this.restrictCropping = true;
      this.createFileUrl();
      this.croppedImage = this.fileSelected;
      return; 
    }

    this.restrictCropping = false;
  }

  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = base64ToFile(event.base64);
    this.croppedImage.name = this.fileSelected.name;
  }

  createFileUrl(){
    let reader = new FileReader();
    reader.readAsDataURL(this.fileSelected);
    reader.onload = () => {
      this.fileUrl = reader.result; 
    };
  }

  imageLoaded() {
    /* show cropper */
  }

  cropperReady() {
    /* cropper ready */
  }

  loadImageFailed() {
    this.snack.snack({ message: 'An error occured while processing the image', status: 0 });
  }

  cancelCurrent(){
    this.imageSelected.emit(undefined);
  }

  submit() {
    this.imageSelected.emit(this.croppedImage);
  }
}
