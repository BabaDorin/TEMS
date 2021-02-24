import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue';
import { ViewIssue } from './../../models/communication/issues/view-issue';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class IssuesService {

  constructor() { }

  getIssues(){
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

  getIssuesOfRoom(roomId: string){
    return this.getIssues();
  }

  getIssuesOfEquipment(equipmentId: string){
    return this.getIssues();
  }

  getIssuesOfPersonnel(personnelId: string){
    return this.getIssues();
  }
}
