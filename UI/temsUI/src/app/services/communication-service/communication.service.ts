import { ViewAnnouncement } from './../../models/communication/announcement/view-announcement.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommunicationService {
  constructor() { }

  getAnnouncements(){
    return [
      new ViewAnnouncement(),
      new ViewAnnouncement(),
      new ViewAnnouncement(),
      new ViewAnnouncement(),
      new ViewAnnouncement(),
    ];
  }
}
