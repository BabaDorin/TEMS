import { CAN_MANAGE_ENTITIES } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { SnackService } from './../../services/snack/snack.service';
import { Router } from '@angular/router';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { IOption } from 'src/app/models/option.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { CreateIssueComponent } from './../issue/create-issue/create-issue.component';
import { IssuesService } from './../../services/issues-service/issues.service';
import { Component, Input, OnInit, OnChanges, ViewChild } from '@angular/core';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import * as confetti from 'canvas-confetti';

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
  @Input() endPoint;

  // for AddIssueComponent
  @Input() equipment: IOption[] = [];
  @Input() rooms: IOption[] = [];
  @Input() personnel: IOption[] =[];

  @Input() onlyClosed: boolean = false;
  @Input() includingClosed: boolean = false;
  @Input() showIncludeClosed: boolean = true;

  @ViewChild('includeClosedToggle') includeClosedToggle;
  @ViewChild('issuesPanel', { static: true }) issuesPanel;

  issues: ViewIssueSimplified[];
  canManage = false;
  statuses: IOption[];
  loading = true;
  pageNumber = 1;

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
    this.loading = true;
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);
    this.getStatuses();
    this.getIssues();  
  }

  ngOnChanges(): void {
    if(this.cancelFirstOnChange){
      this.cancelFirstOnChange = false;
      return;
    }
    
    this.getIssues();
  }

  getIssues(){
    this.loading = true;

    if(this.endPoint == undefined)
      this.endPoint =  this.issuesService.getIssues(
        this.equipmentId, this.roomId, this.personnelId, this.includingClosed, this.onlyClosed);

    this.subscriptions.push(
      this.endPoint
      .subscribe(result => {
        if(this.snackService.snackIfError(this.issues))
          return;

        this.issues = result;
        this.loading = false;
      }))
  }

  sortBy(criterium: string){
    if(criterium == 'date')
    {
      this.issues = this.issues.sort((a,b)=> new Date(a.dateCreated).getTime() - new Date(b.dateCreated).getTime());
      return;
    }

    this.issues = this.issues.sort((a,b) => (parseInt(a.status.additional) > parseInt(b.status.additional)) ? 1 : ((b.status.additional > a.status.additional) ? -1 : 0));
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

        this.issues[index + (this.pageNumber-1)*20].status = eventData.value;
      })
    )
  }

  solve(issueId, index){
    this.subscriptions.push(
      this.issuesService.closeIssue(issueId)
      .subscribe(result => {
        if(result.status == 0){
          this.snackService.snack(result);
          return;
        }

        let selectedIssue = this.issues[index + (this.pageNumber-1)*20];
        selectedIssue.dateClosed = new Date;
        let d1 = new Date(selectedIssue.dateClosed);
        let d2 = new Date(selectedIssue.dateCreated);
        let difference = Math.abs(d1.getTime() - d2.getTime()) / 36e5;
        
        if(difference <= 24)
          this.launchConfetti();

        this.snackService.snack({message: "🎉🎉 Let's close them all!", status: 1}, 'default-snackbar')
      })
    )
  }

  reopen(issueId, index){
    this.subscriptions.push(
      this.issuesService.reopenIssue(issueId)
      .subscribe(result => {
        if(result.status == 0){
          this.snackService.snack(result);
          return;
        }

        this.issues[index + (this.pageNumber-1)*20].dateClosed = undefined;
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

  remove(issueId, index){
    if(!confirm("Are you sure you want to remove that issue?"))
      return;

    this.subscriptions.push(
      this.issuesService.archieve(issueId)
      .subscribe(result => {
        this.snackService.snack(result);
        if(result.status == 1)
          this.issues.splice(index + (this.pageNumber-1)*20, 1);
      })
    )
  }

  launchConfetti(){
    confetti.create(undefined, { resize: true, useWorker: true })({
      particleCount: 130,
      spread: 130,
      origin: { y: 0.6 }
    });
  }
}

