import { Component, OnInit, Input, Inject, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Report } from 'src/app/models/report/report.model';

@Component({
  selector: 'app-view-template',
  templateUrl: './view-template.component.html',
  styleUrls: ['./view-template.component.scss']
})
export class ViewTemplateComponent implements OnInit {

  @Input() template;
  dialogRef;
  
  constructor(
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    if(dialogData != undefined){
      this.template = dialogData.template;
    }
  }

  ngOnInit(): void {
  }

  closeDialog(){
    this.dialogRef.close();
  }

}
