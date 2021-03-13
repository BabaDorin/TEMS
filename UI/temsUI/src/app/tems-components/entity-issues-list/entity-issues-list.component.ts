import { IOption } from 'src/app/models/option.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { CreateIssueComponent } from './../issue/create-issue/create-issue.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { IssuesService } from './../../services/issues-service/issues.service';
import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-entity-issues-list',
  templateUrl: './entity-issues-list.component.html',
  styleUrls: ['./entity-issues-list.component.scss'],
})
export class EntityIssuesListComponent extends TEMSComponent implements OnInit, OnChanges {

  // for ticket fetching
  @Input() equipmentId: string = "any";
  @Input() roomId: string = "any";
  @Input() personnelId: string = "any";

  // for AddIssueComponent
  @Input() equipment: IOption[] = [];
  @Input() rooms: IOption[] = [];
  @Input() personnel: IOption[] =[];

  @Input() onlyClosed: boolean = false;
  @Input() includingClosed: boolean = false;

  issues: Observable<ViewIssueSimplified[]>;
  @Input() addIssueEnabled: boolean = true;
  
  constructor(
    private issuesService: IssuesService,
    public dialog: MatDialog
  ) { 
    super();
  }
 
  ngOnInit(): void {
    this.getIssues();
  }

  ngOnChanges(): void {
    this.getIssues();
  }

  getIssues(){
    this.subscriptions.push(this.issuesService.getIssues(
      this.equipmentId, this.roomId, this.personnelId, this.includingClosed, this.onlyClosed)
      .subscribe(result => {
        this.issues = of(result);
      }))
  }

  private addIssue(){
    // Opens a dialog containing an instance of CreateIssueComponent
    
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(CreateIssueComponent);
    
    dialogRef.componentInstance.equipmentAlreadySelected = this.equipment;
    dialogRef.componentInstance.roomsAlreadySelected = this.rooms;
    dialogRef.componentInstance.personnelAlreadySelected = this.personnel;
    
    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }
}

