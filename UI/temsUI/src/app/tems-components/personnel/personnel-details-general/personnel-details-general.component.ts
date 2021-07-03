import { Component, EventEmitter, Inject, Input, OnInit, Optional, Output } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Property } from 'src/app/models/equipment/view-property.model';
import { SnackService } from 'src/app/services/snack.service';
import { DialogService } from '../../../services/dialog.service';
import { PersonnelService } from '../../../services/personnel.service';
import { TokenService } from '../../../services/token.service';
import { CAN_MANAGE_ENTITIES } from './../../../models/claims';
import { ViewPersonnel } from './../../../models/personnel/view-personnel.model';
import { ClaimService } from './../../../services/claim.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { SendEmailComponent } from './../../send-email/send-email.component';
import { AddPersonnelComponent } from './../add-personnel/add-personnel.component';

@Component({
  selector: 'app-personnel-details-general',
  templateUrl: './personnel-details-general.component.html',
  styleUrls: ['./personnel-details-general.component.scss']
})
export class PersonnelDetailsGeneralComponent extends TEMSComponent implements OnInit {

  @Input() personnelId: string;
  @Output() archivationStatusChanged = new EventEmitter();
  @Input() personnel: ViewPersonnel;
  @Input() displayViewMore: boolean = false;
  dialogRef;
  headerClass;
  canManage: boolean = false;

  personnelProperties: Property[];
  constructor(
    private personnelService: PersonnelService,
    private route: Router,
    private snackService: SnackService,
    private dialogService: DialogService,
    private tockenService: TokenService,
    public claims: ClaimService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(dialogData != undefined){
      this.personnelId = dialogData.personnelId;
      this.displayViewMore = dialogData.displayViewMore;
    }
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.personnelService.getPersonnelById(this.personnelId)
      .subscribe(response => {
        this.personnel = response;
        this.headerClass = (this.personnel.isArchieved) ? 'text-muted' : '';
        
        this.personnelProperties = [
          { displayName: 'Name', value: this.personnel.name },
          { displayName: 'Phone Number', value: this.personnel.phoneNumber },
          { displayName: 'Email', value: this.personnel.email },
          { displayName: 'Active equipment allocations', value: this.personnel.allocatedEquipments },
          { displayName: 'Active tickets', value: this.personnel.activeTickets },
        ];
      })
    );

    this.canManage = this.tockenService.hasClaim(CAN_MANAGE_ENTITIES);
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
    if(!this.claims.canSendEmails)
      return;

    this.dialogService.openDialog(
      SendEmailComponent,
      [{label: "personnel", value: [{label: this.personnel.name, value: this.personnel.id}]}]
    );
  }

  archieve(){
    if(!this.personnel.isArchieved && !confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
      return;

    let newArchivationStatus = !this.personnel.isArchieved;
    this.subscriptions.push(
      this.personnelService.archievePersonnel(this.personnelId, newArchivationStatus)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.personnel.isArchieved = newArchivationStatus;
          this.headerClass = (this.personnel.isArchieved) ? 'text-muted' : '';

        this.archivationStatusChanged.emit(this.personnel.isArchieved);
      })
    )
  }
}
