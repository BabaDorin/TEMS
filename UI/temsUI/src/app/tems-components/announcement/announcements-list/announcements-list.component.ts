import { TEMSComponent } from 'src/app/tems/tems.component';
import { AddAnnouncementComponent } from './../add-announcement/add-announcement.component';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ViewAnnouncement } from '../../../models/communication/announcement/view-announcement.model';
import { CommunicationService } from '../../../services/communication-service/communication.service';

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
    public dialog: MatDialog
  ) { 
    super();
  }

  ngOnInit(): void {
    let endPoint = 
      (this.skip != undefined && this.take != undefined)
      ? this.communicationService.getAnnouncements(this.skip, this.take)
      : this.communicationService.getAnnouncements();

    this.subscriptions.push(this.communicationService.getAnnouncements()
      .subscribe(result => {
        this.announcements = result;
      }));
  }

  addAnnouncement(){
    
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(AddAnnouncementComponent); 

    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }
}
