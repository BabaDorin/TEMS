import { ResponseFactory } from './../../../models/system/response.model';
import { HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { LibraryService } from '../../../services/library.service';
import { SnackService } from '../../../services/snack.service';

@Component({
  selector: 'app-upload-library-item',
  templateUrl: './upload-library-item.component.html',
  styleUrls: ['./upload-library-item.component.scss']
})
export class UploadLibraryItemComponent extends TEMSComponent implements OnInit {

  files: any[] = [];
  formDatas: FormData[] = [];
  uploadEnabled = true;

  constructor(
    public libraryService: LibraryService,
    private snackService: SnackService,
  ) {
    super();
  }

  ngOnInit(): void {
  }

  upload() {
    if (this.files.length === 0)
      return;

    if(this.files.findIndex(q => q != null && q != undefined && q.size > 2147482548) > -1){
      this.snackService.snack(ResponseFactory.Neutral("One of selected files exceeds the maximum file size allowed of 2 GB."));
      return;
    }

    this.uploadEnabled = false;
    this.uploadFile(0);
  }

  uploadFile(index) {
    // If current file has been deleted or already uploaded, we go to the next one
    if(this.files[index] == null || this.files[index].status == 'uploaded')
      if(++index < this.files.length)
        return this.uploadFile(index);
      else
        return;

    let fileToUpload = this.files[index];
    fileToUpload.status = "uploading";

    // A formdata object is created. It will contain data about the 
    // currently uploading file.
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    formData.append('myName', fileToUpload.myName)
    formData.append('myDescription', fileToUpload.myDescription)
    this.formDatas[index]=formData;
    
    let timer = null;

    new Promise<void>((resolve)=> {
      this.subscriptions.push(
        this.libraryService.getAvailableLibraryStorageSpace()
        .subscribe(result => {
          if(result - this.files[index].size > 0 )
            resolve();
          else
            this.snackService.snack({message: "Free up some space", status: 0});
        })
      )
    }).then(() => {
      this.subscriptions.push(
        this.libraryService.uploadFile(this.formDatas[index])
        .subscribe(event => {
          if (event.type === HttpEventType.UploadProgress){
            
            if(fileToUpload)
              fileToUpload.progress = Math.round(100 * event.loaded / event.total);
            
            // If we haven't got any response for 1 second or so, it might mean
            // that the file has been uploaded and now it is being compressed on the server
            if(timer) clearTimeout(timer);
            timer = setTimeout(() => {
              fileToUpload.message = "Compressing... Please wait, It might take a while."
              fileToUpload.status = "compressing"
            }, 1000);
          }
          else if (event.type === HttpEventType.Response) {
            // The file has been uploaded
            clearTimeout(timer);
            fileToUpload.message = 'Success';
            fileToUpload.status = "uploaded";
  
            // Go to the next file, if it exists
            if(++index < this.files.length)
              return this.uploadFile(index);
          }
        }));
    })
  }

  //  Convert Files list to normal array list
  prepareFilesList(files: Array<any>) {
    this.files = [];
    this.formDatas = [];
    for (const item of files) {
      item.progress = 0;
      item.myName = '';
      item.myDescription = '';
      item.message = '';
      this.files.push(item);
      this.formDatas.push(new FormData());
    }
    this.uploadEnabled = true;
  }

  filesSelected(files) {
    this.prepareFilesList(files);
  }

  deleteFile(index: number) {
    this.formDatas[index] = null;
    this.files[index] = null;
  }
}
