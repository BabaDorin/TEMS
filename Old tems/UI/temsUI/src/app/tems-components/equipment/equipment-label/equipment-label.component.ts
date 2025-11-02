import { QRCodeComponent } from 'angularx-qrcode';
import { LazyLoaderService } from 'src/app/services/lazy-loader.service';
import { environment } from 'src/environments/environment';
import { propertyChanged } from 'src/app/helpers/onchanges-helper';
import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { DownloadService } from 'src/app/download.service';

@Component({
  selector: 'app-equipment-label',
  templateUrl: './equipment-label.component.html',
  styleUrls: ['./equipment-label.component.scss']
})
export class EquipmentLabelComponent implements OnInit, OnChanges{

  @Input() mainText;
  @Input() downloadable = true;
  qrCodeModuleImported = false;
  qrData: string;
  @ViewChild('qr') qr: QRCodeComponent;

  link = "data:image/png;base64,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";
  constructor(
    private downloadService: DownloadService,
    private lazyLoader: LazyLoaderService) 
  { 

  }
 
  ngOnChanges(changes: SimpleChanges): void {
    if(propertyChanged(changes, 'mainText')){
      this.setQr();
    }
  }

  ngOnInit(): void {
    this.setQr();
  }

  async setQr(){
    this.qrData = environment.clientUrl + "/find/" + this.mainText;
    setTimeout(() => {
      this.link = this.qr.qrcElement.nativeElement.childNodes[0].toDataURL(); 
    }, 100);
  }


  downloadLabel() {
    let svgContainer = document.querySelector('#label');
    let actualSvg = document.querySelector('#label svg');
    
    // vbp = view box parameters
    let vbp = actualSvg.getAttribute('viewBox').split(' ');

    const canvas = document.createElement('canvas');
    canvas.width = +vbp[2];
    canvas.height = +vbp[3];

    const ctx = canvas.getContext('2d');

    var img = new Image();
    img.src = "data:image/svg+xml;base64,"+btoa(svgContainer.innerHTML);

    img.onload = () => {
        ctx.drawImage(img, +vbp[0], +vbp[1]);
        canvas.toBlob((blob) => {
          this.downloadService.downloadFile(blob, this.mainText + '.jpeg');
        }, 'image/jpeg', 1);
    }
  }
}