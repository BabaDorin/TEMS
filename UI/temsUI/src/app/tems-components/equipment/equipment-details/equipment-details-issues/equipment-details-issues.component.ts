import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';
import { CreateIssueComponent } from './../../../issue/create-issue/create-issue.component';
import { IssuesService } from './../../../../services/issues-service/issues.service';
import { Component, OnInit, Input } from '@angular/core';
import { ViewIssue } from 'src/app/models/communication/issues/view-issue';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-equipment-details-issues',
  templateUrl: './equipment-details-issues.component.html',
  styleUrls: ['./equipment-details-issues.component.scss']
})
export class EquipmentDetailsIssuesComponent implements OnInit {

  // This component can be used to display all issues, or issues of a specific 
  // room / equipment / personnel
  @Input() roomId: string;
  @Input() personnelId: string;
  @Input() equipment: ViewEquipmentSimplified;

  issues: ViewIssue[];

  constructor(
    private issuesService: IssuesService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
    if(this.roomId != undefined)
      this.issues = this.issuesService.getIssuesOfRoom(this.roomId);
    else if(this.personnelId != undefined)
      this.issues = this.issuesService.getIssuesOfPersonnel(this.personnelId);
    else if (this.equipment != undefined)
      this.issues = this.issuesService.getIssuesOfEquipment(this.equipment.id);
    else
      this.issues = this.issuesService.getIssues();
  }

  addIssue(){
    // Displays CreateIssue component in a modal
    this.openCreateIssueDialog();
  }
  
  openCreateIssueDialog(): void {
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(CreateIssueComponent); 
    dialogRef.componentInstance.equipmentAlreadySelected = [{
      id: this.equipment.id, 
      value: this.equipment.temsidOrSn
    }];

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
}
