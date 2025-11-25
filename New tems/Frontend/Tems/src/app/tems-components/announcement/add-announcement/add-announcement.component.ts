import { SnackService } from './../../../services/snack.service';
import { Component, OnInit, Inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { TEMS_FORMS_IMPORTS } from 'src/app/modules/tems-forms/tems-forms.module';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddAnnouncement } from 'src/app/models/communication/announcement/add-announcement.model';
import { CommunicationService } from 'src/app/services/communication.service';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-add-announcement',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    TranslateModule,
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './add-announcement.component.html',
  styleUrls: ['./add-announcement.component.scss']
})
export class AddAnnouncementComponent extends TEMSComponent implements OnInit {

  formlyDataModel = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  constructor(
    public formlyParserService: FormlyParserService,
    private communicationService: CommunicationService,
    private snackService: SnackService,
    @Optional() public dialogRef: MatDialogRef<AddAnnouncementComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit(): void {
    this.formlyDataModel.fields = this.formlyParserService.parseAddAnnouncement();
  }

  onSubmit() {

    let addAnnouncement: AddAnnouncement = this.formlyDataModel.model.announcement;

    this.subscriptions.push(this.communicationService.createAnnouncement(addAnnouncement)
      .subscribe(result => {
        if (this.snackService.snackIfError(result))
          return;

        if (result?.status == 1 && this.dialogRef != undefined) {
          this.dialogRef.close();
        }
      }))
  }
}
