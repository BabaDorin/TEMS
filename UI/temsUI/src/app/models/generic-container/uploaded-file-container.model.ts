import { TokenService } from './../../services/token-service/token.service';
import { ViewLibraryItem } from './../library/view-library-item.model';
import { SnackService } from './../../services/snack/snack.service';
import { LibraryService } from './../../services/library-service/library.service';
import { IContainerAction, IGenericContainerModel, ITagGroup } from './IGenericContainer.model';
import { TEMSComponent } from './../../tems/tems.component';
import { Downloader } from 'src/app/shared/downloader/fileDownloader';
import { CAN_MANAGE_ENTITIES } from '../claims';

export class UploadedFileContainerModel extends TEMSComponent implements IGenericContainerModel {
    title: string;
    tagGroups: ITagGroup[];
    actions: IContainerAction[];
    description: string;
    eventEmitted: Function;

    downloader = new Downloader();
    canManage: boolean = false;

    constructor(
        private libraryService: LibraryService,
        private snackService: SnackService,
        private tokenService: TokenService,
        private libraryItem: ViewLibraryItem
    ) {
        super();

        this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);
        this.buildContainerModel();
    }

    buildContainerModel() {
        this.title = this.libraryItem.displayName;

        this.tagGroups = [];
        this.tagGroups.push({
            name: 'Downloads',
            tags: [this.libraryItem.downloads.toString()]
        },
            {
                name: 'DateUploaded',
                tags: [new Date(this.libraryItem.dateUploaded).toDateString()]
            },
            {
                name: 'File size',
                tags: [this.libraryItem.fileSize.toString()]
            }
        )

        this.description = this.libraryItem.description;

        this.actions = [];
        this.actions.push({
            name: 'Download',
            icon: 'download',
            action: () => this.download()
        });

        if(this.canManage){
            this.actions.push(
                {
                    name: 'Remove',
                    icon: 'delete',
                    action: () => this.remove()
                }
            )
        }
    }


    remove() {
        if (!confirm("Do you realy want to remove that library item?"))
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
        this.description = "Preparing... Please wait";
        let downloadButton = this.actions.find(q => q.name == 'Download'); 
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