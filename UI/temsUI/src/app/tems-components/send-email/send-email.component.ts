import { IOption } from './../../models/option.model';
import { SendEmail } from './../../models/email/send-email.model';
import { EmailService } from './../../services/email.service';
import { TEMSComponent } from './../../tems/tems.component';
import { SnackService } from './../../services/snack/snack.service';
import { FormlyParserService } from './../../services/formly-parser-service/formly-parser.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.scss']
})
export class SendEmailComponent extends TEMSComponent implements OnInit {
  
  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  personnel: IOption[];
  personnelNames: string;
  dialogRef;

  constructor(
    private formlyParser: FormlyParserService,
    private snackService: SnackService,
    private emailService: EmailService,
  ) { 
    super();
  }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParser.parseSendEmail();
    this.personnelNames = this.personnel.map(q => q.label).join(", ");
  }

  onSubmit(model){
    if(this.personnel == undefined || this.personnel.length == 0)
    {
      this.snackService.snack({message: "You want me to send your email to... Whom?", status: 0});
      return;  
    }

    let sendEmailModel = new SendEmail();
    sendEmailModel.from = model.from;
    sendEmailModel.subject = model.subject;
    sendEmailModel.text = model.text;
    sendEmailModel.addressees = this.personnel.map(q => q.value);

    console.log(sendEmailModel);

    if(!sendEmailModel.validate())
    {
      this.snackService.snack({message: "Complete all the fields, please", status: 0});
      return;  
    }

    this.subscriptions.push(
      this.emailService.sendEmail(model)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1 && this.dialogRef != undefined){
          this.dialogRef.close();
        } 
      })
    )
  }
}
