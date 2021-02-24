import { CreateIssueComponent } from './../issue/create-issue/create-issue.component';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { IssuesService } from './../../services/issues-service/issues.service';
import { Component, Input, OnInit } from '@angular/core';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';

@Component({
  selector: 'app-entity-issues-list',
  templateUrl: './entity-issues-list.component.html',
  styleUrls: ['./entity-issues-list.component.scss']
})
export class EntityIssuesListComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  @Input() room: ViewRoomSimplified;
  @Input() personnel: ViewPersonnelSimplified;

  issues: ViewIssueSimplified[];
  @Input() addLogEnabled: boolean = true;
  
  constructor(
    private issuesService: IssuesService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
    if(this.equipment == undefined && this.room == undefined && this.personnel == undefined){
      console.warn('EntityIssuesListComponent requires an entity in order to display logs');
      return;
    }

    if(this.equipment)
      this.issues = this.issuesService.getIssuesOfEquipment(this.equipment.id);

    if(this.room)
      this.issues = this.issuesService.getIssuesOfRoom(this.room.id);

    if(this.personnel)
      this.issues = this.issuesService.getIssuesOfPersonnel(this.personnel.id);
  }

  private addIssue(){
    // Opens a dialog containing an instance of CreateIssueComponent
    
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(CreateIssueComponent); 
    
    if(this.equipment){
      dialogRef.componentInstance.equipmentAlreadySelected = [
        {
          id: this.equipment.id, 
          value: this.equipment.temsidOrSn
        }];
    }

    if(this.room){
      dialogRef.componentInstance.roomsAlreadySelected = [
        {
          id: this.room.id, 
          value: this.room.identifier
        }];
    }

    if(this.personnel){
      dialogRef.componentInstance.personnelAlreadySelected = [
        {
          id: this.personnel.id, 
          value: this.personnel.name
        }];
    }

    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }

}
