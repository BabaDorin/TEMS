import { TokenService } from './../../../services/token-service/token.service';
import { SnackService } from './../../../services/snack/snack.service';
import { UploadedFileContainerModel } from './../../../models/generic-container/uploaded-file-container.model';
import { Downloader } from './../../../shared/downloader/fileDownloader';
import { DialogService } from 'src/app/services/dialog-service/dialog.service';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { TEMSComponent } from './../../../tems/tems.component';
import { ViewLibraryItem } from './../../../models/library/view-library-item.model';
import { LibraryService } from './../../../services/library-service/library.service';
import { Component, OnInit } from '@angular/core';
import { CAN_MANAGE_ENTITIES } from 'src/app/models/claims';

@Component({
  selector: 'app-view-library',
  templateUrl: './view-library.component.html',
  styleUrls: ['./view-library.component.scss']
})
export class ViewLibraryComponent extends TEMSComponent implements OnInit {

  libraryItems: ViewLibraryItem[];
  libraryContainerModels: UploadedFileContainerModel[];
  pageNumber = 1;
  downloader = new Downloader();
  canManage: boolean = false;

  constructor(
    private libraryService: LibraryService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private tokenService: TokenService
  ) {
    super();
  }

  ngOnInit(): void {
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);

    this.subscriptions.push(this.libraryService.getItems()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.libraryContainerModels = (result as ViewLibraryItem[])
          .map(q => new UploadedFileContainerModel(this.libraryService, this.snackService, this.tokenService, q));
      }));
  }

  eventEmitted($event, i){
    if($event == 'removed'){
      this.libraryContainerModels.splice(i, 1);
    }
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
