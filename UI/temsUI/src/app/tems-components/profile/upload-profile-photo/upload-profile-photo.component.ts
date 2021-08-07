import { SnackService } from './../../../services/snack.service';
import { Component, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ImageCroppedEvent, base64ToFile } from 'ngx-image-cropper';

@Component({
  selector: 'app-upload-profile-photo',
  templateUrl: './upload-profile-photo.component.html',
  styleUrls: ['./upload-profile-photo.component.scss']
})
export class UploadProfilePhotoComponent {

  /*
  If user uploads an image (.png, .jpg etc.) then he will be able to resize it.
  
  If he uploads a gif, it won't be possible to resize due to the fact that canvas (which is used for cropping)
  can't deal with gifs (And I don't think it is necessary to get over-complicate things just to allow users to
  crop their gifs).

  The cropped image or uploaded gif is being sent to userService via an instance of ProfilePhoto
  */

  @Output() imageSelected = new EventEmitter();

  selectedFileFormat: string = 'jpg';
  imageChangedEvent;
  croppedImage;
  fileSelected: File;
  restrictCropping: boolean = false;
  fileUrl;

  dialogRef;

  constructor(
    public translate: TranslateService,
    private snack: SnackService) {
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

  submit() {
    this.imageSelected.emit(this.croppedImage);
  }
}
