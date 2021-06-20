import { Fraction } from './../../../models/analytics/fraction.model';
import { ClaimService } from './../../../services/claim.service';
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
import { DecimalPipe } from '@angular/common';

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
  accessPass;
  spaceUsageData: Fraction;

  constructor(
    private libraryService: LibraryService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private claims: ClaimService,
    private _decimalPipe: DecimalPipe
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.libraryService.getItems()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.libraryContainerModels = (result as ViewLibraryItem[])
          .map(q => new UploadedFileContainerModel(this.libraryService, this.snackService, this.claims, this._decimalPipe, q));
      }));

    this.fetchSpaceUsageData();
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

  fetchSpaceUsageData(){
    this.subscriptions.push(
      this.libraryService.getSpaceUsageData(this.accessPass)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.spaceUsageData = result;
      })
    )
  }
}
