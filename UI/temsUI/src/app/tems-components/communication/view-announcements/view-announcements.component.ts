import { CommunicationService } from './../../../services/communication-service/communication.service';
import { ViewAnnouncement } from './../../../models/communication/announcement/view-announcement.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-announcements',
  templateUrl: './view-announcements.component.html',
  styleUrls: ['./view-announcements.component.scss']
})
export class ViewAnnouncementsComponent implements OnInit {

  constructor() { 

  }

  ngOnInit(): void {
  }
}
