import { SnackService } from './../../../services/snack.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddAnnouncement } from 'src/app/models/communication/announcement/add-announcement.model';
import { CommunicationService } from 'src/app/services/communication.service';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-add-announcement',
  templateUrl: './add-announcement.component.html',
  styleUrls: ['./add-announcement.component.scss']
})
export class AddAnnouncementComponent extends TEMSComponent implements OnInit {

  formlyDataModel = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  dialogRef;

  constructor(
    public formlyParserService: FormlyParserService,
    private communicationService: CommunicationService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.formlyDataModel.fields = this.formlyParserService.parseAddAnnouncement();
  }

  onSubmit() {

    let addAnnouncement: AddAnnouncement = this.formlyDataModel.model.announcement;
    console.log(this.formlyDataModel.model)

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
