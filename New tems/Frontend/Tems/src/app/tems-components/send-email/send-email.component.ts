import { Component, OnInit, OnDestroy, Inject, Optional, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatSelectModule } from '@angular/material/select';
import { TEMS_FORMS_IMPORTS } from 'src/app/shared/constants/tems-forms-imports.const';
import { FormlyParserService } from '../../services/formly-parser.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EmailService } from '../../services/email.service';
import { PersonnelService } from '../../services/personnel.service';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { FormlyData } from '../../models/formly/formly-data.model';
import { IOption } from '../../models/option.model';

interface ISendEmail {
  from: string;
  recipients: string[];
  subject: string;
  text: string;
}

@Component({
  selector: 'app-send-email',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    TranslateModule,
    MatSelectModule,
    ChipsAutocompleteComponent,
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.scss']
})
export class SendEmailComponent implements OnInit, OnDestroy {
  public formlyData = new FormlyData();
  @Input() personnel: IOption[] = [];
  public translate: TranslateService;
  subscriptions: any[] = [];

  constructor(
    translate: TranslateService,
    private formlyParser: FormlyParserService,
    private snackBar: MatSnackBar,
    private emailService: EmailService,
    public personnelService: PersonnelService,
    @Optional() public dialogRef: MatDialogRef<SendEmailComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.translate = translate;
  }

  ngOnInit(): void {}

  onSubmit(model: any): void {
    this.sendEmail();
  }

  sendEmail(): void {
    const model = this.formlyData?.model || {};
    const sendEmailModel: ISendEmail = {
      from: model.from || '',
      recipients: this.personnel.map(p => p.value),
      subject: model.subject || '',
      text: model.text || ''
    };

    if (!sendEmailModel.from || !sendEmailModel.subject || !sendEmailModel.text || sendEmailModel.recipients.length === 0) {
      this.snackBar.open(this.translate.instant('email.validationFailed'), '', { duration: 3000 });
      return;
    }

    this.subscriptions.push(
      this.emailService.sendEmail(sendEmailModel).subscribe({
        next: () => {
          this.snackBar.open(this.translate.instant('email.sent'), '', { duration: 3000 });
          this.dialogRef?.close(true);
        },
        error: () => this.snackBar.open(this.translate.instant('email.sendFailed'), '', { duration: 3000 })
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
  }
}
