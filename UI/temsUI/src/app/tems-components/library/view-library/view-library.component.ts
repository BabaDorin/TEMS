import { Downloader } from './../../../shared/downloader/fileDownloader';
import { DialogService } from 'src/app/services/dialog-service/dialog.service';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { TEMSComponent } from './../../../tems/tems.component';
import { ViewLibraryItem } from './../../../models/library/view-library-item.model';
import { LibraryService } from './../../../services/library-service/library.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-library',
  templateUrl: './view-library.component.html',
  styleUrls: ['./view-library.component.scss']
})
export class ViewLibraryComponent extends TEMSComponent implements OnInit {

  libraryItems: ViewLibraryItem[];
  pageNumber = 1;
  downloader = new Downloader();

  constructor(
    private libraryService: LibraryService,
    private dialogService: DialogService
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.libraryService.getItems()
      .subscribe(result => {
        console.log(result);
        this.libraryItems = result;
      }));
  }

  downloadItem(eventData, item) {
    let button = eventData.target;
    button.innerHTML = "Preparing... Please wait";
    button.disabled = true;

    this.subscriptions.push(this.libraryService.downloadItem(item.id)
      .subscribe((event) => {
        this.downloader.downloadFile(event, item.actualName);
        button.disabled = false;
        button.innerHTML = "Download";
        item.downloads++;
      }));
  }

  removeItem(item: ViewLibraryItem){
    this.subscriptions.push(this.libraryService.removeItem(item.id)
      .subscribe(result => {
        if(result.status == 1)
          this.libraryItems.splice(this.libraryItems.indexOf(item), 1);
        console.log(result);
      }))
  }

  openUploadItems(){
    this.dialogService.openDialog(
      UploadLibraryItemComponent,
      undefined,
      () => {
        this.unsubscribeFromAll();
        this.subscriptions.push(this.libraryService.getItems()
          .subscribe(response => {
            this.libraryItems = response;
          }));
      }
    )
  }
}
