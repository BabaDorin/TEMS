import { DownloadService } from './../../../download.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-equipment-label',
  templateUrl: './equipment-label.component.html',
  styleUrls: ['./equipment-label.component.scss']
})
export class EquipmentLabelComponent implements OnInit {

  @Input() mainText;
  @Input() downloadable = true;

  constructor(private downloadService: DownloadService) { }

  ngOnInit(): void {
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