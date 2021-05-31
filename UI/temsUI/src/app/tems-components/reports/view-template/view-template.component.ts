import { Component, OnInit, Input } from '@angular/core';
import { Report } from 'src/app/models/report/report.model';

@Component({
  selector: 'app-view-template',
  templateUrl: './view-template.component.html',
  styleUrls: ['./view-template.component.scss']
})
export class ViewTemplateComponent implements OnInit {

  @Input() template;
  dialogRef;
  
  constructor() { }

  ngOnInit(): void {
  }

  closeDialog(){
    this.dialogRef.close();
  }

}
