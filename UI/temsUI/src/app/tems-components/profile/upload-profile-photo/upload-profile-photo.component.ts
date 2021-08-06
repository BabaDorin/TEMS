import { SnackService } from './../../../services/snack.service';
import { Component, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ImageCroppedEvent, base64ToFile } from 'ngx-image-cropper';
import { resizeImage } from 'src/app/helpers/image-resizer.helper';

@Component({
  selector: 'app-upload-profile-photo',
  templateUrl: './upload-profile-photo.component.html',
  styleUrls: ['./upload-profile-photo.component.scss']
})
export class UploadProfilePhotoComponent {

  @Output() imageSelected = new EventEmitter();

  imageChangedEvent;
  croppedImage;
  fileSelected;

  constructor(
    public translate: TranslateService,
    private snack: SnackService) {
  }

  fileChangeEvent(event: any): void {
    if (event == undefined)
      return;

    this.imageChangedEvent = event;
    this.fileSelected = event.target.files[0];
  }

  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = base64ToFile(event.base64);
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
    resizeImage(this.croppedImage, 400, 400)
    .then(result => {
      this.imageSelected.emit(result);
    })
    .catch(() => {
      this.snack.snack({ message: 'An error occured while processing the image', status: 0 });
    });
  }
}
