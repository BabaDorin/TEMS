import { TEMSComponent } from 'src/app/tems/tems.component';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { AddAnnouncement } from 'src/app/models/communication/announcement/add-announcement.model';
import { CommunicationService } from 'src/app/services/communication.service';

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

  constructor(
    private formlyParserService: FormlyParserService,
    private communicationService: CommunicationService
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
        console.log(result);
      }))
  }

}
