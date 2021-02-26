import { AddAnnouncementComponent } from './../add-announcement/add-announcement.component';
import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ViewAnnouncement } from '../models/communication/announcement/view-announcement.model';
import { CommunicationService } from '../services/communication-service/communication.service';

@Component({
  selector: 'app-announcements-list',
  templateUrl: './announcements-list.component.html',
  styleUrls: ['./announcements-list.component.scss']
})
export class AnnouncementsListComponent implements OnInit {

  announcements: ViewAnnouncement[];
  constructor(
    private communicationService: CommunicationService,
    public dialog: MatDialog
  ) { 

  }

  ngOnInit(): void {
    this.announcements = this.communicationService.getAnnouncements();
  }

  addAnnouncement(){
    
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(AddAnnouncementComponent); 

    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }
}
