import { TEMSComponent } from './../../tems/tems.component';
import { Component, Inject, Input, OnInit, Output, EventEmitter, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-confirm',
  templateUrl: './confirm.component.html',
  styleUrls: ['./confirm.component.scss']
})
export class ConfirmComponent extends TEMSComponent implements OnInit {

  @Input() message;
  @Output() optionSelected = new EventEmitter();

  constructor(
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(this.dialogData != undefined){
      this.message = this.dialogData.message;
    }
  }

  ngOnInit(): void {
  }

  cancel(){
    this.optionSelected.emit(false);
    this.dialogRef.close();
  }

  ok(){
    this.optionSelected.emit(true);
    this.dialogRef.close();
  }
}
