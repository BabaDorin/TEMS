import { Component, Inject, Input, OnInit, Optional, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { PersonnelService } from 'src/app/services/personnel.service';
import { FormlyParserService } from '../../services/formly-parser.service';
import { SnackService } from '../../services/snack.service';
import { SendEmail } from './../../models/email/send-email.model';
import { FormlyData } from './../../models/formly/formly-data.model';
import { IOption } from './../../models/option.model';
import { EmailService } from './../../services/email.service';
import { TEMSComponent } from './../../tems/tems.component';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.scss']
})
export class SendEmailComponent extends TEMSComponent implements OnInit {
  
  // BEFREE: Reactive forms

  @ViewChild('recievers') recieversChipsAutocomplete: ChipsAutocompleteComponent;

  public formlyData = new FormlyData();

  @Input() personnel: IOption[] = [];
  personnelNames: string;
  dialogRef;

  constructor(
    private formlyParser: FormlyParserService,
    private snackService: SnackService,
    private emailService: EmailService,
    public personnelService: PersonnelService,
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    super();

    if(dialogData != undefined){
      this.personnel = dialogData.personnel;
    }
  }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParser.parseSendEmail();
    this.personnelNames = this.personnel.map(q => q.label).join(", ");
  }

  onSubmit(model){
    this.personnel = this.recieversChipsAutocomplete.options;

    if(this.personnel == undefined || this.personnel.length == 0)
    {
      this.snackService.snack({message: "What about specifiying some recievers? :)", status: 0});
      return;  
    }

    let sendEmailModel = new SendEmail();
    sendEmailModel.from = model.from;
    sendEmailModel.subject = model.subject;
    sendEmailModel.text = model.text;
    sendEmailModel.addressees = this.personnel.map(q => q.value);

    if(!sendEmailModel.validate())
    {
      this.snackService.snack({message: "Complete all the fields, please", status: 0});
      return;  
    }

    this.subscriptions.push(
      this.emailService.sendEmail(sendEmailModel)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1 && this.dialogRef != undefined){
          this.dialogRef.close();
        } 
      })
    )
  }
}
