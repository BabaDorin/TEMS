import { TEMSComponent } from './../../tems/tems.component';
import { Component, Inject, Input, OnInit, Output, EventEmitter, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { DialogButtons } from 'src/app/models/confirm/confirm-buttons';

@Component({
  selector: 'app-confirm',
  templateUrl: './confirm.component.html',
  styleUrls: ['./confirm.component.scss']
})
export class ConfirmComponent extends TEMSComponent implements OnInit {

  @Input() message;
  @Input() buttons: DialogButtons = DialogButtons.YesNo;
  @Output() optionSelected = new EventEmitter();

  confirmButtonText: string;
  cancelButtonText: string;
  
  constructor(
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(this.dialogData != undefined){
      this.message = this.dialogData.message;
    }

    if(this.buttons = DialogButtons.YesNo){
      this.confirmButtonText = this.translate.instant('general.dialog_yes');
      this.cancelButtonText = this.translate.instant('general.dialog_no');
    }
    else
    {
      this.confirmButtonText = this.translate.instant('general.dialog_ok');
      this.cancelButtonText = this.translate.instant('general.dialog_cancel');
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
