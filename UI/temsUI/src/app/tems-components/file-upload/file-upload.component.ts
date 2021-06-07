import { Component, OnInit, Output, EventEmitter, ViewChild, TemplateRef } from '@angular/core';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent implements OnInit {

  nrOfFiles = 0;
  @Output() filesSelected = new EventEmitter();
  @ViewChild('dropContainer', {static: true}) dropContainer;

  constructor() { }

  ngOnInit(): void {
  }

  onFilesSelected(files){
    this.nrOfFiles = files.length;
    this.filesSelected.emit(files);
    this.dropContainer.nativeElement.classList.remove('fileover');
  }

  dragOver($event){
    $event.preventDefault();
    console.log(this.dropContainer);
    this.dropContainer.nativeElement.classList.add('fileover');
  }

  dragLeave($event){
    $event.preventDefault();
    console.log(this.dropContainer);
    this.dropContainer.nativeElement.classList.remove('fileover');
  }
}
