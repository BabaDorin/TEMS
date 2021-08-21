import { Injectable } from '@angular/core';
import { Downloader } from './shared/downloader/fileDownloader';

@Injectable({
  providedIn: 'root'
})
export class DownloadService {

  downloader: Downloader;

  constructor() { }

  downloadFile(blob, fileName: string) {
    if(this.downloader == undefined)
      this.downloader = new Downloader();

    this.downloader.downloadFile(blob, fileName);
  }
}
