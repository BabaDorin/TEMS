import { SnackService } from '../../../services/snack.service';
import { AddAnnouncement } from '../../../models/communication/announcement/add-announcement.model';
import { CommunicationService } from '../../../services/communication.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';

@Component({
  selector: 'app-add-announcement',
  templateUrl: './add-announcement.component.html',
  styleUrls: ['./add-announcement.component.scss']
})
export class AddAnnouncementComponent extends TEMSComponent implements OnInit {

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  dialogRef;

  constructor(
    private formlyParserService: FormlyParserService,
    private communicationService: CommunicationService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParserService.parseAddAnnouncement();
  }

  onSubmit(){

    let addAnnouncement: AddAnnouncement = this.formlyData.model.announcement;
    console.log(this.formlyData.model)
    
    this.subscriptions.push(this.communicationService.createAnnouncement(addAnnouncement)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        console.log(result);
        console.log(this.dialogRef);
        if(result.status == 1)
        {
          if(this.dialogRef != undefined)
            this.dialogRef.close();

          this.formlyData.model.announcement.title = "";
          this.formlyData.model.announcement.text = "";
        }
      }))
  }

}
