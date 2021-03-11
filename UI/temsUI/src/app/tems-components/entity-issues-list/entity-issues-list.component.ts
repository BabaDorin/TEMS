import { TEMSComponent } from 'src/app/tems/tems.component';
import { CreateIssueComponent } from './../issue/create-issue/create-issue.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { IssuesService } from './../../services/issues-service/issues.service';
import { Component, Input, OnInit } from '@angular/core';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';

@Component({
  selector: 'app-entity-issues-list',
  templateUrl: './entity-issues-list.component.html',
  styleUrls: ['./entity-issues-list.component.scss']
})
export class EntityIssuesListComponent extends TEMSComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  @Input() room: ViewRoomSimplified;
  @Input() personnel: ViewPersonnelSimplified;

  @Input() onlyClosed: boolean = false;
  @Input() includingClosed: boolean = false;

  issues: ViewIssueSimplified[];
  @Input() addLogEnabled: boolean = true;
  
  constructor(
    private issuesService: IssuesService,
    public dialog: MatDialog
  ) { 
    super();
  }

  ngOnInit(): void {
    if(this.equipment)
      this.subscriptions.push(this.issuesService.getIssuesOfEquipment(
        this.equipment.id, this.onlyClosed, this.includingClosed
      ).subscribe(response => {
        console.log(response);
        this.issues = response;
      }))

    if(this.room)
      this.subscriptions.push(this.issuesService.getIssuesOfRoom(
        this.room.id, this.includingClosed, this.onlyClosed
      ).subscribe(result => {
        console.log(result);
        this.issues = result;
      }))

    if(this.personnel)
      this.subscriptions.push(this.issuesService.getIssuesOfEquipment(
        this.personnel.id, this.includingClosed, this.onlyClosed
      ).subscribe(result => {
        console.log(result);
        this.issues = result;
      }))

    if(this.personnel == undefined && this.room == undefined && this.equipment == undefined){
      this.subscriptions.push(this.issuesService.getIssues(
        this.includingClosed, this.onlyClosed
      ).subscribe(result => {
        console.log(result);
        this.issues = result;
      }))
    }
  }

  private addIssue(){
    // Opens a dialog containing an instance of CreateIssueComponent
    
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(CreateIssueComponent); 
    
    if(this.equipment){
      dialogRef.componentInstance.equipmentAlreadySelected = [
        {
          value: this.equipment.id, 
          label: this.equipment.temsIdOrSerialNumber
        }];
    }

    if(this.room){
      dialogRef.componentInstance.roomsAlreadySelected = [
        {
          valued: this.room.id, 
          label: this.room.identifier
        }];
    }

    if(this.personnel){
      dialogRef.componentInstance.personnelAlreadySelected = [
        {
          value: this.personnel.id, 
          label: this.personnel.name
        }];
    }

    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }
}
