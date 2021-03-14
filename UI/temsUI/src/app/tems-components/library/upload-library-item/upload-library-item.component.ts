import { LibraryService } from './../../../services/library-service/library.service';
import { API_LBR_URL } from './../../../models/backend.config';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpEventType, HttpRequest } from '@angular/common/http';
import { AddLibraryItem } from 'src/app/models/library/add-library-item.model';

@Component({
  selector: 'app-upload-library-item',
  templateUrl: './upload-library-item.component.html',
  styleUrls: ['./upload-library-item.component.scss']
})
export class UploadLibraryItemComponent extends TEMSComponent implements OnInit {

  files: any[] = [];

  constructor(
    private http: HttpClient,
    private libraryService: LibraryService,
  ) {
    super();
  }

  ngOnInit(): void {
  }

  private formDatas: FormData[] = [];

  upload() {
    if (this.files.length === 0)
      return;

    for (let index = 0; index < this.files.length; index++) {

      let fileToUpload = <File>this.files[index];

      const formData = new FormData();
      formData.append('file', fileToUpload, fileToUpload.name);
      this.formDatas.push(formData);

      this.http.post(API_LBR_URL + '/uploadfile', this.formDatas[index], { reportProgress: true, observe: 'events' })
        .subscribe(event => {
          console.log(event);

          if (event.type === HttpEventType.UploadProgress)
            this.files[index].progress = Math.round(100 * event.loaded / event.total);
          else if (event.type === HttpEventType.Response) {
            this.files[index].message = 'Upload success.';
            // this.files[0].onUploadFinished.emit(event.body);
          }
        });
    }

  }

  //  Convert Files list to normal array list
  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      item.progress = 0;
      item.myName = '';
      item.myDescription = '';
      item.message = '';
      this.files.push(item);
    }
  }

  onFileDropped($event) {
    this.prepareFilesList($event);
  }

  fileBrowseHandler(files) {
    this.prepareFilesList(files);
  }

  deleteFile(index: number) {
    this.libraryService.cancelThread(0).subscribe(result => console.log(result));
    // this.formDatas.splice(index, 1);
    // this.files.splice(index, 1);
    this.formDatas[index] = null;
    this.files[index] = { message: "canceled" };
  }

  formatBytes(bytes, decimals) {
    if (bytes === 0) {
      return '0 Bytes';
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals || 2;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }
}
