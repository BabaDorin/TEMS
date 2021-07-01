import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

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
