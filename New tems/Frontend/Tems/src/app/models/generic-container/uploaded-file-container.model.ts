import { TranslateService } from '@ngx-translate/core';
import { ClaimService } from './../../services/claim.service';
import { TokenService } from '../../services/token.service';
import { ViewLibraryItem } from './../library/view-library-item.model';
import { SnackService } from '../../services/snack.service';
import { LibraryService } from '../../services/library.service';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';
import { TEMSComponent } from './../../tems/tems.component';
import { Downloader } from 'src/app/shared/downloader/fileDownloader';
import { DecimalPipe } from '@angular/common';
import { ConfirmService } from 'src/app/confirm.service';

export class UploadedFileContainerModel extends TEMSComponent implements IGenericContainerModel {
    title: string;
    tagGroups: ITagGroup[];
    actions: IContainerAction[];
    description: string;
    eventEmitted: Function;

    downloader = new Downloader();

    constructor(
        private libraryService: LibraryService,
        private snackService: SnackService,
        private claims: ClaimService,
        private _decimalPipe: DecimalPipe,
        private libraryItem: ViewLibraryItem,
        private confirmService: ConfirmService,
        private translate: TranslateService
    ) {
        super();

        this.buildContainerModel();
    }

    buildContainerModel() {
        this.title = this.libraryItem.displayName;

        this.tagGroups = [];
        this.tagGroups.push({
            name: this.translate.instant('library.downloads'),
            tags: [this.libraryItem.downloads.toString()]
        },
        {
            name: this.translate.instant('library.fileSize'),
            tags: [this._decimalPipe.transform(this.libraryItem.fileSize / 1024 / 1024 / 1024, '1.1-1') + ' GB']
        }
        )

        this.description = this.libraryItem.description;

        this.actions = [];
        this.actions.push({
            value: 'download',
            name: this.translate.instant('library.download'),
            icon: 'download',
            action: () => this.download()
        });

        if (this.claims.canManageAssets) {
            this.actions.push(
                {
                    value: 'delete',
                    name: this.translate.instant('library.remove'),
                    icon: 'delete',
                    action: () => this.remove()
                }
            )
        }
    }


    async remove() {
        if (!await this.confirmService.confirm("Do you realy want to remove that library item?"))
            return;

        this.libraryService.removeItem(this.libraryItem.id)
            .subscribe(result => {
                if (this.snackService.snackIfError(result))
                    return;

                if (result.status == 1)
                    this.eventEmitted('removed');
            })
    }

    download() {
        let downloadButton = this.actions.find(q => q.value == 'download');
            downloadButton.disabled = true;

        this.subscriptions.push(this.libraryService.downloadItem(this.libraryItem.id)
            .subscribe((event) => {
                this.downloader.downloadFile(event, this.libraryItem.actualName);
                downloadButton.disabled = false;
                this.description = this.libraryItem.description;
                this.libraryItem.downloads++;
            }));
    }
}