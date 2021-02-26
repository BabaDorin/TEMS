import { Component, OnInit } from '@angular/core';
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
    private communicationService: CommunicationService
  ) { 

  }

  ngOnInit(): void {
    this.announcements = this.communicationService.getAnnouncements();
  }

  addAnnouncement(){
    console.warn('AddAnnouncenementComponent has not ben implemented yet.');
  }

}
