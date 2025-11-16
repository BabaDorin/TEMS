import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-view-template',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, RouterModule, TranslateModule],
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
