import { TEMSComponent } from './../../../tems/tems.component';
import { ViewLibraryItem } from './../../../models/library/view-library-item.model';
import { LibraryService } from './../../../services/library-service/library.service';
import { Component, OnInit } from '@angular/core';
import { HttpEventType, HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-view-library',
  templateUrl: './view-library.component.html',
  styleUrls: ['./view-library.component.scss']
})
export class ViewLibraryComponent extends TEMSComponent implements OnInit {

  libraryItems: ViewLibraryItem[];

  constructor(
    private libraryService: LibraryService
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

  downloadItem(itemId: string, fileName: string) {
    this.subscriptions.push(this.libraryService.downloadItem(itemId)
      .subscribe((event) => {
        this.downloadFile(event, fileName);
        // if (event.type === HttpEventType.Response) {
        //   this.downloadFile(event);
        // }
      }));
  }

  private downloadFile(data, fileName: string) {
    const downloadedFile = new Blob([data], { type: data.type.toString() });
    var url = window.URL.createObjectURL(downloadedFile);
    var anchor = document.createElement("a");
    anchor.download = fileName;
    anchor.href = url;
    anchor.click();
  }

  removeItem(itemId: string){
    this.subscriptions.push(this.libraryService.removeItem(itemId)
      .subscribe(result => {
        console.log(result);
      }))
  }
}
