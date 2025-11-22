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

  b64toBlob = (b64Data, contentType='', sliceSize=512) => {
    const byteCharacters = atob(b64Data);
    const byteArrays = [];
  
    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      const slice = byteCharacters.slice(offset, offset + sliceSize);
  
      const byteNumbers = new Array(slice.length);
      for (let i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }
  
      const byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }
  
    const blob = new Blob(byteArrays, {type: contentType});
    return blob;
  }


  downloadFromB64(b64: string, contentType: string, fileName: string){
    let blob = this.b64toBlob(b64, contentType);
    this.downloadFile(blob, fileName);
  }

  base64SvgToBase64Png (originalBase64, width) {
    return new Promise(resolve => {
        let img = document.createElement('img');
        img.onload = function () {
            document.body.appendChild(img);
            let canvas = document.createElement("canvas");
            let ratio = (img.clientWidth / img.clientHeight) || 1;
            document.body.removeChild(img);
            canvas.width = width;
            canvas.height = width / ratio;
            let ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
            try {
                let data = canvas.toDataURL('image/png');
                resolve(data);
            } catch (e) {
                resolve(null);
            }
        };
        img.src = originalBase64;
    });
  }
}
