import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-file-upload',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, TranslateModule],
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent {

  nrOfFiles = 0;
  @Output() filesSelected = new EventEmitter<FileList>();

  constructor() { }

  onFilesSelected(files){
    this.nrOfFiles = files.length;
    this.filesSelected.emit(files);
  }

  dragOver($event){
    $event.preventDefault();
  }

  dragLeave($event){
    $event.preventDefault();
  }
}
