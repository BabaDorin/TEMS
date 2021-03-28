import { CAN_MANAGE_ENTITIES } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { SnackService } from './../../services/snack/snack.service';
import { Router } from '@angular/router';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { IOption } from 'src/app/models/option.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { CreateIssueComponent } from './../issue/create-issue/create-issue.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { IssuesService } from './../../services/issues-service/issues.service';
import { Component, Input, OnInit, OnChanges, ViewChild } from '@angular/core';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';

@Component({
  selector: 'app-entity-issues-list',
  templateUrl: './entity-issues-list.component.html',
  styleUrls: ['./entity-issues-list.component.scss'],
})
export class EntityIssuesListComponent extends TEMSComponent implements OnInit, OnChanges {

  // for ticket fetching
  @Input() equipmentId: string = "any";
  @Input() roomId: string = "any";
  @Input() addIssueEnabled: boolean = true;
  @Input() personnelId: string = "any";

  // for AddIssueComponent
  @Input() equipment: IOption[] = [];
  @Input() rooms: IOption[] = [];
  @Input() personnel: IOption[] =[];

  @Input() onlyClosed: boolean = false;
  @Input() includingClosed: boolean = false;

  @ViewChild('includeClosedToggle') includeClosedToggle;

  issues: ViewIssueSimplified[];
  canManage = false;
  statuses: IOption[];

  constructor(
    private issuesService: IssuesService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private router: Router,
    private tokenService: TokenService
  ) { 
    super();
  }
 
  ngOnInit(): void {
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);
    this.getStatuses();
    this.getIssues();
  }

  ngOnChanges(): void {
    this.getIssues();
  }

  getIssues(){
    this.subscriptions.push(this.issuesService.getIssues(
      this.equipmentId, this.roomId, this.personnelId, this.includingClosed, this.onlyClosed)
      .subscribe(result => {
        this.issues = result;
      }))
  }

  getStatuses(){
    this.subscriptions.push(
      this.issuesService.getStatuses()
      .subscribe(result => {
        this.statuses = result;
      })
    )
  }

  statusChanged(eventData, issueId, index){
    this.subscriptions.push(
      this.issuesService.changeIssueStatus(issueId, eventData.value.value)
      .subscribe(result => {
        if(result.status == 0){
          this.snackService.snack(result);
          return;
        }

        this.issues[index].status = eventData.value;
      })
    )
  }

  solve(issueId: string, index: number){
    this.subscriptions.push(
      this.issuesService.closeIssue(issueId)
      .subscribe(result => {
        if(result.status == 0){
          this.snackService.snack(result);
          return;
        }

        this.issues[index].status = {value: 'closed', label: 'Closed'};
      })
    )
  }

  private addIssue(){
    this.dialogService.openDialog(
      CreateIssueComponent,
      [
        { label: "equipmentAlreadySelected", value: this.equipment},
        { label: "roomsAlreadySelected", value: this.rooms},
        { label: "personnelAlreadySelected", value: this.personnel},
      ],
      () => {
        this.ngOnInit();
      }
    )
  }

  includeClosedChanged(){
    this.includingClosed = !this.includingClosed;
    this.getIssues()
  }

  viewEquipment(equipmentId: string){
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(()=>
    this.router.navigate(['/equipment/details/' + equipmentId]));
  }

  viewRoom(roomId: string){
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(()=>
    this.router.navigate(['/rooms/details/' + roomId]));
  }
  
  viewPersonnel(personnelId: string){
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(()=>
    this.router.navigate(['/personnel/details/' + personnelId]));
  }

  remove(issueId: string, index: number){
    if(!confirm("Are you sure you want to remove that issue?"))
      return;

    this.subscriptions.push(
      this.issuesService.remove(issueId)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.issues.splice(index, 1);
      })
    )
  }
}

