import { ClaimService } from './../../../services/claim.service';
import { DialogService } from '../../../services/dialog.service';
import { CAN_MANAGE_SYSTEM_CONFIGURATION } from './../../../models/claims';
import { TokenService } from '../../../services/token.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { AddAnnouncementComponent } from './../add-announcement/add-announcement.component';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ViewAnnouncement } from '../../../models/communication/announcement/view-announcement.model';
import { CommunicationService } from '../../../services/communication.service';
import { CAN_MANAGE_ANNOUNCEMENTS } from 'src/app/models/claims';

@Component({
  selector: 'app-announcements-list',
  templateUrl: './announcements-list.component.html',
  styleUrls: ['./announcements-list.component.scss']
})
export class AnnouncementsListComponent extends TEMSComponent implements OnInit {

  @Input() skip: number;
  @Input() take: number;
  @Input() displayCreateNew: boolean = false;

  announcements: ViewAnnouncement[];
  constructor(
    private communicationService: CommunicationService,
    public dialog: MatDialog,
    public claims: ClaimService,
    private dialogService: DialogService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.fetchAnnouncements();
  }

  fetchAnnouncements(){
    let endPoint = 
    (this.skip != undefined && this.take != undefined)
    ? this.communicationService.getAnnouncements(this.skip, this.take)
    : this.communicationService.getAnnouncements();

  this.subscriptions.push(
    endPoint
    .subscribe(result => {
      console.log(result);
      this.announcements = result;
    }));
  }

  addAnnouncement(){
    this.dialogService.openDialog(
      AddAnnouncementComponent,
      undefined,
      () => {
        this.fetchAnnouncements();
      }
    );
  }
}
