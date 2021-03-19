import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { TEMSComponent } from './../../../tems/tems.component';
import { ViewLibraryItem } from './../../../models/library/view-library-item.model';
import { LibraryService } from './../../../services/library-service/library.service';
import { Component, OnInit } from '@angular/core';
import { HttpEventType, HttpResponse } from '@angular/common/http';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-view-library',
  templateUrl: './view-library.component.html',
  styleUrls: ['./view-library.component.scss']
})
export class ViewLibraryComponent extends TEMSComponent implements OnInit {

  libraryItems: ViewLibraryItem[];

  constructor(
    private libraryService: LibraryService,
    public dialog: MatDialog
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
        this.downloadFile(event, item.actualName);
        button.disabled = false;
        button.innerHTML = "Download";
        item.downloads++;
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

  removeItem(item: ViewLibraryItem){
    this.subscriptions.push(this.libraryService.removeItem(item.id)
      .subscribe(result => {
        if(result.status == 1)
          this.libraryItems.splice(this.libraryItems.indexOf(item), 1);
        console.log(result);
      }))
  }

  openUploadItems(){
      let dialogRef: MatDialogRef<any>;
      dialogRef = this.dialog.open(UploadLibraryItemComponent);
  
      dialogRef.afterClosed().subscribe(result => {
        console.log('The dialog was closed');
        this.unsubscribeFromAll();
        this.subscriptions.push(this.libraryService.getItems()
          .subscribe(response => {
            this.libraryItems = response;
          }));
      });
  }
}
