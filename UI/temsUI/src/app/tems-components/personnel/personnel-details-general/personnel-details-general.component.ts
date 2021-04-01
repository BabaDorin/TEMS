import { SendEmailComponent } from './../../send-email/send-email.component';
import { AddPersonnelComponent } from './../add-personnel/add-personnel.component';
import { DialogService } from './../../../services/dialog-service/dialog.service';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { ViewPersonnel } from './../../../models/personnel/view-personnel.model';
import { Component, Input, OnInit } from '@angular/core';
import { Property } from 'src/app/models/equipment/view-property.model';
import { Router } from '@angular/router';
import { SnackService } from 'src/app/services/snack/snack.service';

@Component({
  selector: 'app-personnel-details-general',
  templateUrl: './personnel-details-general.component.html',
  styleUrls: ['./personnel-details-general.component.scss']
})
export class PersonnelDetailsGeneralComponent extends TEMSComponent implements OnInit {

  @Input() personnelId: string;
  @Input() personnel: ViewPersonnel;
  @Input() displayViewMore: boolean = false;
  dialogRef;

  personnelProperties: Property[];
  constructor(
    private personnelService: PersonnelService,
    private route: Router,
    private snackService: SnackService,
    private dialogService: DialogService
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.personnelService.getPersonnelById(this.personnelId)
      .subscribe(response => {
        console.log(response);
        this.personnel = response;

        this.personnelProperties = [
          { displayName: 'Name', value: this.personnel.name },
          { displayName: 'Position', value: "display them in a fancy way" },
          { displayName: 'Phone Number', value: this.personnel.phoneNumber },
          { displayName: 'Email', value: this.personnel.email },
          { displayName: 'Active equipment allocations', value: this.personnel.allocatedEquipments },
          { displayName: 'Active tickets', value: this.personnel.activeTickets },
          { displayName: 'Room supervisories', value: "Display them in a fancy way" },
        ];
      })
    )
  }

  viewMore(){
    if(this.dialogRef != undefined)
      this.dialogRef.close();
  }

  edit(){
    this.dialogService.openDialog(
      AddPersonnelComponent,
      [{label: "personnelId", value: this.personnelId}],
      () => {
        this.ngOnInit();
      }
    )
  }

  sendMail(){
    this.dialogService.openDialog(
      SendEmailComponent,
      [{label: "personnel", value: [{label: this.personnel.name, value: this.personnel.id}]}]
    );
  }
}
