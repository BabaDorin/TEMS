import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue';
import { ViewIssue } from './../../models/communication/issues/view-issue';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class IssuesService {

  constructor() { }

  getIssues(onlyClosed?: boolean, includingClosed?: boolean){
    return [
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
      new ViewIssueSimplified(),
    ];
  }

  getIssuesOfRoom(roomId: string, onlyClosed?: boolean, includingClosed?: boolean){
    return this.getIssues(onlyClosed, includingClosed);
  }

  getIssuesOfEquipment(equipmentId: string, onlyClosed?: boolean, includingClosed?: boolean){
    return this.getIssues(onlyClosed, includingClosed);
  }

  getIssuesOfPersonnel(personnelId: string, onlyClosed?: boolean, includingClosed?: boolean){
    return this.getIssues(onlyClosed, includingClosed);
  }
}
