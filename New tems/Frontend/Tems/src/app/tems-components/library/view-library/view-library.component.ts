import { TranslateService } from '@ngx-translate/core';
import { DecimalPipe } from '@angular/common';
import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConfirmService } from 'src/app/confirm.service';
import { DialogService } from 'src/app/services/dialog.service';
import { UploadLibraryItemComponent } from 'src/app/tems-components/library/upload-library-item/upload-library-item.component';
import { LibraryService } from '../../../services/library.service';
import { SnackService } from '../../../services/snack.service';
import { Fraction } from './../../../models/analytics/fraction.model';
import { UploadedFileContainerModel } from './../../../models/generic-container/uploaded-file-container.model';
import { ViewLibraryItem } from './../../../models/library/view-library-item.model';
import { ClaimService } from './../../../services/claim.service';
import { Downloader } from './../../../shared/downloader/fileDownloader';
import { TEMSComponent } from './../../../tems/tems.component';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { GenericContainerComponent } from '../../../shared/generic-container/generic-container.component';

@Component({
  selector: 'app-view-library',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    GenericContainerComponent,
    MatIconModule,
    MatProgressBarModule,
    TranslateModule,
    NgxPaginationModule
  ],
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
    public claims: ClaimService,
    private _decimalPipe: DecimalPipe,
    private confirmService: ConfirmService,
    private translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(dialogData != undefined){
      this.accessPass = dialogData.accessPass;
    }
  }

  ngOnInit(): void {
    this.subscriptions.push(this.libraryService.getItems(this.accessPass)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.libraryContainerModels = (result as ViewLibraryItem[])
          .map(q => new UploadedFileContainerModel(this.libraryService, this.snackService, this.claims, this._decimalPipe, q, this.confirmService, this.translate));
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
        this.ngOnInit();
      }
    )
  }

  fetchSpaceUsageData(){
    this.subscriptions.push(
      this.libraryService.getSpaceUsageData()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.spaceUsageData = result;
      })
    )
  }
}
