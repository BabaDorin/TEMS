import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { IOption } from './../../models/option.model';
import { SendEmail } from './../../models/email/send-email.model';
import { EmailService } from './../../services/email.service';
import { TEMSComponent } from './../../tems/tems.component';
import { SnackService } from './../../services/snack/snack.service';
import { FormlyParserService } from './../../services/formly-parser-service/formly-parser.service';
import { Component, Inject, Input, OnInit, Optional, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.scss']
})
export class SendEmailComponent extends TEMSComponent implements OnInit {
  
  @ViewChild('recievers') recieversChipsAutocomplete: ChipsAutocompleteComponent;

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  @Input() personnel: IOption[] = [];
  personnelNames: string;
  dialogRef;

  constructor(
    private formlyParser: FormlyParserService,
    private snackService: SnackService,
    private emailService: EmailService,
    private equipmentService: EquipmentService,
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
    this.personnel = this.recieversChipsAutocomplete.value;

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

    console.log(sendEmailModel);

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
