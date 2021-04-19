import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent implements OnInit {

  nrOfFiles = 0;
  @Output() filesSelected = new EventEmitter();
  constructor() { }

  ngOnInit(): void {
  }

  onFilesSelected(files){
    this.nrOfFiles = files.length;
    this.filesSelected.emit(files);
  }
}
